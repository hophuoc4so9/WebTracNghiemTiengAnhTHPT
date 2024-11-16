using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WebTracNghiemTiengAnhTHPT.Services
{
    public class GroqService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.groq.com/openai/v1";
        private const string ChatCompletionsEndpoint = "/chat/completions";
        private const string TranscriptionsEndpoint = "/audio/transcriptions";
        private const string TranslationsEndpoint = "/audio/translations";

        public GroqService(string apiKey)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<JsonObject> CreateChatCompletionAsync(JsonObject request)
        {
            var content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(BaseUrl + ChatCompletionsEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}. Response content: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonNode.Parse(responseContent)?.AsObject() ?? new JsonObject();
        }
    }
}
