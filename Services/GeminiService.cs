using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace NeuroMentor.Api.Services;

public class GeminiService(HttpClient http, IConfiguration config)
{
    private string ApiKey => config["Gemini:ApiKey"]!;
    private const string Model = "gemini-2.0-flash";
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";

    public async Task<string> CompleteAsync(string system, string userPrompt, int maxTokens = 2000)
    {
        var url = $"{BaseUrl}/{Model}:generateContent?key={ApiKey}";

        var payload = new
        {
            system_instruction = new { parts = new[] { new { text = system } } },
            contents = new[]
            {
                new { role = "user", parts = new[] { new { text = userPrompt } } }
            },
            generationConfig = new { maxOutputTokens = maxTokens }
        };

        var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var res = await http.SendAsync(req);
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "";
    }

    public async IAsyncEnumerable<string> StreamAsync(
        string system,
        List<object> messages,
        int maxTokens = 1500,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/{Model}:streamGenerateContent?alt=sse&key={ApiKey}";

        var contents = messages.Select(m =>
        {
            var json = JsonSerializer.Serialize(m);
            using var doc = JsonDocument.Parse(json);
            var role = doc.RootElement.GetProperty("role").GetString() == "assistant" ? "model" : "user";
            var content = doc.RootElement.GetProperty("content").GetString() ?? "";
            return new { role, parts = new[] { new { text = content } } };
        }).ToList();

        var payload = new
        {
            system_instruction = new { parts = new[] { new { text = system } } },
            contents,
            generationConfig = new { maxOutputTokens = maxTokens }
        };

        var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var res = await http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
        res.EnsureSuccessStatusCode();

        await using var stream = await res.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (!ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (line is null) break;
            if (!line.StartsWith("data: ")) continue;

            var data = line[6..].Trim();
            if (data == "[DONE]") break;

            string text;
            try
            {
                using var doc = JsonDocument.Parse(data);
                text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "";
            }
            catch { continue; }

            if (!string.IsNullOrEmpty(text))
                yield return text;
        }
    }
}