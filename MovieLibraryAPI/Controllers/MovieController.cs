using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly MovieContext _context;

    public MovieController(MovieContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
    {
        return await _context.Movies.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Movie>> GetMovie(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null)
        {
            return NotFound("Movie with the given ID was not found.");
        }
        return movie;
    }

    [HttpPost]
    public async Task<IActionResult> AddMovie([FromBody] Movie movie)
    {
        try
        {
            if (_context.Movies.Any(existingMovie => existingMovie.Title == movie.Title && existingMovie.Director == movie.Director))
            {
                return BadRequest("A movie with the same data already exists.");
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred while adding the movie: {ex.Message}");
        }
    }

    [HttpPost("add-movies")]
    public async Task<IActionResult> AddMovies([FromBody] List<Movie> movies)
    {
        if (movies == null || movies.Count == 0)
        {
            return BadRequest("The movie list is empty or null.");
        }

        try
        {
            foreach (var movie in movies)
            {
                if (!_context.Movies.Any(m => m.Title == movie.Title && m.Year == movie.Year))
                {
                    _context.Movies.Add(movie);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(movies);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred while adding movies: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMovie(int id, [FromBody] Movie movie)
    {
        if (id != movie.Id)
        {
            return BadRequest("The movie ID in the URL does not match the ID in the request body.");
        }

        if (movie == null || movie.Year == 0)
        {
            return BadRequest("The movie data is empty or the year is invalid.");
        }

        var existingMovie = await _context.Movies
            .FirstOrDefaultAsync(m => m.Title == movie.Title && m.Director == movie.Director &&
                                      m.Year == movie.Year && m.Rate == movie.Rate && m.Id != movie.Id);

        if (existingMovie != null)
        {
            return Conflict("A movie with the same data already exists.");
        }

        var movieToUpdate = await _context.Movies.FindAsync(id);
        if (movieToUpdate is null)
        {
            return NotFound("Movie with the given ID was not found.");
        }

        movieToUpdate.Title = movie.Title;
        movieToUpdate.Director = movie.Director;
        movieToUpdate.Year = movie.Year;
        movieToUpdate.Rate = movie.Rate;

        await _context.SaveChangesAsync();
        return Ok(movieToUpdate);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie is null)
        {
            return NotFound("Movie with the given ID was not found.");
        }

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("fetch-external-movies")]
    public async Task<IActionResult> FetchExternalMovies()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://filmy.programdemo.pl/MyMovies");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Failed to fetch movies from the external API, StatusCode: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var externalMovies = JsonSerializer.Deserialize<List<Movie>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (externalMovies == null)
            {
                return BadRequest("No movies were fetched from the external API.");
            }

            var moviesToAdd = externalMovies.Select(m => new Movie
            {
                Title = m.Title,
                Director = m.Director,
                Year = m.Year,
                Rate = m.Rate
            }).ToList();

            var existingMovies = await _context.Movies.AsNoTracking().ToListAsync();
            var newMovies = moviesToAdd
                .Where(m => !existingMovies.Any(em => em.Title == m.Title && em.Director == m.Director && em.Year == m.Year))
                .ToList();

            if (newMovies.Count > 0)
            {
                _context.Movies.AddRange(newMovies);
                await _context.SaveChangesAsync();
            }

            return Ok(newMovies);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred while fetching movies: {ex.Message}");
        }
    }
}
