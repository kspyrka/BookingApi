using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Integration;

public class ODataAndSingleBooksTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ODataAndSingleBooksTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;

        _client =
            factory.CreateClient();
    }

    [Fact]
    public async Task GetBooks_ShouldSupportTop()
    {
        // Arrange
        _factory.BookServiceMock
            .Setup(x => x.GetBooksAsync(
                It.IsAny<BooksQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new BookDto
                {
                    Id = "1",
                    Title = "Pan Tadeusz"
                },

                new BookDto
                {
                    Id = "2",
                    Title = "Dziady"
                }
            ]);

        // Act
        var response =
            await _client.GetAsync(
                "/api/Books?$top=1");

        // Assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var result =
            await response.Content
                .ReadFromJsonAsync<List<BookDto>>();

        result.Should()
            .HaveCount(1);
    }

    [Fact]
    public async Task GetBooks_ShouldOrderByTitleDescending()
    {
        // Arrange
        _factory.BookServiceMock
            .Setup(x => x.GetBooksAsync(
                It.IsAny<BooksQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new BookDto
                {
                    Id = "1",
                    Title = "Ala"
                },

                new BookDto
                {
                    Id = "2",
                    Title = "Zenon"
                }
            ]);

        // Act
        var response =
            await _client.GetAsync(
                "/api/Books?$orderby=Title desc");
        
        // Assert
        var result =
            await response.Content
                .ReadFromJsonAsync<List<BookDto>>();
        
        result![0].Title
            .Should()
            .Be("Zenon");
    }


    [Fact]
    public async Task GetBooks_ShouldSupportPagination()
    {
        // Arrange
        _factory.BookServiceMock
            .Setup(x => x.GetBooksAsync(
                It.IsAny<BooksQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new BookDto { Id = "1", Title = "A" },
                new BookDto { Id = "2", Title = "B" },
                new BookDto { Id = "3", Title = "C" }
            ]);

        // Act
        var response =
            await _client.GetAsync(
                "/api/Books?$skip=1&$top=1");

        // Assert
        var result =
            await response.Content
                .ReadFromJsonAsync<List<BookDto>>();

        result.Should()
            .ContainSingle();

        result![0].Title
            .Should()
            .Be("B");
    }

    [Fact]
    public async Task GetBookBySlug_ShouldReturnBook_WhenBookExists()
    {
        // Arrange
        _factory.BookServiceMock
            .Setup(x => x.GetBookAsync(
                "pan-tadeusz",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new BookDto
                {
                    Id = "pan-tadeusz",
                    Title = "Pan Tadeusz",
                    Url = "https://wolnelektury.pl/katalog/lektura/pan-tadeusz/"
                });

        // Act
        var response =
            await _client.GetAsync(
                "/api/Books/pan-tadeusz");
        
        // Assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
        
        var result =
            await response.Content
                .ReadFromJsonAsync<BookDto>();
        
        result.Should()
            .NotBeNull();

        result!.Id
            .Should()
            .Be("pan-tadeusz");
        
        result.Title
            .Should()
            .Be("Pan Tadeusz");
    }

    [Fact]
    public async Task GetBookBySlug_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        _factory.BookServiceMock
            .Setup(x => x.GetBookAsync(
                "unknown-book",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (BookDto?)null);

        // Act
        var response =
            await _client.GetAsync(
                "/api/Books/unknown-book");
        
        // Assert
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.NotFound);
    }
}