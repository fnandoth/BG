using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NotificationService.Aplication.DTOs;
using NotificationService.Infrastructure.Data;
using Npgsql;
using PostService.Domain.Entities;
using PostService.Infrastructure.Data;
using PostService.IntegrationTests.Helpers;
using PostService.IntegrationTests.Models;
using Respawn;
using Respawn.Graph;
using UserService.Aplication.DTOs;
using UserService.Infrastructure.Data;
using Xunit;

namespace PostService.IntegrationTests.Fixtures;

public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private NpgsqlConnection? _userConnection;
    private NpgsqlConnection? _postConnection;
    private NpgsqlConnection? _notificationConnection;
    private Respawner? _userRespawner;
    private Respawner? _postRespawner;
    private Respawner? _notificationRespawner;

    public TestEnvironment Environment { get; private set; } = default!;

    public JsonSerializerOptions JsonOptions { get; } = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task InitializeAsync()
    {
        Environment = TestEnvironment.Load();

        await EnsureServicesHealthyAsync();

        _userConnection = new NpgsqlConnection(Environment.UserDbConnectionString);
        _postConnection = new NpgsqlConnection(Environment.PostDbConnectionString);
        _notificationConnection = new NpgsqlConnection(Environment.NotificationDbConnectionString);

        await _userConnection.OpenAsync();
        await _postConnection.OpenAsync();
        await _notificationConnection.OpenAsync();

        _userRespawner = await CreateRespawnerAsync(_userConnection);
        _postRespawner = await CreateRespawnerAsync(_postConnection);
        _notificationRespawner = await CreateRespawnerAsync(_notificationConnection);

        await ResetStateAsync();
    }

    public async Task DisposeAsync()
    {
        if (_notificationConnection is not null)
        {
            await _notificationConnection.DisposeAsync();
        }

        if (_postConnection is not null)
        {
            await _postConnection.DisposeAsync();
        }

        if (_userConnection is not null)
        {
            await _userConnection.DisposeAsync();
        }
    }

    public async Task ResetStateAsync()
    {
        ArgumentNullException.ThrowIfNull(_notificationRespawner);
        ArgumentNullException.ThrowIfNull(_postRespawner);
        ArgumentNullException.ThrowIfNull(_userRespawner);
        ArgumentNullException.ThrowIfNull(_notificationConnection);
        ArgumentNullException.ThrowIfNull(_postConnection);
        ArgumentNullException.ThrowIfNull(_userConnection);

        await _notificationRespawner.ResetAsync(_notificationConnection);
        await _postRespawner.ResetAsync(_postConnection);
        await _userRespawner.ResetAsync(_userConnection);
    }

    public async Task<TestUser> SeedUserAsync()
    {
        using var context = CreateUserContext();
        var (entity, model) = BogusDataFactory.CreateUser();
        context.Users.Add(entity);
        await context.SaveChangesAsync();
        return model;
    }

    public async Task<IReadOnlyList<TestUser>> SeedUsersAsync(int count)
    {
        using var context = CreateUserContext();
        var seeded = Enumerable.Range(0, count)
            .Select(_ => BogusDataFactory.CreateUser())
            .ToList();

        context.Users.AddRange(seeded.Select(item => item.Entity));
        await context.SaveChangesAsync();

        return seeded.Select(item => item.Model).ToList();
    }

    public async Task<Post> SeedPostAsync(
        TestUser author,
        string? content = null,
        DateTimeOffset? createdAt = null)
    {
        using var context = CreatePostContext();
        var post = BogusDataFactory.CreatePost(
            author,
            PostType.Post,
            content: content,
            createdAt: createdAt);

        context.Posts.Add(post);
        await context.SaveChangesAsync();
        return post;
    }

    public async Task<Post> SeedReplyAsync(
        TestUser author,
        Guid parentPostId,
        string? content = null,
        DateTimeOffset? createdAt = null)
    {
        using var context = CreatePostContext();
        var parent = await context.Posts.FirstAsync(post => post.Id == parentPostId);
        var reply = BogusDataFactory.CreatePost(
            author,
            PostType.Reply,
            content: content,
            parentPostId: parentPostId,
            createdAt: createdAt);

        parent.RepliesCount++;
        context.Posts.Add(reply);
        await context.SaveChangesAsync();
        return reply;
    }

    public async Task<Post> SeedQuoteAsync(
        TestUser author,
        Guid quotedPostId,
        string? content = null,
        DateTimeOffset? createdAt = null)
    {
        using var context = CreatePostContext();
        var quoted = await context.Posts.FirstAsync(post => post.Id == quotedPostId);
        var quote = BogusDataFactory.CreatePost(
            author,
            PostType.Quote,
            content: content,
            quotedPostId: quotedPostId,
            createdAt: createdAt);

        quoted.QuotesCount++;
        context.Posts.Add(quote);
        await context.SaveChangesAsync();
        return quote;
    }

    public async Task<Post> SeedRepostAsync(
        TestUser author,
        Guid originalPostId,
        DateTimeOffset? createdAt = null)
    {
        using var context = CreatePostContext();
        var original = await context.Posts.FirstAsync(post => post.Id == originalPostId);
        var repost = BogusDataFactory.CreatePost(
            author,
            PostType.Repost,
            content: string.Empty,
            originalPostId: originalPostId,
            createdAt: createdAt);

        original.RepostsCount++;
        context.Posts.Add(repost);
        await context.SaveChangesAsync();
        return repost;
    }

    public async Task<string> GetAccessTokenAsync(TestUser user)
    {
        using var client = CreateClient(Environment.UserServiceBaseUri);
        var response = await client.PostAsJsonAsync("/api/user/login", new UserLoginDTO
        {
            UserName = user.UserName,
            Password = user.Password
        });

        var auth = await response.ReadRequiredJsonAsync<AuthResponseDTO>(JsonOptions);
        return auth.AccessToken;
    }

    public HttpClient CreatePostClient()
    {
        return CreateClient(Environment.PostServiceBaseUri);
    }

    public HttpClient CreateAuthorizedPostClient(string accessToken)
    {
        return CreateAuthorizedClient(Environment.PostServiceBaseUri, accessToken);
    }

    public HttpClient CreateAuthorizedNotificationClient(string accessToken)
    {
        return CreateAuthorizedClient(Environment.NotificationServiceBaseUri, accessToken);
    }

    public async Task<PagedResult<NotificationDto>> GetNotificationsAsync(TestUser user, bool onlyUnread = false)
    {
        var accessToken = await GetAccessTokenAsync(user);
        using var client = CreateAuthorizedNotificationClient(accessToken);
        var response = await client.GetAsync($"/api/notifications?onlyUnread={onlyUnread.ToString().ToLowerInvariant()}");
        return await response.ReadRequiredJsonAsync<PagedResult<NotificationDto>>(JsonOptions);
    }

    private UserContext CreateUserContext()
    {
        var options = new DbContextOptionsBuilder<UserContext>()
            .UseNpgsql(Environment.UserDbConnectionString)
            .Options;

        return new UserContext(options);
    }

    private PostContext CreatePostContext()
    {
        var options = new DbContextOptionsBuilder<PostContext>()
            .UseNpgsql(Environment.PostDbConnectionString)
            .Options;

        return new PostContext(options);
    }

    private NotificationsContext CreateNotificationsContext()
    {
        var options = new DbContextOptionsBuilder<NotificationsContext>()
            .UseNpgsql(Environment.NotificationDbConnectionString)
            .Options;

        return new NotificationsContext(options);
    }

    private static HttpClient CreateClient(Uri baseAddress)
    {
        return new HttpClient
        {
            BaseAddress = baseAddress,
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    private static HttpClient CreateAuthorizedClient(Uri baseAddress, string accessToken)
    {
        var client = CreateClient(baseAddress);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private async Task EnsureServicesHealthyAsync()
    {
        await WaitForHealthAsync(Environment.UserServiceBaseUri, "UserService");
        await WaitForHealthAsync(Environment.PostServiceBaseUri, "PostService");
        await WaitForHealthAsync(Environment.NotificationServiceBaseUri, "NotificationService");
    }

    private static async Task WaitForHealthAsync(Uri baseUri, string serviceName)
    {
        using var client = CreateClient(baseUri);

        await WaitHelpers.UntilAsync(
            async () =>
            {
                try
                {
                    return await client.GetAsync("/health");
                }
                catch
                {
                    return null;
                }
            },
            response => response is { IsSuccessStatusCode: true },
            timeout: TimeSpan.FromSeconds(60),
            pollInterval: TimeSpan.FromSeconds(2));

        try
        {
            using var response = await client.GetAsync("/health");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"No se pudo confirmar el estado saludable de {serviceName}. " +
                "Asegurate de haber levantado la infraestructura con 'docker compose up -d --build'.",
                ex);
        }
    }

    private static Task<Respawner> CreateRespawnerAsync(NpgsqlConnection connection)
    {
        return Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore =
            [
                new Table("__EFMigrationsHistory")
            ]
        });
    }
}
