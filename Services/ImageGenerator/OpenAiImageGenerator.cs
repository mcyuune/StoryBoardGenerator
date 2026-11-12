using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace StoryBoardGenerator.Services.ImageGenerator;


/// <summary>
/// Generates PNG images using the OpenAI Images API.
/// </summary>
public class OpenAiImageGenerator : IImageGenerator
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiImageGenerator(
        HttpClient httpClient, 
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"] 
            ?? throw new InvalidOperationException(
            "OpenAI API key is missing.");
    }
    
    public async Task<byte[]> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/images/generations");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        request.Content = JsonContent.Create(new
        {
            model = "gpt-image-2.5-flare",
            prompt = prompt,
            n = 1,
            size = "1024x1024",
            quality = "low",
            output_format = "png"
        });

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string errorBody = await response.Content.ReadAsStringAsync(
                cancellationToken);

            throw new HttpRequestException(
            $"OpenAI image generation failed with HTTP " +
            $"{(int)response.StatusCode}: {errorBody}");
        }

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var result = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        if (result.RootElement.ValueKind != JsonValueKind.Object ||
            !result.RootElement.TryGetProperty("data", out var data) ||
            data.ValueKind != JsonValueKind.Array ||
            data.GetArrayLength() == 0 ||
            data[0].ValueKind != JsonValueKind.Object ||
            !data[0].TryGetProperty("b64_json", out var image) ||
            image.ValueKind != JsonValueKind.String ||
            string.IsNullOrWhiteSpace(image.GetString()))
        {
            throw new InvalidOperationException("OpenAI returned no image data.");
        }

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(image.GetString()!);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("OpenAI returned invalid base64 image data.", ex);
        }

        if (bytes.Length == 0)
        {
            throw new InvalidOperationException("OpenAI returned empty image data.");
        }

        return bytes;
    }
}
