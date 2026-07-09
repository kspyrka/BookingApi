using Application.DTOs;

namespace Application.Interfaces;

public interface IAuthorService
{
    Task<IReadOnlyCollection<AuthorDto>> GetAuthorsAsync(CancellationToken cancellationToken = default);

    Task<AuthorDto?> GetAuthorByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}