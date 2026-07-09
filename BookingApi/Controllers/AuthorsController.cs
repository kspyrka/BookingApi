using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;
    private readonly IBookService _bookService;

    public AuthorsController(IAuthorService authorService, IBookService bookService)
    {
        _authorService = authorService;
        _bookService = bookService;
    }

    [HttpGet]
    [EnableQuery(
        AllowedQueryOptions =
            AllowedQueryOptions.OrderBy |
            AllowedQueryOptions.Skip |
            AllowedQueryOptions.Top)]
    public async Task<IActionResult> GetAuthors(
        CancellationToken cancellationToken)
    {
        var authors = await _authorService.GetAuthorsAsync(cancellationToken);

        return Ok(authors.AsQueryable());
    }

    [HttpGet("{slug}/books")]
    [EnableQuery(
        AllowedQueryOptions =
            AllowedQueryOptions.OrderBy |
            AllowedQueryOptions.Skip |
            AllowedQueryOptions.Top)]
    public async Task<IActionResult> GetBooksByAuthor(
        string slug,
        CancellationToken cancellationToken)
    {
        var books = await _bookService.GetBooksAsync(
            new BooksQuery
            {
                Author = slug
            },
            cancellationToken);

        return Ok(books.AsQueryable());
    }
}