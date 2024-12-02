using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using CityInfo.API.Entities;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/[controller]")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        private readonly CitiesDataStore _citiesDataStore;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper, CitiesDataStore citiesDataStore)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }

        [HttpGet]
        public async Task<ActionResult<List<PointOfInterestDTO>>> GetPointsOfInterest(int cityId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id: {cityId} do not found");
                return NotFound();
            }
            var points = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDTO>>(points));
        }

        [HttpGet("{pointId}", Name = "GetPointOfInterestById")]
        public async Task<ActionResult<PointOfInterestDTO>> GetPointOfInterestById(int cityId, int pointId)
        {
            if (!await _cityInfoRepository.PointOfInterestExistsAsync(cityId, pointId))
            {
                _logger.LogInformation($"City with id: {cityId} do not found");
                return NotFound();
            }
            var point = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointId);

            return Ok(_mapper.Map<PointOfInterestDTO>(point));

        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDTO>> CreatePointOfInterest(int cityId, PointOfInterestForCreationDTO pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var createdPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.CreatePointOfInterestAsync(cityId, createdPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDTO>(createdPointOfInterest);


            return CreatedAtRoute("GetPointOfInterestById", new { cityId, pointId = createdPointOfInterestToReturn.Id }, createdPointOfInterestToReturn);
        }

        [HttpPut("{pointId}")]
        public async Task<ActionResult<PointOfInterestDTO>> UpdatePointOfInterest(int cityId, int pointId, PointOfInterestForUpdateDTO pointOfInterest)
        {
            if (!await _cityInfoRepository.PointOfInterestExistsAsync(cityId, pointId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointId);

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            //var updatePointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);
            //_mapper.Map(updatePointOfInterest, pointOfInterestEntity);
            //await _cityInfoRepository.UpdatePointOfInterestAsync(cityId, pointId, updatePointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();


            return NoContent();

        }

        [HttpPatch("{pointId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointId, JsonPatchDocument<PointOfInterestForUpdateDTO> patchDocument)
        {
            if (!await _cityInfoRepository.PointOfInterestExistsAsync(cityId, pointId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointId);

            var pointToPatch = _mapper.Map<PointOfInterestForUpdateDTO>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointToPatch, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{pointId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointId)
        {
            if (!await _cityInfoRepository.PointOfInterestExistsAsync(cityId, pointId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointId);


            _cityInfoRepository.DeletePointOfInterestAsync(pointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            string subject = $"Point Of Interest form {pointOfInterest.City.Name} was deleted";
            string message = $"The Point of Interest with Id: {pointId} of the city {pointOfInterest.City.Name}, was recently deleted by the user.";
            _mailService.SendMail(subject, message);


            return NoContent();
        }

    }
}
