namespace StoryBoardGenerator.Services;

public interface IImageGenerator
{
    Task<byte[]> GenerateAsync(string prompt, CancellationToken cancellationToken = default);
}
