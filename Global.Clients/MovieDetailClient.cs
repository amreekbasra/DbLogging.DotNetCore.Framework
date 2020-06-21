using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Global.Clients
{
    public interface IMovieDetailsClient
    {
        Task<MovieDetailModel> GetMovieDetailsAsync(string movieName);
    }
    public class MovieDetailsClient : IMovieDetailsClient
    {
        private readonly HttpClient _httpClient;

        public MovieDetailsClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("http://www.omdbapi.com/");
            _httpClient = httpClient;
        }

        public async Task<MovieDetailModel> GetMovieDetailsAsync(string movieName)
        {
            var queryString = $"?t={movieName}&apikey=<your-api-key>";
            var response = await _httpClient.GetStringAsync(queryString);

            JObject json = JObject.Parse(response);

            if (json.SelectToken("Response").Value<string>() == "True")
            {

                var movieDetails = new MovieDetailModel
                {
                    Title = json.SelectToken("Title").Value<string>(),
                    Year = json.SelectToken("Year").Value<string>(),
                    Director = json.SelectToken("Director").Value<string>(),
                    Actors = json.SelectToken("Actors").Value<string>(),
                    IMDBRating = json.SelectToken("imdbRating").Value<string>(),
                    PosterImage = json.SelectToken("Poster").Value<string>(),
                    Plot = json.SelectToken("Plot").Value<string>()
                };

                return movieDetails;
            }

            return new MovieDetailModel
            {
                Title = movieName
            };
        }
    }
}
