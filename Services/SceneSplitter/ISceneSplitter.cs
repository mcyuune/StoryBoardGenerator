using StoryBoardGenerator.Models;

namespace StoryBoardGenerator.Services;

public interface ISceneSplitter
{
    IReadOnlyList<Scene> Split(string sourceText);
}
