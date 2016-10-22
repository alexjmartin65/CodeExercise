using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeApplication
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
                    distances[city.CityName] = -1;
                }
            }

            foreach (var distance in distances.OrderByDescending(d => d.Value).ThenBy(d => d.Key))
            {
                var currentCity = Cities.First(city => city.CityName == distance.Key);
                shortestDistances.Add(($"{distance.Value} {currentCity.CityName}, {currentCity.State}"));
            }

            return shortestDistances;
        }

    }

    public class City
    {

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

    }

    public static class Constants
    {

        public const int POPULATION_INDEX = 0;

        public const int CITY_INDEX = 1;

        public const int STATE_INDEX = 2;

        public const int INTERSTATE_INDEX = 3;

        public const string STARTING_CITY_NAME = "CHICAGO";

        public const int INTERSTATE_EDGE_LENGTH = 1;

    }

}

//public class 
