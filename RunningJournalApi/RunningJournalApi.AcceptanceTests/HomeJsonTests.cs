using System;
using System.Configuration;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Simple.Data;
using Xunit;

namespace RunningJournalApi.AcceptanceTests
{
    public class HomeJsonTests
    {
        [Fact]
        public async Task GetReturnsResponseWithCorrectStatusCode()
        {
            using (var client = HttpClientFactory.Create())
            {
                var response = await client.GetAsync("");

                Assert.True(response.IsSuccessStatusCode);
            }
        }

        [Fact]
        public async Task PostReturnsPesponseWithCorrectStatusCode()
        {
            using (var client = HttpClientFactory.Create())
            {
                var json = new
                {
                    time = DateTimeOffset.Now,
                    distance = 8500,
                    duration = TimeSpan.FromMinutes(44)
                };

                var response = await client.PostAsJsonAsync("", json);

                Assert.True(response.IsSuccessStatusCode);
            }
        }

        [Fact]
        [UseDatabase]
        public async Task GetAfterPostReturnsReponseWithPostedEntry()
        {
            using (var client = HttpClientFactory.Create())
            {
                var json = new
                {
                    time = DateTimeOffset.Now,
                    distance = 8100,
                    duration = TimeSpan.FromMinutes(41)
                };
                var expected = json.ToJObject();
                await client.PostAsJsonAsync("", json);

                var response = await client.GetAsync("");

                var actual = await response.Content.ReadAsJsonAsync();
                Assert.Contains(expected, actual.entries);
            }
        }

        [Fact]
        [UseDatabase]
        public void GetRootReturnsCorrectEntryFromDatabase()
        {
            dynamic entry = new ExpandoObject();
            entry.time = DateTimeOffset.Now;
            entry.distance = 6000;
            entry.duration = TimeSpan.FromMinutes(31);

            var expected = ((object)entry).ToJObject();

            var connStr = ConfigurationManager.ConnectionStrings["running-journal"].ConnectionString;
            var db = Database.OpenConnection(connStr);

            var userId = db.User.Insert(UserName: "foo").UserId;
            entry.UserId = userId;
            db.JournalEntry.Insert(entry);

            using (var client = HttpClientFactory.Create(userName))
            {
                var response = client.GetAsync("").Result;

                var actual = response.Content.ReadAsJsonAsync().Result;
                Assert.Contains(expected, actual.entries);
            }
        }
    }
}