namespace OpenWeatherMapsClassLibrary
{
    interface IOpenWeatherMapsFacade
    {
         void GetWeatherDataBy(string country, string city);

         void GetWeatherDataBy(decimal latitude, decimal longitude);
    }
}
