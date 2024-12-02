using AutoMapper;
using CityInfo.API.DbContexts;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {

        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        private readonly int _maxPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<List<CityWithoutPointOfInterestDTO>>> GetCities(string? name, string? searchQuery,
            int currentPage = 1, int pageSize = 10)
        {
            if (pageSize > _maxPageSize)
            {
                pageSize = _maxPageSize;
            }

            var (cities, pagination) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, currentPage, pageSize);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagination));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDTO>>(cities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityById(int id, bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

            if (city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDTO>(city));
            }

            return Ok(_mapper.Map<CityWithoutPointOfInterestDTO>(city));

        }

    }
}
