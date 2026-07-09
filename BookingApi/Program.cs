using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.OData;
using Infrastructure.Clients;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddControllers()
    .AddOData(options =>
    {
        options
            .Filter()
            .OrderBy()
            .SetMaxTop(100)
            .SkipToken();
    });
builder.Services.AddMemoryCache();
builder.Services
    .AddScoped<IBookService, BookService>()
    .AddScoped<IAuthorService, AuthorService>()
    .AddHttpClient<IWolneLekturyClient, WolneLekturyClient>(client =>
{
    client.BaseAddress = new Uri("https://wolnelektury.pl/api/");
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.Run();