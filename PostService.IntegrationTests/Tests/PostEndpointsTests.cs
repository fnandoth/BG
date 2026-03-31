using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PostService.Aplication.DTOs;
using PostService.Domain.Entities;
using PostService.IntegrationTests.Fixtures;

namespace PostService.IntegrationTests.Tests;

public sealed class PostEndpointsTests(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task CreateAsync_ShouldCreateANewPost()
    {
        var author = await Fixture.SeedUserAsync();
        var accessToken = await Fixture.GetAccessTokenAsync(author);

        using var client = Fixture.CreateAuthorizedPostClient(accessToken);

        var request = new CreatePostRequest
        {
            Content = "post creado desde la suite de integracion",
            PostType = PostType.Post,
            MediaUrls = ["https://example.test/media/1.png"]
        };

        using var response = await client.PostAsJsonAsync("/api/posts", request);
        var created = await response.ReadRequiredJsonAsync<PostDetailDto>(Fixture.JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        created.Content.Should().Be(request.Content);
        created.PostType.Should().Be(PostType.Post);
        created.Author.UserId.Should().Be(author.Id);
        created.Author.Username.Should().Be(author.UserName);
        created.Author.DisplayName.Should().Be(author.DisplayName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteOwnPost()
    {
        var author = await Fixture.SeedUserAsync();
        var post = await Fixture.SeedPostAsync(author, content: "post para eliminar");
        var accessToken = await Fixture.GetAccessTokenAsync(author);

        using var authorizedClient = Fixture.CreateAuthorizedPostClient(accessToken);
        using var deleteResponse = await authorizedClient.DeleteAsync($"/api/posts/{post.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var anonymousClient = Fixture.CreatePostClient();
        using var getResponse = await anonymousClient.GetAsync($"/api/posts/{post.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPostDetailAsync_ShouldReturnPostWithNestedReplies()
    {
        var author = await Fixture.SeedUserAsync();
        var replier = await Fixture.SeedUserAsync();
        var post = await Fixture.SeedPostAsync(author, content: "post principal");
        var firstReply = await Fixture.SeedReplyAsync(
            replier,
            post.Id,
            content: "primera respuesta",
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-2));
        var secondReply = await Fixture.SeedReplyAsync(
            author,
            post.Id,
            content: "segunda respuesta",
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-1));

        using var client = Fixture.CreatePostClient();
        using var response = await client.GetAsync($"/api/posts/{post.Id}");
        var detail = await response.ReadRequiredJsonAsync<PostDetailDto>(Fixture.JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        detail.Id.Should().Be(post.Id);
        detail.Replies.Should().HaveCount(2);
        detail.Replies.Select(reply => reply.Id).Should().Contain([firstReply.Id, secondReply.Id]);
    }

    [Fact]
    public async Task GetRepliesAsync_ShouldReturnRepliesOrderedByCreatedAtDescending()
    {
        var author = await Fixture.SeedUserAsync();
        var replier = await Fixture.SeedUserAsync();
        var post = await Fixture.SeedPostAsync(author);

        var oldest = await Fixture.SeedReplyAsync(
            replier,
            post.Id,
            content: "reply vieja",
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-3));
        var middle = await Fixture.SeedReplyAsync(
            replier,
            post.Id,
            content: "reply media",
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-2));
        var newest = await Fixture.SeedReplyAsync(
            replier,
            post.Id,
            content: "reply nueva",
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-1));

        using var client = Fixture.CreatePostClient();
        using var response = await client.GetAsync($"/api/posts/{post.Id}/replies?page=1&pageSize=2");
        var replies = await response.ReadRequiredJsonAsync<List<PostDetailDto>>(Fixture.JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        replies.Should().HaveCount(2);
        replies.Select(reply => reply.Id).Should().Equal(newest.Id, middle.Id);
        replies.Select(reply => reply.Id).Should().NotContain(oldest.Id);
    }

    [Fact]
    public async Task GetQuotesAsync_ShouldReturnQuotesOrderedByCreatedAtDescending()
    {
        var author = await Fixture.SeedUserAsync();
        var quoter = await Fixture.SeedUserAsync();
        var post = await Fixture.SeedPostAsync(author);

        var oldest = await Fixture.SeedQuoteAsync(
            quoter,
            post.Id,
            content: "quote vieja",
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-3));
        var newest = await Fixture.SeedQuoteAsync(
            quoter,
            post.Id,
            content: "quote nueva",
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-1));

        using var client = Fixture.CreatePostClient();
        using var response = await client.GetAsync($"/api/posts/{post.Id}/quotes?page=1&pageSize=10");
        var quotes = await response.ReadRequiredJsonAsync<List<PostSummaryDto>>(Fixture.JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        quotes.Select(quote => quote.Id).Should().Equal(newest.Id, oldest.Id);
    }

    [Fact]
    public async Task GetTimelineAsync_ShouldReturnAllPostsFromRequestedUser()
    {
        var author = await Fixture.SeedUserAsync();
        var otherUser = await Fixture.SeedUserAsync();
        var baseTime = DateTimeOffset.UtcNow;

        var post = await Fixture.SeedPostAsync(author, content: "post timeline", createdAt: baseTime.AddMinutes(-3));
        var reply = await Fixture.SeedReplyAsync(author, post.Id, content: "reply timeline", createdAt: baseTime.AddMinutes(-2));
        var originalFromOther = await Fixture.SeedPostAsync(otherUser, content: "origen repost", createdAt: baseTime.AddMinutes(-4));
        var repost = await Fixture.SeedRepostAsync(author, originalFromOther.Id, createdAt: baseTime.AddMinutes(-1));
        await Fixture.SeedPostAsync(otherUser, content: "post ajeno", createdAt: baseTime);

        var accessToken = await Fixture.GetAccessTokenAsync(author);
        using var client = Fixture.CreateAuthorizedPostClient(accessToken);
        using var response = await client.GetAsync($"/api/posts/timeline/{author.Id}?page=1&pageSize=10");
        var timeline = await response.ReadRequiredJsonAsync<List<TimelinePostDto>>(Fixture.JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        timeline.Select(item => item.Id).Should().Equal(repost.Id, reply.Id, post.Id);
        timeline.Should().OnlyContain(item => item.Author.UserId == author.Id);
    }

    [Fact]
    public async Task GetUserPostsAsync_ShouldExcludeReplies()
    {
        var author = await Fixture.SeedUserAsync();
        var otherUser = await Fixture.SeedUserAsync();
        var baseTime = DateTimeOffset.UtcNow;

        var post = await Fixture.SeedPostAsync(author, content: "post visible", createdAt: baseTime.AddMinutes(-2));
        var quoteTarget = await Fixture.SeedPostAsync(otherUser, content: "post citado", createdAt: baseTime.AddMinutes(-4));
        var quote = await Fixture.SeedQuoteAsync(author, quoteTarget.Id, content: "quote visible", createdAt: baseTime.AddMinutes(-1));
        var reply = await Fixture.SeedReplyAsync(author, post.Id, content: "reply oculta", createdAt: baseTime);

        using var client = Fixture.CreatePostClient();
        using var response = await client.GetAsync($"/api/posts/user/{author.Id}?page=1&pageSize=10");
        var posts = await response.ReadRequiredJsonAsync<List<TimelinePostDto>>(Fixture.JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        posts.Select(item => item.Id).Should().Equal(quote.Id, post.Id);
        posts.Select(item => item.Id).Should().NotContain(reply.Id);
    }

    [Fact]
    public async Task RepostAsync_ShouldCreateRepostAndIncrementOriginalCounter()
    {
        var author = await Fixture.SeedUserAsync();
        var reposter = await Fixture.SeedUserAsync();
        var original = await Fixture.SeedPostAsync(author, content: "post original para repost");
        var accessToken = await Fixture.GetAccessTokenAsync(reposter);

        using var client = Fixture.CreateAuthorizedPostClient(accessToken);
        using var repostResponse = await client.PostAsync($"/api/posts/{original.Id}/repost", content: null);
        var repost = await repostResponse.ReadRequiredJsonAsync<TimelinePostDto>(Fixture.JsonOptions);

        repostResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        repost.PostType.Should().Be(PostType.Repost);
        repost.Author.UserId.Should().Be(reposter.Id);
        repost.RepostedPost.Should().NotBeNull();
        repost.RepostedPost!.Id.Should().Be(original.Id);

        using var anonymousClient = Fixture.CreatePostClient();
        using var detailResponse = await anonymousClient.GetAsync($"/api/posts/{original.Id}");
        var detail = await detailResponse.ReadRequiredJsonAsync<PostDetailDto>(Fixture.JsonOptions);

        detail.RepostsCount.Should().Be(1);
    }

    [Fact]
    public async Task QuotePostAsync_ShouldCreateQuoteAndIncrementOriginalCounter()
    {
        var author = await Fixture.SeedUserAsync();
        var quoter = await Fixture.SeedUserAsync();
        var original = await Fixture.SeedPostAsync(author, content: "post original para quote");
        var accessToken = await Fixture.GetAccessTokenAsync(quoter);

        using var client = Fixture.CreateAuthorizedPostClient(accessToken);

        var request = new CreatePostRequest
        {
            Content = "comentario de la cita",
            PostType = PostType.Post
        };

        using var quoteResponse = await client.PostAsJsonAsync($"/api/posts/{original.Id}/quote", request);
        var quote = await quoteResponse.ReadRequiredJsonAsync<PostDetailDto>(Fixture.JsonOptions);

        quoteResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        quote.PostType.Should().Be(PostType.Quote);
        quote.Content.Should().Be(request.Content);
        quote.Author.UserId.Should().Be(quoter.Id);

        using var anonymousClient = Fixture.CreatePostClient();
        using var detailResponse = await anonymousClient.GetAsync($"/api/posts/{quote.Id}");
        var detail = await detailResponse.ReadRequiredJsonAsync<PostDetailDto>(Fixture.JsonOptions);

        detail.QuotedPost.Should().NotBeNull();
        detail.QuotedPost!.Id.Should().Be(original.Id);

        using var originalResponse = await anonymousClient.GetAsync($"/api/posts/{original.Id}");
        var originalDetail = await originalResponse.ReadRequiredJsonAsync<PostDetailDto>(Fixture.JsonOptions);

        originalDetail.QuotesCount.Should().Be(1);
    }
}
