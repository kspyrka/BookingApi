using Application.DTOs;

namespace Application.Interfaces;

public interface IBookService
{
    Task<IReadOnlyCollection<BookDto>> GetBooksAsync(BooksQuery booksQuery,
        CancellationToken cancellationToken = default);

    Task<BookDto?> GetBookAsync(string slug, CancellationToken cancellationToken = default);
}