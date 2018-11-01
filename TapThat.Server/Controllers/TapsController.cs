using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace TapThat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TapsController : ControllerBase
    {
        private static Random random = new Random();
        private IMemoryCache _cache;

        public TapsController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            return RandomString(4);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<List<TapModel>> Get(string id)
        {
            var y = _cache.Get<List<TapModel>>($"Taps_{id}");
            return new JsonResult(y);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            var t = JsonConvert.DeserializeAnonymousType(value, new { ID = "", Nick = "" });

            var u = _cache.GetOrCreate($"Taps_{t.ID}", b => new List<TapModel>() {
                new TapModel() {
                        ID = t.ID,
                        Nick = t.Nick,
                        Taps = 0
                    }
                }
            );

            if (!u.Any(q => q.Nick == t.Nick))
            {
                var m = _cache.Get<List<TapModel>>($"Taps_{t.ID}");
                m.Add(new TapModel()
                {
                    ID = t.ID,
                    Nick = t.Nick,
                    Taps = 0
                });

                _cache.Set($"Taps_{t.ID}", m);
            }

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(string id, [FromBody] string value)
        {
            var t = JsonConvert.DeserializeAnonymousType(value, new { ID = "", Nick = "", taps = (long)0 });
            
            var u = _cache.GetOrCreate($"Taps_{t.ID}", b => new List<TapModel>() {
                new TapModel() {
                        ID = t.ID,
                        Nick = t.Nick,
                        Taps = t.taps
                    }
                }
            );

            if (!u.Any(q => q.Nick == t.Nick))
            {
                var m = _cache.Get<List<TapModel>>($"Taps_{t.ID}");
                m.Add(new TapModel()
                {
                    ID = t.ID,
                    Nick = t.Nick,
                    Taps = t.taps
                });

                _cache.Set($"Taps_{t.ID}", m);
            } else
            {
                var m = _cache.Get<List<TapModel>>($"Taps_{t.ID}");
                m.FirstOrDefault(y => y.Nick == t.Nick).Taps = t.taps;

                _cache.Set($"Taps_{t.ID}", m);
            }
        }
    }

    public class TapModel
    {
        public string ID { get; set; }

        public string Nick { get; set; }

        public long Taps { get; set; }
    }
}
