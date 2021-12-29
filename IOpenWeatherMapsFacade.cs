namespace OpenWeatherMapsClassLibrary
{
    interface IOpenWeatherMapsFacade
    {
        string GetWeatherDataBy(string country, string city);

        string GetWeatherDataBy(decimal latitude, decimal longitude);
    }
}
