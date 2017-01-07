using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MangaSurvWebApi.Tests.Controllers.Users
{
    public class UsersTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

		public UsersTest()
        {
            this._server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            this._client = this._server.CreateClient();
        }

        [Fact]
		public async Task ReturnUsers()
        {
            var response = await _client.GetAsync("api/users");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal("", responseString);
        }
    }
}
