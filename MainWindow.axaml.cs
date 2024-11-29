using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.ReactiveUI;
using System;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Linq;
using Weather;
using System.Threading.Tasks;
using System.Reactive;
using ReactiveUI;

namespace garlic;

public partial class MainWindow : Window
{
    // Initialize the weather client (see Weather.cs)
    WeatherClient weather = new WeatherClient();
    // Declare reactive commands to execute on button presses
    public ReactiveCommand<Unit, Unit> C2F { get; }
    public ReactiveCommand<Unit, Unit> SwitchTimeFormat { get; }
    bool militaryTime = false;
    bool fahrenheit = false;
    public MainWindow()
    {   
        // Switch units of temperature
        C2F = ReactiveCommand.Create(() => {
            fahrenheit = !fahrenheit;
            SetTemp();
        });
        // Switch between 12-hr and 24-hr clock
        SwitchTimeFormat = ReactiveCommand.Create(() => {
            militaryTime = !militaryTime;
            ClockTick();
        });
        DataContext = this;
        DispatcherTimer timer = new DispatcherTimer() {
            Interval = TimeSpan.FromSeconds(1)
        };
        // Execute ClockTick() on each tick of the timer
        timer.Tick += ClockTick;
        timer.Start();
        InitializeComponent();
        C2FButton.Text = "Switch to °F";
        SwitchTimeButton.Text = "Switch to 24-hour clock";
        Dispatcher.UIThread.Post(async () => 
        {
            await MainWindowAsync();
        });
    }
    private void ClockTick(object sender, EventArgs e) {
        if (militaryTime) {
            Clock.Text = DateTime.Now.ToString("HH:mm:ss");
            SwitchTimeButton.Text = "Switch to 12-hour clock";
        }
        else {
            DateTime now = DateTime.Now;
            Clock.Text = now.ToString("hh:mm:ss");
            int hour = now.Hour;
            if (hour >= 12)
                Clock.Text += " PM";
            else
                Clock.Text += " AM";
            SwitchTimeButton.Text = "Switch to 24-hour clock";
        }
    }
    private void ClockTick() {
        // If using 24-hr clock, display the time in 24 hr format
        if (militaryTime) {
            Clock.Text = DateTime.Now.ToString("HH:mm:ss");
            SwitchTimeButton.Text = "Switch to 12-hour clock";
        }
        else {
        // If using 12-hr time, display the time and append either AM or PM to it
            DateTime now = DateTime.Now;
            Clock.Text = now.ToString("hh:mm:ss");
            int hour = now.Hour;
            if (hour >= 12)
                Clock.Text += " PM";
            else
                Clock.Text += " AM";
            SwitchTimeButton.Text = "Switch to 24-hour clock";
        }
    }
    // Convert between Celsius and Fahrenheit
    private double CtoF(double d) {
        return d * (9d/5d) + 32d;
    }
    // Set the TextBlock for temperature to display the temperature
    private void SetTemp() {
        if (fahrenheit) {
            Temperature.Text = Math.Round(CtoF(weather.currentTemperature)).ToString() + "°F";
            C2FButton.Text = "Switch to °C";
        }
        else {
            Temperature.Text = Math.Round(weather.currentTemperature).ToString() + "°C";
            C2FButton.Text = "Switch to °F";
        }
    }
    async Task MainWindowAsync() {
        await weather.WeatherClientInit();
        SetTemp();
        weatherImg.Source = weather.WeatherCodeToVisualData();
        // Reads The Onion's RSS feed
        XmlReader reader = XmlReader.Create("https://theonion.com/feed");
        SyndicationFeed feed = SyndicationFeed.Load(reader);
        reader.Close();
        var firstFourItems = feed.Items.Take(4);
        int i = 0;
        TextBlock[] textBlocks = {
            Headline1,
            Headline2,
            Headline3,
            Headline4
        };
        // Sets each Headline TextBlock to display a news headline from The Onion
        foreach (SyndicationItem item in firstFourItems)
        {
            textBlocks[i].Text = item.Title.Text;
            Console.WriteLine(item.Links.FirstOrDefault()?.Uri.ToString());
            i++;
        }
    }
}
