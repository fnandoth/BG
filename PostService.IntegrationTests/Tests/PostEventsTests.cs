using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PostService.Aplication.DTOs;
using PostService.Domain.Entities;
using PostService.IntegrationTests.Fixtures;
using PostService.IntegrationTests.Helpers;

namespace PostService.IntegrationTests.Tests;

public sealed class PostEventsTests(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task CreateReplyAsync_ShouldPublishNotificationVisibleInNotificationService()
    {
        var author = await Fixture.SeedUserAsync();
        var replier = await Fixture.SeedUserAsync();
        var parentPost = await Fixture.SeedPostAsync(author, content: "post que recibira la respuesta");
        var accessToken = await Fixture.GetAccessTokenAsync(replier);

        using var client = Fixture.CreateAuthorizedPostClient(accessToken);

        var request = new CreatePostRequest
        {
            Content = "respuesta que deberia generar notificacion",
            PostType = PostType.Reply,
            ParentPostId = parentPost.Id
        };

        using var response = await client.PostAsJsonAsync("/api/posts", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var notifications = await WaitHelpers.UntilAsync(
            action: () => Fixture.GetNotificationsAsync(author, onlyUnread: true),
            predicate: page => page.Items.Any(notification =>
                notification.Type == "reply" &&
                notification.Actor.UserId == replier.Id &&
                notification.Post?.PostId == parentPost.Id),
            timeout: TimeSpan.FromSeconds(20),
            pollInterval: TimeSpan.FromMilliseconds(500));

        notifications.Items.Should().Contain(notification =>
            notification.Type == "reply" &&
            notification.Actor.UserId == replier.Id &&
            notification.Post != null &&
            notification.Post.PostId == parentPost.Id);
    }

    [Fact]
    public async Task RepostAsync_ShouldPublishNotificationVisibleInNotificationService()
    {
        var author = await Fixture.SeedUserAsync();
        var reposter = await Fixture.SeedUserAsync();
        var original = await Fixture.SeedPostAsync(author, content: "post que recibira el repost");
        var accessToken = await Fixture.GetAccessTokenAsync(reposter);

        using var client = Fixture.CreateAuthorizedPostClient(accessToken);
        using var response = await client.PostAsync($"/api/posts/{original.Id}/repost", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var notifications = await WaitHelpers.UntilAsync(
            action: () => Fixture.GetNotificationsAsync(author, onlyUnread: true),
            predicate: page => page.Items.Any(notification =>
                notification.Type == "repost" &&
                notification.Actor.UserId == reposter.Id &&
                notification.Post?.PostId == original.Id),
            timeout: TimeSpan.FromSeconds(20),
            pollInterval: TimeSpan.FromMilliseconds(500));

        notifications.Items.Should().Contain(notification =>
            notification.Type == "repost" &&
            notification.Actor.UserId == reposter.Id &&
            notification.Post != null &&
            notification.Post.PostId == original.Id);
    }
}
