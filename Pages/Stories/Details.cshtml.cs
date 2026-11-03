using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoryBoardGenerator.Models;
using StoryBoardGenerator.Services;

namespace StoryBoardGenerator.Pages.Stories;

public class DetailsModel : PageModel
{
    private readonly StoryBoardService _storyBoards;

    public DetailsModel(StoryBoardService storyBoards)
    {
        _storyBoards = storyBoards;
    }

    public Story? Story { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        Story = await _storyBoards.GetWithScenesAsync(id, cancellationToken);
        if (Story is null)
        {
            return NotFound();
        }

        return Page();
    }
}
