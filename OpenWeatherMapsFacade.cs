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

    public class OpenWeatherMapsFacade : IOpenWeatherMapsFacade
    {
        private OpenWeatherMaps openWeatherMaps;
        public OpenWeatherMapsFacade()
        {
            openWeatherMaps = new OpenWeatherMaps();
        }

        public string GetWeatherDataBy(string country, string city)
        {
            return openWeatherMaps.GetWeatherDataBy(RequestType.Place, country + "," + city);
        }

        public string GetWeatherDataBy(decimal latitude, decimal longitude)
        {
            return openWeatherMaps.GetWeatherDataBy(RequestType.Coordinated, latitude + "," + longitude);
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

        public string GetWeatherDataBy(RequestType requestType, string location)
        {
            string parameters = urlParameters.Where(url => url.Key == requestType)
                             .Select(url => string.Format(url.Value, location.Split(',')[0], location.Split(',')[1]))
                             .FirstOrDefault();
            return infrastructure.SendRequest(parameters);
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

        public string SendRequest(string parameters)
        {
            try
            {
                HttpWebRequest apiRequest = WebRequest.Create(url + parameters + key) as HttpWebRequest;
                using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
               return ex.Message;
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
