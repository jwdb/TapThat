using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TapThat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Tapper : ContentPage
    {
        long _taps = 0;
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        double ticks = 0.0;
        bool playin = false;

        public string Nick { get; set; }
        public string ID { get; set; }

        long taps
        {
            get
            {
                return _taps;
            }
            set
            {
                _taps = value;
                tapped?.Invoke(null, null);
            }
        }

        public Tapper()
        {
            timer.Start();
            timer.Elapsed += Timer_Elapsed;
            InitializeComponent();
            tapped += Taps_tapped;
        }
        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!playin && ticks < 3)
            {
                ticks++;
                Device.BeginInvokeOnMainThread(() => Downcounter.Text = (3 - ticks).ToString());
            } else if (playin && ticks < 15)
            {
                ticks += 0.1;
                Device.BeginInvokeOnMainThread(() => Downcounter.Text = (15 - ticks).ToString());
            } else if (playin && ticks >= 15)
            {
                timer.Stop();
                playin = false;
                Device.BeginInvokeOnMainThread(() => Downcounter.Text = (0).ToString());

                using (HttpClient c = new HttpClient())
                {
                    c.BaseAddress = new Uri("http://chibi.hunter2.nl:5807");

                    var reg = new
                    {
                        ID,
                        Nick,
                        Taps = taps
                    };
                    StringBuilder sb = new StringBuilder();
                    using (StringWriter sw = new StringWriter(sb))
                    using (JsonTextWriter writer = new JsonTextWriter(sw))
                    {
                        writer.QuoteChar = '\'';

                        JsonSerializer ser = new JsonSerializer();
                        ser.Serialize(writer, reg);
                    }

                    var q = await c.PutAsync($"api/Taps/{ID}", new StringContent($"\"{sb.ToString()}\"", Encoding.UTF8, "text/json"));
                }
            }
            else
            {
                ticks = 0;
                playin = true;
                timer.Stop();
                timer.Interval = 100;
                timer.Start();
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (!playin)
                return;

            taps++;
        }

        private void Taps_tapped(object sender, EventArgs e)
        {
            Taps.Text = _taps.ToString();
        }

        private EventHandler tapped;
    }
}