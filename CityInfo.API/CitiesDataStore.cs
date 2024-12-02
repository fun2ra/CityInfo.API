using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public List<CityDTO> Cities { get; set; }

        //public static CitiesDataStore Current { get; set; } = new CitiesDataStore();
        public CitiesDataStore()
        {
            Cities = new List<CityDTO>
            {
                new CityDTO()
                {
                    Id = 1,
                    Name = "Lisbon",
                    Description = "Capital of Portugal",
                    PointOfInterest = new List<PointOfInterestDTO>()
                    {
                        new PointOfInterestDTO()
                        {
                            Id=1,
                            Name= "Torre de Belen",
                            Description= "That tower near to the river"
                        },
                        new PointOfInterestDTO()
                        {
                            Id=2,
                            Name= "Castelo de Sao Joa",
                            Description= "The big castle inside the city"
                        }
                    }
                },
                new CityDTO()
                {
                    Id = 2,
                    Name = "Madrid",
                    Description = "Capital of Spain",
                    PointOfInterest = new List<PointOfInterestDTO>()
                    {
                        new PointOfInterestDTO()
                        {
                            Id=1,
                            Name= "Museo del Prado",
                            Description= "The important museum"
                        },
                        new PointOfInterestDTO()
                        {
                            Id=2,
                            Name= "Puerta de Alcala",
                            Description= "Ahi esta, Ahi esta"
                        }
                    }

                },
                new CityDTO()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "Capital of France",
                                        PointOfInterest = new List<PointOfInterestDTO>()
                    {
                        new PointOfInterestDTO()
                        {
                            Id=1,
                            Name= "Torre Eiffel",
                            Description= "THE TOWER"
                        },
                        new PointOfInterestDTO()
                        {
                            Id=2,
                            Name= "Arco del Triunfo",
                            Description= "This one famous, the one that Napoleon did"
                        }
                    }
                },
                new CityDTO()
                {
                    Id = 4,
                    Name = "Berlin",
                    Description = "Capital of Germany",

                },
            };
        }
    }
}
