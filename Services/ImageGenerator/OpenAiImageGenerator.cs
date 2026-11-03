using System.Net;
using System.Text;

namespace StoryBoardGenerator.Services.ImageGenerator;


/// <summary>
/// Stand-in until a real image API is wired up. Produces an SVG the browser can display.
/// Interviews: this is the seam — swap this class for an HTTP client to DALL·E / similar.
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
    
    public Task<byte[]> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(prompt);
        throw new NotImplementedException("Image generation is not implemented yet.");
    }
}
