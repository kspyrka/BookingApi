namespace Domain.Entities;

public class Book
{
    public string Id { get; init; }

    public string Title { get; init; }

    public string Description { get; init; }

    public string Url { get; init; }

    public string Thumbnail { get; init; }

    public IReadOnlyCollection<Author> Authors { get; init; }

    public string? Kind { get; init; }

    public string? Genre { get; init; }

    public string? Epoch { get; init; }
}