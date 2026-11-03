using Microsoft.EntityFrameworkCore;
using StoryBoardGenerator.Data;
using StoryBoardGenerator.Models;

namespace StoryBoardGenerator.Services;

public class StoryBoardService
{
    private readonly AppDbContext _db;
    private readonly ISceneSplitter _splitter;
    private readonly IImageGenerator _images;
    private readonly IWebHostEnvironment _env;

    public StoryBoardService(
        AppDbContext db,
        ISceneSplitter splitter,
        IImageGenerator images,
        IWebHostEnvironment env)
    {
        _db = db;
        _splitter = splitter;
        _images = images;
        _env = env;
    }

    public async Task<IReadOnlyList<Story>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Stories
            .OrderBy(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Story?> GetWithScenesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.Stories
            .Include(s => s.Scenes.OrderBy(scene => scene.Sequence))
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Story> CreateAsync(string title, string sourceText, CancellationToken cancellationToken = default)
    {
        var scenes = _splitter.Split(sourceText);
        var story = new Story { Title = title.Trim(), SourceText = sourceText.Trim() };
        _db.Stories.Add(story);
        await _db.SaveChangesAsync(cancellationToken);

        var folder = Path.Combine(_env.WebRootPath, "uploads", story.Id.ToString());
        Directory.CreateDirectory(folder);

        foreach (var scene in scenes)
        {
            var bytes = await _images.GenerateAsync(scene.Caption, cancellationToken);
            var fileName = $"scene-{scene.Sequence}.svg";
            await File.WriteAllBytesAsync(Path.Combine(folder, fileName), bytes, cancellationToken);

            scene.ImagePath = $"/uploads/{story.Id}/{fileName}";
            story.Scenes.Add(scene);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return story;
    }
}
