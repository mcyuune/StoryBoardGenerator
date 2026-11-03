using StoryBoardGenerator.Models;

namespace StoryBoardGenerator.Services;

/// <summary>
/// Rule-based splitter: blank lines, then sentences, capped at 8 scenes.
/// Swap this for another ISceneSplitter (for example an LLM) in Program.cs.
/// </summary>
public class RuleSplitGenerator : ISceneSplitter
{
    private const int MaxScenes = 8;

    public IReadOnlyList<Scene> Split(string sourceText)
    {
        var paragraphs = sourceText
            .Split(["\r\n\r\n", "\n\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(p => p.Length > 0)
            .ToList();

        if (paragraphs.Count == 1)
        {
            paragraphs = sourceText
                .Split(['.', '!', '?'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(s => s.Length > 12)
                .ToList();
        }

        if (paragraphs.Count == 0)
        {
            paragraphs = [sourceText.Trim()];
        }

        return paragraphs
            .Take(MaxScenes)
            .Select((caption, index) => new Scene
            {
                Sequence = index + 1,
                Caption = caption
            })
            .ToList();
    }
}
