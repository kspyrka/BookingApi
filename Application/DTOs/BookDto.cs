namespace Application.DTOs;

public class BookDto
{
    public string Id { get; set; } = "";

    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    public string Url { get; set; } = "";

    public string Thumbnail { get; set; } = "";

    public AuthorDto Author { get; set; } = new();
    public string Epoch { get; set; } = "";
    public string Genre { get; set; } = "";
    public string Kind { get; set; } = "";
}