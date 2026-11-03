using System.Net;
using System.Text;

namespace StoryBoardGenerator.Services;

/// <summary>
/// Stand-in until a real image API is wired up. Produces an SVG the browser can display.
/// Interviews: this is the seam — swap this class for an HTTP client to DALL·E / similar.
/// </summary>
public class PlaceholderImageGenerator : IImageGenerator
{
    public Task<byte[]> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var preview = prompt.Length > 80 ? prompt[..80] + "…" : prompt;
        var escaped = WebUtility.HtmlEncode(preview);
        var svg = $"""
            <svg xmlns="http://www.w3.org/2000/svg" width="960" height="540" viewBox="0 0 960 540">
              <rect width="960" height="540" fill="#1f2937"/>
              <text x="480" y="250" fill="#f9fafb" font-size="28" font-family="Georgia, serif" text-anchor="middle">Scene</text>
              <text x="480" y="300" fill="#d1d5db" font-size="18" font-family="Georgia, serif" text-anchor="middle">{escaped}</text>
            </svg>
            """;

        return Task.FromResult(Encoding.UTF8.GetBytes(svg));
    }
}
