﻿namespace CityInfo.API.Models
{
    public class CityDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ICollection<PointOfInterestDTO> PointOfInterest { get; set; } = new List<PointOfInterestDTO>();

        public int NumberOfPointOfInterest
        {
            get
            {
                return PointOfInterest.Count;
            }
        }
    }
}
