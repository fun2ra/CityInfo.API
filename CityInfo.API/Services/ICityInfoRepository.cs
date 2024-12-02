using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<bool> CityExistsAsync(int cityId);
        Task<bool> PointOfInterestExistsAsync(int cityId, int pointId);
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>, PaginationData)> GetCitiesAsync(string? name, string? searchQuery, int currentPage, int pageSize);
        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointId);
        Task CreatePointOfInterestAsync(int cityId, PointOfInterest pointOfInterest);
        void DeletePointOfInterestAsync(PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();
    }
}
