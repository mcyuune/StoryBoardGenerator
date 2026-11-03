using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoryBoardGenerator.Services;

namespace StoryBoardGenerator.Pages;

public class IndexModel : PageModel
{
    private readonly StoryBoardService _storyBoards;

    public IndexModel(StoryBoardService storyBoards)
    {
        _storyBoards = storyBoards;
    }

    [BindProperty]
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [BindProperty]
    [Required, MinLength(20)]
    [Display(Name = "Story text")]
    public string SourceText { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var story = await _storyBoards.CreateAsync(Title, SourceText, cancellationToken);
        return RedirectToPage("/Stories/Details", new { id = story.Id });
    }
}
