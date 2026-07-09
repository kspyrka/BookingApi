using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IWolneLekturyClient> _clientMock;
    private readonly Mock<IAuthorService> _authorServiceMock;
    private readonly IMemoryCache _cache;
    private readonly BookService _sut;

    public BookServiceTests()
    {
        _clientMock = new Mock<IWolneLekturyClient>();
        _authorServiceMock = new Mock<IAuthorService>();
        _cache = new MemoryCache(
            new MemoryCacheOptions());

        _sut = new BookService(
            _clientMock.Object,
            _cache,
            _authorServiceMock.Object);
    }

    [Fact]
    public async Task GetBooksAsync_ShouldReturnMappedBooks_WhenCacheIsEmpty()
    {
        // Arrange
        var externalBooks = new List<ExternalBookDto>
        {
            new()
            {
                Slug = "pan-tadeusz",
                Title = "Pan Tadeusz",
                Url = "https://wolnelektury.pl/pan-tadeusz/",
                Simple_thumb = "thumb.jpg",
                Author = "Adam Mickiewicz"
            }
        };

        _clientMock
            .Setup(x => x.GetBooksAsync(
                It.IsAny<BooksQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(externalBooks);

        _authorServiceMock
            .Setup(x => x.GetAuthorByNameAsync(
                "Adam Mickiewicz",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new AuthorDto
                {
                    Slug = "adam-mickiewicz",
                    Name = "Adam Mickiewicz"
                });

        // Act
        var result =
            await _sut.GetBooksAsync(
                new BooksQuery());

        // Assert
        result.Should()
            .HaveCount(1);

        var book = result.First();

        book.Id.Should()
            .Be("pan-tadeusz");

        book.Title.Should()
            .Be("Pan Tadeusz");

        book.Author.Id.Should()
            .Be("adam-mickiewicz");

        _clientMock.Verify(
            x => x.GetBooksAsync(
                It.IsAny<BooksQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetBooksAsync_ShouldUseCache_OnSecondCall()
    {
        // Arrange
        _clientMock
            .Setup(x => x.GetBooksAsync(
                It.IsAny<BooksQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new ExternalBookDto
                {
                    Slug = "dziady",
                    Title = "Dziady",
                    Author = "Adam Mickiewicz"
                }
            ]);

        _authorServiceMock
            .Setup(x => x.GetAuthorByNameAsync(
                "Adam Mickiewicz",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new AuthorDto
                {
                    Slug = "adam-mickiewicz",
                    Name = "Adam Mickiewicz"
                });

        var query = new BooksQuery();


        // Act
        await _sut.GetBooksAsync(query);
        await _sut.GetBooksAsync(query);

        // Assert

        _clientMock.Verify(
            x => x.GetBooksAsync(
                It.IsAny<BooksQuery>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task GetBookAsync_ShouldReturnBook_WhenBookExists()
    {
        // Arrange
        _clientMock
            .Setup(x => x.GetBooksAsync(
                It.IsAny<BooksQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new ExternalBookDto
                {
                    Slug = "pan-tadeusz",
                    Title = "Pan Tadeusz",
                    Author = "Adam Mickiewicz"
                }
            ]);

        _authorServiceMock
            .Setup(x => x.GetAuthorByNameAsync(
                "Adam Mickiewicz",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new AuthorDto
                {
                    Slug = "adam-mickiewicz",
                    Name = "Adam Mickiewicz"
                });

        // Act
        var result =
            await _sut.GetBookAsync(
                "pan-tadeusz");

        // Assert
        result.Should()
            .NotBeNull();

        result!.Id.Should()
            .Be("pan-tadeusz");

        result.Title.Should()
            .Be("Pan Tadeusz");

        result.Author.Id.Should()
            .Be("adam-mickiewicz");
    }


    [Fact]
    public async Task GetBookAsync_ShouldReturnNull_WhenBookDoesNotExist()
    {
        // Arrange

        _clientMock
            .Setup(x => x.GetBooksAsync(
                It.IsAny<BooksQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new ExternalBookDto
                {
                    Slug = "dziady",
                    Title = "Dziady",
                    Author = "Adam Mickiewicz"
                }
            ]);

        // Act
        var result =
            await _sut.GetBookAsync(
                "unknown");

        // Assert
        result.Should()
            .BeNull();
    }
}