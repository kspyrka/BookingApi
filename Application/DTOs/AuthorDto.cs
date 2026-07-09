namespace Application.DTOs;

public class AuthorDto
{
    public string Slug { private get; init; } = default!;

    public string Id => Slug;

    public string Name { get; init; } = default!;
}