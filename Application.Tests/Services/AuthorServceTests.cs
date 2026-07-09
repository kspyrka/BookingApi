using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace Tests.Services;

public class AuthorServiceTests
{
    private readonly Mock<IWolneLekturyClient> _clientMock;
    private readonly IMemoryCache _cache;
    private readonly AuthorService _sut;


    public AuthorServiceTests()
    {
        _clientMock = new Mock<IWolneLekturyClient>();
        _cache = new MemoryCache(
            new MemoryCacheOptions());

        _sut = new AuthorService(
            _clientMock.Object,
            _cache);
    }

    [Fact]
    public async Task GetAuthorsAsync_ShouldReturnAuthors_WhenCacheIsEmpty()
    {
        // Arrange
        var authors = new List<AuthorDto>
        {
            new()
            {
                Slug = "adam-mickiewicz",
                Name = "Adam Mickiewicz"
            },

            new()
            {
                Slug = "juliusz-slowacki",
                Name = "Juliusz Słowacki"
            }
        };

        _clientMock
            .Setup(x => x.GetAuthorsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        // Act
        var result =
            await _sut.GetAuthorsAsync(CancellationToken.None);

        // Assert
        result.Should()
            .HaveCount(2);

        result.Should()
            .Contain(x =>
                x.Id == "adam-mickiewicz" &&
                x.Name == "Adam Mickiewicz");

        _clientMock.Verify(
            x => x.GetAuthorsAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAuthorsAsync_ShouldUseCache_OnSecondCall()
    {
        // Arrange
        _clientMock
            .Setup(x => x.GetAuthorsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new AuthorDto
                {
                    Slug = "adam-mickiewicz",
                    Name = "Adam Mickiewicz"
                }
            ]);

        // Act
        await _sut.GetAuthorsAsync(CancellationToken.None);
        await _sut.GetAuthorsAsync(CancellationToken.None);

        // Assert
        _clientMock.Verify(
            x => x.GetAuthorsAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task GetAuthorByNameAsync_ShouldReturnAuthor_WhenAuthorExists()
    {
        // Arrange
        _clientMock
            .Setup(x => x.GetAuthorsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new AuthorDto
                {
                    Slug = "adam-mickiewicz",
                    Name = "Adam Mickiewicz"
                }
            ]);

        // Act
        var result =
            await _sut.GetAuthorByNameAsync(
                "Adam Mickiewicz");

        // Assert
        result.Should()
            .NotBeNull();

        result!.Id.Should()
            .Be("adam-mickiewicz");

        result.Name.Should()
            .Be("Adam Mickiewicz");
    }

    [Fact]
    public async Task GetAuthorByNameAsync_ShouldBeCaseInsensitive()
    {
        // Arrange
        _clientMock
            .Setup(x => x.GetAuthorsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new AuthorDto
                {
                    Slug = "adam-mickiewicz",
                    Name = "Adam Mickiewicz"
                }
            ]);

        // Act
        var result =
            await _sut.GetAuthorByNameAsync(
                "adam mickiewicz");

        // Assert
        result.Should()
            .NotBeNull();

        result!.Id.Should()
            .Be("adam-mickiewicz");
    }


    [Fact]
    public async Task GetAuthorByNameAsync_ShouldReturnNull_WhenAuthorDoesNotExist()
    {
        // Arrange
        _clientMock
            .Setup(x => x.GetAuthorsAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new AuthorDto
                {
                    Slug = "adam-mickiewicz",
                    Name = "Adam Mickiewicz"
                }
            ]);

        // Act
        var result =
            await _sut.GetAuthorByNameAsync(
                "Henryk Sienkiewicz");

        // Assert
        result.Should()
            .BeNull();
    }
}