using System.Text;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services;

public class BookService : IBookService
{
    private readonly IWolneLekturyClient _client;
    private readonly IAuthorService _authorService;
    private readonly IMemoryCache _cache;

    public BookService(
        IWolneLekturyClient client,
        IMemoryCache cache, IAuthorService authorService)
    {
        _client = client;
        _cache = cache;
        _authorService = authorService;
    }

    public async Task<IReadOnlyCollection<BookDto>> GetBooksAsync(
        BooksQuery query,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(query);

        if (_cache.TryGetValue(
                cacheKey,
                out IReadOnlyCollection<BookDto>? cachedBooks))
        {
            return cachedBooks!;
        }


        var externalBooks =
            await _client.GetBooksAsync(
                query,
                cancellationToken);


        var result = new List<BookDto>();


        foreach (var book in externalBooks)
        {
            result.Add(
                await MapToDto(
                    book,
                    cancellationToken));
        }


        _cache.Set(
            cacheKey,
            result,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(30),

                SlidingExpiration =
                    TimeSpan.FromMinutes(10)
            });


        return result;
    }

    public async Task<BookDto?> GetBookAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        var books = await GetBooksAsync(new BooksQuery(), cancellationToken);
        
        var book =
            books.FirstOrDefault(x =>
                x.Id.Equals(
                    slug,
                    StringComparison.OrdinalIgnoreCase));
        
        return book ?? null;
    }

    private static string BuildCacheKey(BooksQuery query)
    {
        return string.Join(':',
            "books",
            query.Author,
            query.Kind,
            query.Genre,
            query.Epoch);
    }

    private async Task<BookDto> MapToDto(
        ExternalBookDto? book,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(book);

        var author =
            await _authorService.GetAuthorByNameAsync(
                book.Author ?? "",
                cancellationToken);


        return new BookDto
        {
            Id = book.Slug ?? "",

            Title = book.Title ?? "",

            Url = book.Url ?? "",

            Thumbnail = book.Simple_thumb ?? "",
            Epoch = book.Epoch ?? "",
            Genre = book.Genre ?? "",
            Kind = book.Kind ?? "",

            Author =
                author ?? new AuthorDto
                {
                    Slug = CreateSlug(book.Author),
                    Name = book.Author ?? ""
                }
        };
    }

    private static string CreateSlug(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var builder = new StringBuilder();

        foreach (var character in normalized)
        {
            var unicodeCategory =
                System.Globalization.CharUnicodeInfo.GetUnicodeCategory(character);

            if (unicodeCategory == System.Globalization.UnicodeCategory.NonSpacingMark) continue;
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
            else if (char.IsWhiteSpace(character) || character == '-')
            {
                builder.Append('-');
            }
        }

        return builder
            .ToString()
            .Trim('-');
    }
}