using Application.DTOs;

namespace Application.Interfaces;

public interface IWolneLekturyClient
{
    Task<IReadOnlyList<ExternalBookDto>> GetBooksAsync(
        BooksQuery query,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AuthorDto>> GetAuthorsAsync(CancellationToken cancellationToken = default);
}