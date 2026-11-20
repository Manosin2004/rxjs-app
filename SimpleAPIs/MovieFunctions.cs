using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace SimpleAPIs;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Genre { get; set; } = "";
    public int Year { get; set; }
    public decimal Rating { get; set; }
}

public class MovieFunctions
{
    private static readonly List<Movie> Movies = new()
    {
        new() { Id = 1,  Title = "The Shawshank Redemption", Genre = "Drama",    Year = 1994, Rating = 9.3m },
        new() { Id = 2,  Title = "The Godfather",            Genre = "Crime",    Year = 1972, Rating = 9.2m },
        new() { Id = 3,  Title = "Inception",                Genre = "Sci-Fi",   Year = 2010, Rating = 8.8m },
        new() { Id = 4,  Title = "Interstellar",             Genre = "Sci-Fi",   Year = 2014, Rating = 8.7m },
        new() { Id = 5,  Title = "Baahubali",                Genre = "Action",   Year = 2015, Rating = 8.0m },
        new() { Id = 6,  Title = "Visaranai",                Genre = "Crime",    Year = 2015, Rating = 8.4m },
        new() { Id = 7,  Title = "Super Deluxe",             Genre = "Drama",    Year = 2019, Rating = 8.4m },
        new() { Id = 8,  Title = "Parasite",                 Genre = "Thriller", Year = 2019, Rating = 8.5m },
        new() { Id = 9,  Title = "KGF",                      Genre = "Action",   Year = 2018, Rating = 8.2m },
        new() { Id = 10, Title = "Avengers Endgame",         Genre = "Action",   Year = 2019, Rating = 8.4m },
        new() { Id = 11, Title = "Jai Bhim",                 Genre = "Drama",    Year = 2021, Rating = 8.7m },
    };

    [Function("GetMovieRecommendations")]
    public IActionResult GetMovieRecommendations(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var genre = req.Query["genre"].ToString();
        var year = req.Query["year"].ToString();
        var rating = req.Query["rating"].ToString();

        IEnumerable<Movie> result = Movies;

        if (!string.IsNullOrWhiteSpace(genre))
            result = result.Where(m => m.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));

        if (int.TryParse(year, out var y))
            result = result.Where(m => m.Year == y);

        if (decimal.TryParse(rating, out var r))
            result = result.Where(m => m.Rating >= r);

        return new OkObjectResult(result.ToList());
    }

    [Function("SearchMovies")]
    public IActionResult SearchMovies(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var search = req.Query["search"].ToString();

        if (string.IsNullOrWhiteSpace(search))
            return new BadRequestObjectResult("Please provide a search term.");

        var result = Movies
            .Where(m => m.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return new OkObjectResult(result);
    }
}
