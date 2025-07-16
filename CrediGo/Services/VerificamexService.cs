using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CrediGo.Models.Verificamex;

namespace CrediGo.Services
{
    public class VerificamexService
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;
        private const string BaseUrl = "https://api.verificamex.com/identity/v1/scraping/renapo";

        public VerificamexService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _token = configuration["Verificamex:Token"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        public async Task<CitizenRecord?> ValidarCurpAsync(string curp)
        {
            var request = new { curp = curp };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(BaseUrl, content);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var resultado = JsonSerializer.Deserialize<VerificamexRenapoResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return resultado?.data?.citizen?.registros?.FirstOrDefault();
        }

        public async Task<VerificamexRenapoResponse?> ValidarCurpConPdfAsync(string curp)
        {
            var request = new { curp = curp };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(BaseUrl, content);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var resultado = JsonSerializer.Deserialize<VerificamexRenapoResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return resultado;
        }

    }
}
