using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace TapThat
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var t = new Tapper();
            t.Nick = Nick.Text;
            t.ID = ID.Text;

            using (HttpClient c = new HttpClient())
            {
                c.BaseAddress = new Uri("http://chibi.hunter2.nl:5807");

                var reg = new
                {
                    t.ID,
                    t.Nick
                };

                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    writer.QuoteChar = '\'';

                    JsonSerializer ser = new JsonSerializer();
                    ser.Serialize(writer, reg);
                }

                var u = await c.PostAsync("api/Taps", new StringContent($"\"{sb.ToString()}\"", System.Text.Encoding.UTF8, "text/json"));

                if (!u.IsSuccessStatusCode)
                    return;
            }

            await Navigation.PushAsync(t);
        }
    }
}
