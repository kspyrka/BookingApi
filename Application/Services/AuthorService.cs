using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

public class AuthorService : IAuthorService
{
    private const string AuthorsCacheKey = "wolne-lektury-authors";

    private readonly IWolneLekturyClient _client;
    private readonly IMemoryCache _cache;

    public AuthorService(
        IWolneLekturyClient client,
        IMemoryCache cache)
    {
        _client = client;
        _cache = cache;
    }


    public async Task<IReadOnlyCollection<AuthorDto>> GetAuthorsAsync(
        CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(
                AuthorsCacheKey,
                out IReadOnlyCollection<AuthorDto>? authors))
        {
            return authors!;
        }

        authors =
            await _client.GetAuthorsAsync(cancellationToken);


        _cache.Set(
            AuthorsCacheKey,
            authors,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromHours(24),

                SlidingExpiration =
                    TimeSpan.FromHours(2)
            });


        return authors;
    }

    public async Task<AuthorDto?> GetAuthorByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        var authors = await GetAuthorsAsync(cancellationToken);

        return authors.FirstOrDefault(x =>
            x.Name.Equals(
                name,
                StringComparison.OrdinalIgnoreCase));
    }
}