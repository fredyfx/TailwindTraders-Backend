﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tailwind.Traders.WebBff.Infrastructure;
using Tailwind.Traders.WebBff.Models;

namespace Tailwind.Traders.WebBff.Controllers
{
    [AllowAnonymous]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _settings;
        private const string VERSION_API = "v1";

        public LoginController(
            IHttpClientFactory httpClientFactory,
            IOptions<AppSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _settings = options.Value;
        }

        // POST: v1/login
        [HttpPost()]
        public async Task<IActionResult> Login([FromBody] TokenRequest request)
        {
            var client = _httpClientFactory.CreateClient(HttpClients.ApiGW);

            var json = JsonConvert.SerializeObject(request);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await client.PostAsync(API.Login.PostLogin(_settings.LoginApiUrl, VERSION_API), stringContent);

            if (response.StatusCode == HttpStatusCode.BadRequest) {
                return BadRequest();
            }

            var result = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(result);
            return Ok(authResponse);
        }

        //// POST: v1/login
        //[HttpPost("/login/refresh")]
        //public async Task<IActionResult> LoginRefresh([FromBody] string refreshToken)
        //{
        //    var client = _httpClientFactory.CreateClient();

        //    var stringContent = new StringContent(refreshToken, UnicodeEncoding.UTF8, "application/json");

        //    var response = await client.PostAsync(API.Login.PostRefreshToken(_settings.LoginApiUrl, VERSION_API), stringContent);

        //    if (response.StatusCode == HttpStatusCode.BadRequest)
        //    {
        //        return BadRequest();
        //    }

        //    var result = await response.Content.ReadAsStringAsync();
        //    var login = JsonConvert.DeserializeObject<LoginResponse>(result);
        //    return Ok(login);
        //}


    }
}