using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Solution
{
    public class Solution
    {
        static void Main(string[] args)
        {
            var cityString = Console.ReadLine();

            var Cities = new List<City>();
            while (string.IsNullOrWhiteSpace(cityString) == false)
            {
                if (!cityString.StartsWith("#"))
                {
                    Cities.Add(new City(cityString));
                }
                cityString = Console.ReadLine();
            }

            var graph = new Graph(Cities);
            var shortestDistances = graph.GetShortestDistances();

            foreach (var distanceString in shortestDistances)
            {
                Console.WriteLine(distanceString);
            }

            Console.ReadLine();
        }
    }

    public class Graph
    {

        protected List<City> Cities { get; set; }

        public Graph(List<City> cities)
        {
            Cities = cities;
        }

        public List<string> GetShortestDistances()
        {

            var shortestDistances = new List<String>();
            var distances = new Dictionary<string, int>();

            var startingCity = Cities.First(c => c.IsStartingCity);

            var queue = new Queue<City>();
            queue.Enqueue(startingCity);

            //Distance to starting city is always 0
            distances[startingCity.CityName] = 0;

            //Using Breadth First Search
            while (queue.Count != 0)
            {
                var currentCity = queue.Dequeue();
                foreach (var interestate in currentCity.Interstates)
                {
                    var adjacentCities = Cities.Where(c => c.Interstates.Contains(interestate));
                    foreach (var adjacentCity in adjacentCities)
                    {
                        if (distances.ContainsKey(adjacentCity.CityName) == false)
                        {
                            distances[adjacentCity.CityName] = distances[currentCity.CityName] + Constants.INTERSTATE_EDGE_LENGTH;
                            queue.Enqueue(adjacentCity);
                        }
                        else
                        {
                            distances[adjacentCity.CityName] = Math.Min(distances[adjacentCity.CityName], distances[currentCity.CityName] + Constants.INTERSTATE_EDGE_LENGTH);
                        }
                    }
                }
            }

            //If a city doesn't have an adjacency then set the value to -1
            foreach (var city in Cities)
            {
                if (distances.ContainsKey(city.CityName) == false)
                {
                    distances[city.CityName] = Constants.ORPHANED_DISTANCE;
                }
            }

            foreach (var distance in distances.OrderByDescending(d => d.Value).ThenBy(d => d.Key))
            {
                var currentCity = Cities.First(city => city.CityName == distance.Key);
                shortestDistances.Add(currentCity.GetCityDistanceString(distance.Value));
            }

            return shortestDistances;
        }

    }

    public class City
    {
        public City()
        {

        }

        public City(string cityString)
        {
            var citySplit = cityString.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Population = int.Parse(citySplit[Constants.POPULATION_INDEX]);
            CityName = citySplit[Constants.CITY_INDEX];
            State = citySplit[Constants.STATE_INDEX];
            Interstates = citySplit[Constants.INTERSTATE_INDEX].Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public int Population { get; set; }

        public string CityName { get; set; }

        public string State { get; set; }

        public bool IsStartingCity
        {
            get
            {
                return CityName.ToUpper() == Constants.STARTING_CITY_NAME;
            }
        }

        public List<string> Interstates { get; set; }

        public string GetCityDistanceString(int distance)
        {
            return $"{distance} {CityName}, {State}";
        }

    }

    public static class Constants
    {

        public const int POPULATION_INDEX = 0;

        public const int CITY_INDEX = 1;

        public const int STATE_INDEX = 2;

        public const int INTERSTATE_INDEX = 3;

        public const string STARTING_CITY_NAME = "CHICAGO";

        public const int INTERSTATE_EDGE_LENGTH = 1;

        public const int ORPHANED_DISTANCE = -1;

    }

    [TestClass]
    public class GraphTest
    {

        [TestMethod]
        public void StraightLineGraph()
        {
            var cities = new List<City>();
            cities.Add(new City()
            {
                CityName = Constants.STARTING_CITY_NAME,
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "1"
                }
            });

            cities.Add(new City()
            {
                CityName = "First",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "1",
                    "2"
                }
            });

            cities.Add(new City()
            {
                CityName = "Second",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "2",
                    "3"
                }
            });

            cities.Add(new City()
            {
                CityName = "Third",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "3",
                    "4"
                }
            });

            var graph = new Graph(cities);
            var distances = graph.GetShortestDistances();

            Assert.AreEqual(cities[3].GetCityDistanceString(3), distances[0]);
            Assert.AreEqual(cities[2].GetCityDistanceString(2), distances[1]);
            Assert.AreEqual(cities[1].GetCityDistanceString(1), distances[2]);
            Assert.AreEqual(cities[0].GetCityDistanceString(0), distances[3]);
        }

        [TestMethod]
        public void GraphWithOrphans()
        {
            var cities = new List<City>();
            cities.Add(new City()
            {
                CityName = Constants.STARTING_CITY_NAME,
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "1"
                }
            });

            cities.Add(new City()
            {
                CityName = "First",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "1",
                    "2"
                }
            });

            cities.Add(new City()
            {
                CityName = "Second",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "2",
                    "3"
                }
            });

            cities.Add(new City()
            {
                CityName = "Third",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "5"
                }
            });

            cities.Add(new City()
            {
                CityName = "Fourth",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "6"
                }
            });

            var graph = new Graph(cities);
            var distances = graph.GetShortestDistances();

            
            Assert.AreEqual(cities[2].GetCityDistanceString(2), distances[0]);
            Assert.AreEqual(cities[1].GetCityDistanceString(1), distances[1]);
            Assert.AreEqual(cities[0].GetCityDistanceString(0), distances[2]);
            Assert.AreEqual(cities[4].GetCityDistanceString(Constants.ORPHANED_DISTANCE), distances[3]);
            Assert.AreEqual(cities[3].GetCityDistanceString(Constants.ORPHANED_DISTANCE), distances[4]);
        }

        [TestMethod]
        public void MultipleConnectionsGraph()
        {
            var cities = new List<City>();
            cities.Add(new City()
            {
                CityName = Constants.STARTING_CITY_NAME,
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "1",
                    "2",
                    "10"
                }
            });

            cities.Add(new City()
            {
                CityName = "First",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "1",
                    "3",
                    "4"
                }
            });

            cities.Add(new City()
            {
                CityName = "Second",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "2",
                    "3"
                }
            });

            cities.Add(new City()
            {
                CityName = "Third",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "4",
                    "6"
                }
            });

            cities.Add(new City()
            {
                CityName = "Fourth",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "7"
                }
            });

            cities.Add(new City()
            {
                CityName = "Fifth",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "6",
                    "7",
                    "8"
                }
            });

            cities.Add(new City()
            {
                CityName = "Sixth",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "8",
                    "9"
                }
            });

            cities.Add(new City()
            {
                CityName = "Seventh",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "9",
                    "10",
                }
            });


            var graph = new Graph(cities);
            var distances = graph.GetShortestDistances();

            Assert.AreEqual(cities[4].GetCityDistanceString(4), distances[0]);
            Assert.AreEqual(cities[5].GetCityDistanceString(3), distances[1]);
            Assert.AreEqual(cities[6].GetCityDistanceString(2), distances[2]);
            Assert.AreEqual(cities[3].GetCityDistanceString(2), distances[3]);
            Assert.AreEqual(cities[1].GetCityDistanceString(1), distances[4]);
            Assert.AreEqual(cities[2].GetCityDistanceString(1), distances[5]);
            Assert.AreEqual(cities[7].GetCityDistanceString(1), distances[6]);
            Assert.AreEqual(cities[0].GetCityDistanceString(0), distances[7]);
        }

        [TestMethod]
        public void MultipleConnectionsGraphWithOrphans()
        {
            var cities = new List<City>();
            cities.Add(new City()
            {
                CityName = Constants.STARTING_CITY_NAME,
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "1",
                    "2",
                    "10"
                }
            });

            cities.Add(new City()
            {
                CityName = "First",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "1",
                    "3",
                    "4"
                }
            });

            cities.Add(new City()
            {
                CityName = "Second",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "2",
                    "3"
                }
            });

            cities.Add(new City()
            {
                CityName = "Third",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "4",
                    "6"
                }
            });

            cities.Add(new City()
            {
                CityName = "Fourth",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "7"
                }
            });

            cities.Add(new City()
            {
                CityName = "Fifth",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "6",
                    "7",
                    "8"
                }
            });

            cities.Add(new City()
            {
                CityName = "Sixth",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "8",
                    "9"
                }
            });

            cities.Add(new City()
            {
                CityName = "Seventh",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "9",
                    "10",
                }
            });

            cities.Add(new City()
            {
                CityName = "Eight",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "12"
                }
            });

            cities.Add(new City()
            {
                CityName = "Nine",
                State = "IL",
                Population = 10,
                Interstates = new List<string>()
                {
                    "12"
                }
            });


            var graph = new Graph(cities);
            var distances = graph.GetShortestDistances();

            Assert.AreEqual(cities[4].GetCityDistanceString(4), distances[0]);
            Assert.AreEqual(cities[5].GetCityDistanceString(3), distances[1]);
            Assert.AreEqual(cities[6].GetCityDistanceString(2), distances[2]);
            Assert.AreEqual(cities[3].GetCityDistanceString(2), distances[3]);
            Assert.AreEqual(cities[1].GetCityDistanceString(1), distances[4]);
            Assert.AreEqual(cities[2].GetCityDistanceString(1), distances[5]);
            Assert.AreEqual(cities[7].GetCityDistanceString(1), distances[6]);
            Assert.AreEqual(cities[0].GetCityDistanceString(0), distances[7]);
            Assert.AreEqual(cities[8].GetCityDistanceString(Constants.ORPHANED_DISTANCE), distances[8]);
            Assert.AreEqual(cities[9].GetCityDistanceString(Constants.ORPHANED_DISTANCE), distances[9]);
        }

    }

}


