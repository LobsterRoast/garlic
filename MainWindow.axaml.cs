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
    WeatherClient weather = new WeatherClient();
    public ReactiveCommand<Unit, Unit> C2F { get; }
    public ReactiveCommand<Unit, Unit> SwitchTimeFormat { get; }
    bool militaryTime = false;
    bool fahrenheit = false;
    public MainWindow()
    {
        C2F = ReactiveCommand.Create(() => {
            fahrenheit = !fahrenheit;
            SetTemp();
        });
        SwitchTimeFormat = ReactiveCommand.Create(() => {
            militaryTime = !militaryTime;
            ClockTick();
        });
        DataContext = this;
        DispatcherTimer timer = new DispatcherTimer() {
            Interval = TimeSpan.FromSeconds(1)
        };
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
    private double CtoF(double d) {
        return d * (9d/5d) + 32d;
    }
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
        foreach (SyndicationItem item in firstFourItems)
        {
            textBlocks[i].Text = item.Title.Text;
            Console.WriteLine(item.Links.FirstOrDefault()?.Uri.ToString());
            i++;
        }
    }
}
