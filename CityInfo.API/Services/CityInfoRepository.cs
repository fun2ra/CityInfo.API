using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }


        public async Task<(IEnumerable<City>, PaginationData)> GetCitiesAsync(string? name, string? searchQuery, int currentPage, int pageSize)
        {
            var citiesCollection = _context.Cities as IQueryable<City>;
            if (!string.IsNullOrWhiteSpace(name))
            {
                var nameTrimed = name.Trim().ToLower();
                citiesCollection = citiesCollection.Where(c => c.Name.ToLower() == nameTrimed);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchQueryTrimed = searchQuery.Trim().ToLower();
                citiesCollection = citiesCollection.Where(c => c.Name.ToLower().Contains(searchQueryTrimed)
                || c.Description != null && c.Description.ToLower().Contains(searchQueryTrimed));
            }

            var totalCities = await citiesCollection.CountAsync();

            PaginationData pagination = new PaginationData(currentPage, pageSize, totalCities);

            var colletionToReturn = await citiesCollection.OrderBy(c => c.Name)
                .Skip(pageSize * (currentPage - 1))
                .Take(pageSize)
                .ToListAsync();

            return (colletionToReturn, pagination);
        }

        public async Task<bool> PointOfInterestExistsAsync(int cityId, int pointId)
        {
            var city = await GetCityAsync(cityId, true);

            if (city == null)
            {
                return false;
            }

            return city.PointOfInterest.Any(p => p.Id == pointId);
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await _context.Cities.Where(c => c.Id == cityId).Include(c => c.PointOfInterest).FirstOrDefaultAsync();
            }
            else
            {
                return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId && p.Id == pointId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointOfInterests.Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task CreatePointOfInterestAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);

            if (city != null)
            {
                city.PointOfInterest.Add(pointOfInterest);
            }
        }
        public async Task UpdatePointOfInterestAsync(int cityId, int pointId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            var point = city?.PointOfInterest.FirstOrDefault(p => p.Id == pointId);

            if (city != null && point != null)
            {
                point.Name = pointOfInterest.Name;
                point.Description = pointOfInterest.Description;
            }
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void DeletePointOfInterestAsync(PointOfInterest pointOfInterest)
        {
            if (pointOfInterest != null)
            {
                _context.PointOfInterests.Remove(pointOfInterest);
            }
        }

 
    }
}
