using IOTDisplay.HelperClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IOTDisplay
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static DispatcherTimer clockTimer;
        private static DispatcherTimer weatherTimer;
        private static GpioPin pin;

        public MainPage()
        {
            this.InitializeComponent();
            SetClockTimer();
            SetWeatherTimer();
            GetWeatherData();
            GpioController gpio = GpioController.GetDefault();
            if (gpio == null)
                return; // GPIO not available on this system


           pin = gpio.OpenPin(22);
        }

        /// <summary>
        /// Creates a time that runs the clock and date for the UI.
        /// </summary>
        private void SetClockTimer()
        {
            // Create a timer with a one second interval.
            clockTimer = new DispatcherTimer();
            // Hook up the Elapsed event for the timer. 
            clockTimer.Tick += new EventHandler<object>(dispatcherTimer_Tick);
            clockTimer.Interval = new TimeSpan(0, 0, 1);
            clockTimer.Start();
        }

        private void SetWeatherTimer()
        {
            weatherTimer = new DispatcherTimer();
            weatherTimer.Tick += new EventHandler<object>(weatherTimer_Tick);
            weatherTimer.Interval = new TimeSpan(0, 30, 0);
            weatherTimer.Start();
        }

        private void weatherTimer_Tick(object sender, object e)
        {
            GetWeatherData();
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            DateText.Text = DateTime.Now.Date.ToString("ddd, MMM dd");
            ClockText.Text = DateTime.Now.ToString("h:mm:ss tt");
        }

        private async void GetWeatherData()
        {
            //Weather Forcast from Weather Underground.com
            Uri uri = new Uri("http://api.wunderground.com/api/60582b1f2d11a6ea/forecast10day/q/IA/Iowa_City.json");
            using (HttpClient downloader = new HttpClient())
            {
                var response = await downloader.GetStringAsync(uri);
                
                    try
                    {
                        Rootobject root = JsonConvert.DeserializeObject<Rootobject>(response);
                        try
                        {
                            var weatherData = root.forecast.simpleforecast;
                            SetWeatherData(weatherData);
                        }
                        catch { }
                    }
                    catch { }
                
            }
        }

        private void SetWeatherData(Simpleforecast weatherData)
        {
            if (weatherData != null)
            {
                int i = 0;
                string hostIconURL = "ms-appx:///Assets/";
                if (DateTime.Now.Hour >= 18 || DateTime.Now.Hour <= 4)
                {
                    hostIconURL = hostIconURL + "nt_";
                }
                while (i <= 3 && i < weatherData.forecastday.Length)
                {
                    if (i == 0)
                    {

                        Temp1.Text = weatherData.forecastday[0].high.fahrenheit + "°/" + weatherData.forecastday[0].low.fahrenheit + "°";
                        //var uri = new System.Uri("ms-appdata:///local/Assests/chancerain.png");
                        //var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
                        //weatherIcon1.Source = new BitmapImage(new Uri("ms-appx:///Assets/chancesleet.png"));
                        weatherIcon1.Source = new BitmapImage(new Uri(hostIconURL + weatherData.forecastday[0].icon + ".png"));
                        
                    }
                    else if (i == 1)
                    {
                        day2.Text = weatherData.forecastday[1].date.weekday;
                        Temp2.Text = weatherData.forecastday[1].high.fahrenheit + "°/" + weatherData.forecastday[1].low.fahrenheit + "°";
                        weatherIcon2.Source = new BitmapImage(new Uri(hostIconURL + weatherData.forecastday[1].icon + ".png"));
                    }
                    else if (i == 2)
                    {
                        day3.Text = weatherData.forecastday[2].date.weekday;
                        Temp3.Text = weatherData.forecastday[2].high.fahrenheit + "°/" + weatherData.forecastday[2].low.fahrenheit + "°";
                        weatherIcon3.Source = new BitmapImage(new Uri(hostIconURL + weatherData.forecastday[2].icon + ".png"));
                    }
                    else if (i == 3)
                    {
                        day4.Text = weatherData.forecastday[3].date.weekday;
                        Temp4.Text = weatherData.forecastday[3].high.fahrenheit + "°/" + weatherData.forecastday[3].low.fahrenheit + "°";
                        weatherIcon4.Source = new BitmapImage(new Uri(hostIconURL + weatherData.forecastday[3].icon + ".png"));
                    }
                    i++;
                }

            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Get the default GPIO controller on the system
                
            // Latch HIGH value first. This ensures a default value when the pin is set as output
            pin.Write(GpioPinValue.High);
            // Set the IO direction as output
            pin.SetDriveMode(GpioPinDriveMode.Output);
            Debug.WriteLine("On");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            // Latch HIGH value first. This ensures a default value when the pin is set as output
            pin.Write(GpioPinValue.Low);
            Debug.WriteLine("Off");

        }
    }
}
