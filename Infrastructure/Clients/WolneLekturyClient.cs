using System.Net.Http.Json;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Clients;

public class WolneLekturyClient : IWolneLekturyClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WolneLekturyClient> _logger;

    public WolneLekturyClient(
        HttpClient httpClient,
        ILogger<WolneLekturyClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ExternalBookDto>> GetBooksAsync(
        BooksQuery query,
        CancellationToken cancellationToken = default)
    {
        var endpoint = BuildBooksEndpoint(query);

        _logger.LogInformation("Fetching books from endpoint {Endpoint}", endpoint);

        var books = await _httpClient.GetFromJsonAsync<List<ExternalBookDto>>(
            endpoint,
            cancellationToken);

        return books ?? [];
    }

    public async Task<IReadOnlyCollection<AuthorDto>> GetAuthorsAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching authors from Wolne Lektury API.");

        var authors = await _httpClient.GetFromJsonAsync<List<AuthorDto>>(
            "authors/",
            cancellationToken);

        return authors ?? [];
    }

    private static string BuildBooksEndpoint(BooksQuery query)
    {
        var segments = new List<string>();

        if (!string.IsNullOrWhiteSpace(query.Author))
            segments.Add($"authors/{query.Author}");

        if (!string.IsNullOrWhiteSpace(query.Epoch))
            segments.Add($"epochs/{query.Epoch}");

        if (!string.IsNullOrWhiteSpace(query.Genre))
            segments.Add($"genres/{query.Genre}");

        if (!string.IsNullOrWhiteSpace(query.Kind))
            segments.Add($"kinds/{query.Kind}");

        segments.Add("books");

        return string.Join("/", segments) + "/";
    }
}