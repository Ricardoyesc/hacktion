using hacktion.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RestSharp;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace hacktion.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class TripController : ControllerBase
    {
        private readonly ITripRepository _tripRepository;
        public TripController(ITripRepository tripRepository)
        {
            _tripRepository = tripRepository;
        }

        [HttpGet(Name = "get")]
        public Trip? get(string name)
        {
            return _tripRepository.Get(name);
        }
        [HttpPost(Name = "post")]
        public Trip? post([FromBody] Trip trip)
        {
            return _tripRepository.Save(trip);
        }
        [HttpPost(Name = "daily-route")]
        public async Task<RouteTrip?> DayRouteAsync([FromBody] List<Place> route_sites, [FromQuery] string name)
        {
            var trip = _tripRepository.Get(name);
            if (route_sites.Count() < 2)
            {
                return null;
            }
            var response = new RouteTrip();

            for(int i = 0; i < route_sites.Count-1; i++)
            {
                response.Segments.Add(new Segment
                {
                    price = await CalculatePriceAsync(route_sites[i], route_sites[i+1]),
                    from = route_sites[i],
                    to = route_sites[i+1]
                });
            }
            return response;
        }

        private async Task<decimal> CalculatePriceAsync(Place place1, Place place2)
        {
            const string API_KEY = "AIzaSyDJwMaNCoswKvqec13SdGcMNt57puJAgic";
            const string base_url = "https://routes.googleapis.com/directions/v2:computeRoutes";

            // Crear el cliente RestSharp
            var client = new RestClient(base_url);

            // Crear la solicitud
            var request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-Goog-Api-Key", API_KEY);
            request.AddHeader("X-Goog-FieldMask", "routes.localizedValues.transitFare");

            // Datos para la solicitud en formato JSON
            var data = new
            {
                origin = new { address = place1.address },
                destination = new { address = place2.address },
                travelMode = "TRANSIT",
                computeAlternativeRoutes = false
            };

            request.AddJsonBody(data);  // Agregar el cuerpo JSON directamente

            // Ejecutar la solicitud POST de forma asíncrona
            var response = await client.ExecutePostAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var directions_result = JsonConvert.DeserializeObject<RootObject>(response.Content);
                var res = 0.0m;
                var text = directions_result?.Routes[0].LocalizedValues.TransitFare.Text;
                var match = Regex.Match(text, @"(\d+\.?\d*)");
                if (match.Success)
                {
                    decimal.TryParse(match.Value, out res);

                }
                return res;
            }
            else
            {
                Console.WriteLine("Error fetching directions: " + (int)response.StatusCode + ", " + response.Content); // Mostrar el contenido del error
            }
            return 0.0m;
        }

        public decimal GetAmount(string text)
        {
            var match = Regex.Match(text, @"(\d+\.?\d*)"); // Extraer solo los números

            if (match.Success && decimal.TryParse(match.Value, out decimal amount))
            {
                return amount;
            }
            else
            {
                throw new FormatException("Formato de tarifa inválido");
            }
        }

    }
}
