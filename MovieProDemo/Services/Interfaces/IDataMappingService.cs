using MovieProDemo.Models.Database;
using MovieProDemo.Models.TMDB;

namespace MovieProDemo.Services.Interfaces
{
    public interface IDataMappingService
    {
        Task<Movie> MapMovieDetailAsync(MovieDetail movie);
        ActorDetail MapActorDetail(ActorDetail actor);

    }
}
