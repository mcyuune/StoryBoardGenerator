using Microsoft.AspNetCore.Mvc.RazorPages;
using StoryBoardGenerator.Models;
using StoryBoardGenerator.Services;

namespace StoryBoardGenerator.Pages;

public class GalleryModel : PageModel
{
    private readonly StoryBoardService _storyBoards;

    public GalleryModel(StoryBoardService storyBoards)
    {
        _storyBoards = storyBoards;
    }

    public IReadOnlyList<Story> Stories { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Stories = await _storyBoards.ListAsync(cancellationToken);
    }
}
