using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;

namespace StoryBoardGenerator.Services;

/// <summary>
/// Stand-in until a real image API is wired up. Produces a PNG the browser can display.
/// Interviews: this is the seam — swap this class for an HTTP client to DALL·E / similar.
/// </summary>
public class PlaceholderImageGenerator : IImageGenerator
{
    public Task<byte[]> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(prompt);
        cancellationToken.ThrowIfCancellationRequested();

        const int width = 960;
        const int height = 540;
        const int stride = width * 3 + 1;
        var pixels = new byte[stride * height];
        for (var y = 0; y < height; y++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Each PNG scanline starts with filter type 0 (no filtering).
            for (var x = 0; x < width; x++)
            {
                var offset = y * stride + 1 + x * 3;
                pixels[offset] = 0x1f;
                pixels[offset + 1] = 0x29;
                pixels[offset + 2] = 0x37;
            }
        }

        var preview = prompt.Length > 80 ? prompt[..80] + "..." : prompt;
        DrawText("Scene", 225, 4, 0xf9, 0xfa, 0xfb);
        DrawText(preview, 290, 1, 0xd1, 0xd5, 0xdb);

        using var png = new MemoryStream();
        png.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
        var header = new byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(0, 4), width);
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4, 4), height);
        header[8] = 8; // Eight bits per channel.
        header[9] = 2; // RGB, without alpha.
        WriteChunk(png, "IHDR", header);

        using var compressed = new MemoryStream();
        using (var zlib = new ZLibStream(compressed, CompressionLevel.Fastest, leaveOpen: true))
        {
            zlib.Write(pixels);
        }

        WriteChunk(png, "IDAT", compressed.ToArray());
        WriteChunk(png, "IEND", Array.Empty<byte>());
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(png.ToArray());

        void DrawText(string text, int top, int scale, byte red, byte green, byte blue)
        {
            var left = (width - (text.Length * 6 - 1) * scale) / 2;
            foreach (var character in text.ToUpperInvariant())
            {
                var glyph = Glyphs.GetValueOrDefault(character, Glyphs['?']);
                for (var row = 0; row < 7; row++)
                for (var column = 0; column < 5; column++)
                {
                    if ((glyph[row] & (1 << (4 - column))) == 0)
                        continue;

                    for (var dy = 0; dy < scale; dy++)
                    for (var dx = 0; dx < scale; dx++)
                    {
                        var offset = (top + row * scale + dy) * stride + 1 +
                            (left + column * scale + dx) * 3;
                        pixels[offset] = red;
                        pixels[offset + 1] = green;
                        pixels[offset + 2] = blue;
                    }
                }
                left += 6 * scale;
            }
        }
    }

    private static void WriteChunk(Stream output, string type, byte[] data)
    {
        Span<byte> number = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(number, data.Length);
        output.Write(number);
        var typeBytes = Encoding.ASCII.GetBytes(type);
        output.Write(typeBytes);
        output.Write(data);

        // PNG chunks use CRC-32 over the chunk type and payload.
        var crc = uint.MaxValue;
        UpdateCrc(typeBytes);
        UpdateCrc(data);
        BinaryPrimitives.WriteUInt32BigEndian(number, ~crc);
        output.Write(number);

        void UpdateCrc(byte[] bytes)
        {
            foreach (var value in bytes)
            {
                crc ^= value;
                for (var bit = 0; bit < 8; bit++)
                    crc = (crc >> 1) ^ ((crc & 1) != 0 ? 0xedb88320u : 0u);
            }
        }
    }

    // A small bitmap font keeps placeholder rendering independent of native graphics libraries.
    // Unsupported characters are rendered as question marks.
    private static readonly Dictionary<char, byte[]> Glyphs = new()
    {
        [' '] = [0, 0, 0, 0, 0, 0, 0],
        ['A'] = [14, 17, 17, 31, 17, 17, 17],
        ['B'] = [30, 17, 17, 30, 17, 17, 30],
        ['C'] = [14, 17, 16, 16, 16, 17, 14],
        ['D'] = [30, 17, 17, 17, 17, 17, 30],
        ['E'] = [31, 16, 16, 30, 16, 16, 31],
        ['F'] = [31, 16, 16, 30, 16, 16, 16],
        ['G'] = [14, 17, 16, 23, 17, 17, 14],
        ['H'] = [17, 17, 17, 31, 17, 17, 17],
        ['I'] = [14, 4, 4, 4, 4, 4, 14],
        ['J'] = [7, 2, 2, 2, 18, 18, 12],
        ['K'] = [17, 18, 20, 24, 20, 18, 17],
        ['L'] = [16, 16, 16, 16, 16, 16, 31],
        ['M'] = [17, 27, 21, 21, 17, 17, 17],
        ['N'] = [17, 25, 21, 19, 17, 17, 17],
        ['O'] = [14, 17, 17, 17, 17, 17, 14],
        ['P'] = [30, 17, 17, 30, 16, 16, 16],
        ['Q'] = [14, 17, 17, 17, 21, 18, 13],
        ['R'] = [30, 17, 17, 30, 20, 18, 17],
        ['S'] = [15, 16, 16, 14, 1, 1, 30],
        ['T'] = [31, 4, 4, 4, 4, 4, 4],
        ['U'] = [17, 17, 17, 17, 17, 17, 14],
        ['V'] = [17, 17, 17, 17, 17, 10, 4],
        ['W'] = [17, 17, 17, 21, 21, 21, 10],
        ['X'] = [17, 17, 10, 4, 10, 17, 17],
        ['Y'] = [17, 17, 10, 4, 4, 4, 4],
        ['Z'] = [31, 1, 2, 4, 8, 16, 31],
        ['0'] = [14, 17, 19, 21, 25, 17, 14],
        ['1'] = [4, 12, 4, 4, 4, 4, 14],
        ['2'] = [14, 17, 1, 2, 4, 8, 31],
        ['3'] = [30, 1, 1, 14, 1, 1, 30],
        ['4'] = [2, 6, 10, 18, 31, 2, 2],
        ['5'] = [31, 16, 16, 30, 1, 1, 30],
        ['6'] = [14, 16, 16, 30, 17, 17, 14],
        ['7'] = [31, 1, 2, 4, 8, 8, 8],
        ['8'] = [14, 17, 17, 14, 17, 17, 14],
        ['9'] = [14, 17, 17, 15, 1, 1, 14],
        ['.'] = [0, 0, 0, 0, 0, 6, 6],
        [','] = [0, 0, 0, 0, 6, 6, 4],
        ['!'] = [4, 4, 4, 4, 4, 0, 4],
        ['?'] = [14, 17, 1, 2, 4, 0, 4],
        ['-'] = [0, 0, 0, 31, 0, 0, 0],
        ['\''] = [4, 4, 2, 0, 0, 0, 0],
        ['"'] = [10, 10, 5, 0, 0, 0, 0],
        [':'] = [0, 6, 6, 0, 6, 6, 0]
    };
}
