namespace StoryBoardGenerator.Models;

public class Scene
{
    /// <summary>Database primary key. Unique across all stories — not the scene number.</summary>
    public int Id { get; set; }

    public int StoryId { get; set; }
    public Story? Story { get; set; }

    /// <summary>1-based scene number in this storyboard (1, 2, 3…).</summary>
    public int Sequence { get; set; }

    public string Caption { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
}
