using hacktion.Repository.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Win32;

namespace hacktion.Repository
{
    public class TripRepository : ITripRepository
    {
        private readonly IMemoryCache _cache;
        public TripRepository(IMemoryCache cache)
        {
            _cache = cache;
        }
        public Trip? Get(string name)
        {
            return _cache.Get<Trip>(name);
        }

        public Trip? Save(Trip trip)
        {
            return _cache.Set(trip.name, trip, TimeSpan.FromDays(1));
        }
    }
}
