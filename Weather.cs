using System;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace Weather;

public class WeatherClient
{
    private TaskCompletionSource<bool> _init = new TaskCompletionSource<bool>();
    JsonElement root;
    public string qualitativePrecipitation;
    public double currentTemperature;
    private int weatherCode;
    private float latitude;
    private float longitude;
    public Bitmap WeatherCodeToVisualData()
    {
        // Sets the visual data to match the WMO code received from the API.
        switch (weatherCode)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                qualitativePrecipitation = "Mostly clear";
                return new Bitmap("Emojis/sun_3d.png");
            case 45:
            case 48:
                qualitativePrecipitation = "Foggy";
                return new Bitmap("Emojis/fog_3d.png");
            case 51:
            case 53:
            case 55:
            case 56:
            case 57:
                qualitativePrecipitation = "Drizzly";
                return new Bitmap("Emojis/umbrella_with_rain_drops_3d.png");
            case 61:
            case 63:
            case 65:
            case 66:
            case 67:
            case 80:
            case 81:
            case 82:
                qualitativePrecipitation = "Rainy";
                return new Bitmap("Emojis/cloud_with_rain_3d.png");
            case 71:
            case 73:
            case 75:
            case 77:
            case 85:
            case 86:
                qualitativePrecipitation = "Snowy";
                return new Bitmap("Emojis/cloud_with_snow_3d.png");
            case 95:
            case 96:
            case 99:
                qualitativePrecipitation = "Thunderstorm";
                return new Bitmap("Emojis/cloud_with_lightning_and_rain_3d.png");
            default:
                // If for some reason the code doesn't have a WMO translation. we display a picture of our Lord Gaben.
                qualitativePrecipitation = "Could not parse weather code";
                return new Bitmap("gabe.jpg");
        }
    }
    public async Task WeatherClientInit() {
        await GetCoordinates();
        await GetWeatherData();
    }
    private async Task RunHttpRequest(string url) {
        // Create an http client from which to make requests
        var httpClient = new HttpClient();
        // Send the request
        HttpResponseMessage response = await httpClient.GetAsync(url);
        // Receive the content and parse it into the JsonElement variable
        HttpContent content = response.Content;
        using (var responseStream = await response.Content.ReadAsStreamAsync()) {
            JsonDocument json = await JsonDocument.ParseAsync(responseStream);
            root = json.RootElement;
        }
    }
    private async Task GetCoordinates() {
        await RunHttpRequest("http://ipinfo.io/json");
        string[] coordinates = root.GetProperty("loc").GetString().Split(',');
        latitude = StrToFloat(coordinates[0]);
        longitude = StrToFloat(coordinates[1]);
    }

    private async Task GetWeatherData() {
        await RunHttpRequest("https://api.open-meteo.com/v1/forecast?latitude="+latitude+"&longitude="+longitude+"&current=temperature_2m,weather_code");
        root = root.GetProperty("current");
        currentTemperature = root.GetProperty("temperature_2m").GetDouble();
        weatherCode = root.GetProperty("weather_code").GetInt32();
    }
    private float StrToFloat(string inputString) {
        // C# doesn't have a good built in method to convert strings to floats :(
        float outputFloat;
        if (string.IsNullOrEmpty(inputString)) {
            Console.WriteLine("Error! Unable to parse string into floating point. String is empty.");
            return -1;
        }
        if (!float.TryParse(inputString, out outputFloat)) {
            Console.WriteLine("Error! Unable to parse string into floating point. String is invalid.");
            return 1;
        }
        return outputFloat;
    }
}
