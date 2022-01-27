using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MovieProDemo.Enums;
using MovieProDemo.Models.Settings;
using MovieProDemo.Models.TMDB;
using MovieProDemo.Services.Interfaces;
using System.Runtime.Serialization.Json;

namespace MovieProDemo.Services
{
    public class TmDbMovieService : IRemoteMovieService
    {
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public TmDbMovieService(IOptions<AppSettings> appSettings, IHttpClientFactory httpClientFactory)
        {
            _appSettings = appSettings.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ActorDetail> ActorDetailAsync(int id)
        {
            ActorDetail actorDetail = new();

            var query = $"{_appSettings.TmDbSettings.BaseUrl}/person/{id}";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", _appSettings.MovieProSettings.TmDbApiKey },
                { "language", _appSettings.TmDbSettings.QueryOptions.Language}
            };
            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();

                var dcjs = new DataContractJsonSerializer(typeof(ActorDetail));
                actorDetail = (ActorDetail)dcjs.ReadObject(responseStream);
            }

            return actorDetail;
        }

        public async Task<MovieDetail> MovieDetailAsync(int id)
        {
            MovieDetail movieDetail = new MovieDetail();
            var query = $"{_appSettings.TmDbSettings.BaseUrl}/movie/{id}";

            var queryParams = new Dictionary<string, string>()
            {
                {"api_key", _appSettings.MovieProSettings.TmDbApiKey },
                {"language", _appSettings.TmDbSettings.QueryOptions.Language},
                {"append_to_response", _appSettings.TmDbSettings.QueryOptions.AppendToResponse}
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var dcjs = new DataContractJsonSerializer(typeof(MovieSearch));
                using var responseStream = await response.Content.ReadAsStreamAsync();
                movieDetail = dcjs.ReadObject(responseStream) as MovieDetail;
                

            }


            return movieDetail;
        }

        public async Task<MovieSearch> SearchMovieAsync(MovieCategory category, int count)
        {
            MovieSearch movieSearch = new MovieSearch();
            var query = $"{_appSettings.TmDbSettings.BaseUrl}/movie/{category}";

            var queryParams = new Dictionary<string, string>()
            {
                {"api_key", _appSettings.MovieProSettings.TmDbApiKey },
                {"language", _appSettings.TmDbSettings.QueryOptions.Language},
                {"page", _appSettings.TmDbSettings.QueryOptions.Page}
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var dcjs = new DataContractJsonSerializer(typeof(MovieSearch));
                using var responseStream = await response.Content.ReadAsStreamAsync();
                movieSearch = (MovieSearch)dcjs.ReadObject(responseStream);
                movieSearch.results = movieSearch.results.Take(count).ToArray();
                movieSearch.results.ToList().ForEach(result => result.poster_path = $"{_appSettings.TmDbSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultPosterSize}/{result.poster_path}");


            }


            return movieSearch;
        }
    }
}
