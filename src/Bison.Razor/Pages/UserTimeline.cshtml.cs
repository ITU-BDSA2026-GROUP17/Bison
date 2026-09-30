using Bison.Database.Services;
using Bison.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class UserTimelineModel(IObservationService service, IUserService uService) : PageModel
{
    private readonly IObservationService _obsService = service;
    private readonly IUserService _userService = uService;
    public List<Observation>? Observations { get; set; }

    public async Task<ActionResult> OnGet(string author)
    {
        var user = await _userService.GetUserByNameAsync(author);
        if (user is not null)
        {
            Observations = await _obsService.GetObservationsByUserAsync(user.Id);
        }
        return Page();
    }
}
