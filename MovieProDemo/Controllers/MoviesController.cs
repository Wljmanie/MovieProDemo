using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieProDemo.Data;
using MovieProDemo.Models.Database;
using MovieProDemo.Models.Settings;
using MovieProDemo.Services.Interfaces;

namespace MovieProDemo.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly IRemoteMovieService _remoteMovieService;
        private readonly IDataMappingService _dataMappingService;

        public MoviesController(IOptions<AppSettings> appSettings, ApplicationDbContext context, IImageService imageService, IRemoteMovieService remoteMovieService, IDataMappingService dataMappingService)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _imageService = imageService;
            _remoteMovieService = remoteMovieService;
            _dataMappingService = dataMappingService;
        }

        [HttpGet]
        public async Task<IActionResult> Import()
        {
            var movies = await _context.Movie.ToListAsync();

            return View(movies);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(int id)
        {
            if(_context.Movie.Any(m => m.MovieId == id))
            {
                var localMovie = await _context.Movie.FirstOrDefaultAsync(m => m.MovieId == id);
                return RedirectToAction("Details", "Movies", new { id = localMovie.Id, local = true });

            }

            var movieDetail = await _remoteMovieService.MovieDetailAsync(id);
            var movie = await _dataMappingService.MapMovieDetailAsync(movieDetail);
            _context.Add(movie);
            await _context.SaveChangesAsync();

            await AddToMovieCollection(movie.Id, _appSettings.MovieProSettings.DefaultCollection.Name);

            return RedirectToAction("Import");
        }

        private async Task AddToMovieCollection(int movieId, string collectionName)
        {
            var collection = await _context.Collection.FirstOrDefaultAsync(c => c.Name == collectionName);
            _context.Add(
                new MovieCollection()
                {
                    MovieId = movieId,
                    CollectionId = collection.Id
                });
            await _context.SaveChangesAsync();
        }

        private async Task AddToMovieCollection(int movieId, int collectionId)
        {
            _context.Add(
                new MovieCollection()
                {
                    MovieId = movieId,
                    CollectionId = collectionId
                });
            await _context.SaveChangesAsync();
        }
    }
}
