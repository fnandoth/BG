using Bogus;
using PostService.Domain.Entities;
using PostService.Domain.ValueObjects;
using PostService.IntegrationTests.Models;
using PostEntity = PostService.Domain.Entities.Post;
using UserEntity = UserService.Domain.Entities.User;

namespace PostService.IntegrationTests.Helpers;

public static class BogusDataFactory
{
    private static readonly Faker Faker = new("es");
    private static int _userSequence;
    private static int _postSequence;

    public static (UserEntity Entity, TestUser Model) CreateUser()
    {
        var sequence = Interlocked.Increment(ref _userSequence);
        var slug = Faker.Internet.UserName().Replace(".", string.Empty).Replace("-", string.Empty).ToLowerInvariant();
        var userName = $"bg_user_{sequence}_{slug}";
        var displayName = $"bgdisplay{sequence}";
        var email = $"bg_user_{sequence}@example.test";
        var password = $"BgTests!{sequence}Aa";
        var avatarUrl = $"https://example.test/avatar/{sequence}.png";
        var now = DateTime.UtcNow;

        var entity = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Email = email,
            DisplayName = displayName,
            Bio = Faker.Lorem.Sentence(8),
            AvatarUrl = avatarUrl,
            IsPrivate = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        var model = new TestUser(
            entity.Id,
            entity.UserName,
            password,
            entity.DisplayName,
            entity.Email,
            entity.AvatarUrl);

        return (entity, model);
    }

    public static PostEntity CreatePost(
        TestUser author,
        PostType postType = PostType.Post,
        string? content = null,
        Guid? parentPostId = null,
        Guid? quotedPostId = null,
        Guid? originalPostId = null,
        DateTimeOffset? createdAt = null)
    {
        var sequence = Interlocked.Increment(ref _postSequence);

        return new PostEntity
        {
            Id = Guid.NewGuid(),
            AuthorId = author.Id,
            AuthorSnapshot = new AuthorSnapshot(
                author.Id,
                author.UserName,
                author.DisplayName,
                author.AvatarUrl,
                false),
            Content = content ?? Faker.Lorem.Sentence(12) + $" #{sequence}",
            MediaUrls = null,
            PostType = postType,
            ParentPostId = parentPostId,
            QuotedPostId = quotedPostId,
            OriginalPostId = originalPostId,
            LikesCount = 0,
            RepliesCount = 0,
            RepostsCount = 0,
            QuotesCount = 0,
            IsDeleted = false,
            CreatedAt = createdAt ?? DateTimeOffset.UtcNow
        };
    }
}
