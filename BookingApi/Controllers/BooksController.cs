using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace BookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    [EnableQuery(
        AllowedQueryOptions =
            AllowedQueryOptions.OrderBy |
            AllowedQueryOptions.Skip |
            AllowedQueryOptions.Top)]
    public async Task<IActionResult> GetBooks(
        [FromQuery] BooksQuery query,
        CancellationToken cancellationToken)
    {
        var books = await _bookService.GetBooksAsync(query, cancellationToken);

        return Ok(books.AsQueryable());
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<BookDto>> GetBook(string slug)
    {
        var book = await _bookService.GetBookAsync(slug);

        if (book is null)
            return NotFound();

        return Ok(book);
    }
}