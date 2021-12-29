using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace OpenWeatherMapsClassLibrary
{
    enum RequestType : int
    {
        Place = 0,
        Coordinated = 1,
    }

    public class OpenWeatherMapsFacade
    {
        private OpenWeatherMaps openWeatherMaps;
        public OpenWeatherMapsFacade()
        {
            openWeatherMaps = new OpenWeatherMaps();
        }

        public void GetWeatherDataBy(string country, string city)
        {
            openWeatherMaps.GetWeatherDataBy(RequestType.Place, country + "," + city);
        }

        public void GetWeatherDataBy(decimal latitude, decimal longitude)
        {
            openWeatherMaps.GetWeatherDataBy(RequestType.Coordinated, latitude + "," + longitude);
        }
    }

    internal class OpenWeatherMaps
    {
        private readonly Infrastructure infrastructure;

        private readonly Dictionary<RequestType, string> urlParameters = new Dictionary<RequestType, string>();

        public OpenWeatherMaps()
        {
            infrastructure = new Infrastructure();
            urlParameters.Add(RequestType.Place, "q={0},{1}");
            urlParameters.Add(RequestType.Coordinated, "lat={0}&lon={1}");
        }

        public void GetWeatherDataBy(RequestType requestType, string location)
        {
            string parameters = urlParameters.Where(url => url.Key == requestType)
                             .Select(url => string.Format(url.Value, location.Split(',')[0], location.Split(',')[1]))
                             .FirstOrDefault();
            infrastructure.SendRequest(parameters);
        }
    }

    internal class Infrastructure
    {
        private readonly string url;
        private readonly string key;

        public Infrastructure()
        {
            key = new OpenWeatherMapsConfig().Key;
            url = new OpenWeatherMapsConfig().Url;
        }

        public void SendRequest(string parameters)
        {
            try
            {
                HttpWebRequest apiRequest = WebRequest.Create(url + parameters + key) as HttpWebRequest;
                using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    Console.WriteLine(reader.ReadToEnd());
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", ex.Message);
            }
        }
    }

    internal class OpenWeatherMapsConfig
    {
        private readonly string url;
        private readonly string key = "&appid={key}";
        public OpenWeatherMapsConfig()
        {
            key = key.Replace("{key}", new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("key")?.Value);
            url = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("url")?.Value;
        }

        public string Key
        {
            get
            {
                return key;
            }
        }

        public string Url
        {
            get
            {
                return url;
            }
        }
    }
}
