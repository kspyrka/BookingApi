using Application.DTOs;
using Application.Interfaces;
using BookingApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookService> _serviceMock;
    private readonly BooksController _sut;


    public BooksControllerTests()
    {
        _serviceMock = new Mock<IBookService>();

        _sut = new BooksController(
            _serviceMock.Object);
    }


    [Fact]
    public async Task GetBooks_ShouldReturnOk_WhenBooksExist()
    {
        // Arrange
        var books = new List<BookDto>
        {
            new()
            {
                Id = "pan-tadeusz",
                Title = "Pan Tadeusz"
            }
        };

        _serviceMock
            .Setup(x => x.GetBooksAsync(
                It.IsAny<BooksQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);

        // Act
        var result =
            await _sut.GetBooks(
                new BooksQuery(),
                CancellationToken.None);

        // Assert
        var okResult =
            result.Should()
                .BeOfType<OkObjectResult>()
                .Subject;

        okResult.Value
            .Should()
            .BeAssignableTo<IEnumerable<BookDto>>();
    }


    [Fact]
    public async Task GetBook_ShouldReturnOk_WhenBookExists()
    {
        // Arrange
        _serviceMock
            .Setup(x => x.GetBookAsync(
                "pan-tadeusz",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new BookDto
                {
                    Id = "pan-tadeusz",
                    Title = "Pan Tadeusz"
                });

        // Act
        var result =
            await _sut.GetBook(
                "pan-tadeusz");

        // Assert
        var ok =
            result.Result.Should()
                .BeOfType<OkObjectResult>()
                .Subject;

        var book =
            ok.Value.Should()
                .BeOfType<BookDto>()
                .Subject;

        book.Id.Should()
            .Be("pan-tadeusz");
    }

    [Fact]
    public async Task GetBook_ShouldReturnNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        _serviceMock
            .Setup(x => x.GetBookAsync(
                "unknown",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (BookDto?)null);

        // Act
        var result =
            await _sut.GetBook(
                "unknown");

        // Assert
        result.Result.Should()
            .BeOfType<NotFoundResult>();
    }
}