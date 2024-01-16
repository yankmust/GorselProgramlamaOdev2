using GorselProgramlamaOdev2.ModelsWeather;
using GorselProgramlamaOdev2.ServicesWeather;

namespace GorselProgramlamaOdev2;

public partial class Weather : ContentPage
{
    public List<ModelsWeather.List> WeatherList;
    private double latitude;
    private double longitude;
	public Weather()
	{
		InitializeComponent();
        WeatherList = new List<ModelsWeather.List>();
	}
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetLocation();
        await GetWeatherDataByLocation(latitude, longitude);


    }

    public async Task GetLocation()
    {
      var location = await  Geolocation.GetLocationAsync();
       latitude =  location.Latitude;
        longitude = location.Longitude;

    }

    private async void TapLocation_Tapped(object sender, TappedEventArgs e)
    {
        await GetLocation();
        await GetWeatherDataByLocation(latitude, longitude);
    }
    public async Task GetWeatherDataByLocation(double latidude, double longitude)
    {
        var result = await ApiService.GetWeather(latitude, longitude);
         UpdateUI(result);

        
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
      var response = await DisplayPromptAsync(title: "", message: "", placeholder: "Search weather by City",accept:"Search",cancel:"Cancel");
        if (response != null)
        {
            await GetWeatherDataByCity(response);
        }
    }

    public async Task GetWeatherDataByCity(string city)
    {
        var result = await ApiService.GetWeatherByCity(city);
        UpdateUI(result);

        
    }

    public void UpdateUI(dynamic result)
    {
        foreach (var item in result.list)
        {
            WeatherList.Add(item);
        }
        CvWeather.ItemsSource = WeatherList;
        LblCity.Text = result.city.name;
        LblWeatherDescription.Text = result.list[0].weather[0].description;
        LblTemperature.Text = result.list[0].main.temp + "°C";
        LblHumidity.Text = result.list[0].main.humidity + "%";
        LblWind.Text = result.list[0].wind.speed + "km/h";
        ImgWeatherIcon.Source = result.list[0].weather[0].customIcon;
    }

}