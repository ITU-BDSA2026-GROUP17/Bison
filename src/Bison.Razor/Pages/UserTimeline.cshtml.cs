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

    public async Task<ActionResult> OnGet(string author, [FromQuery] int page)
    {
        if (page < 2)
        {
            var user = await _userService.GetUserByNameAsync(author);
            if (user is not null)
            {
                Observations = await _obsService.GetObservationsByUserAsync(user.Id, PublicModel.PAGE_SIZE);
            }
            return Page();
        }
        else
        {
            var user = await _userService.GetUserByNameAsync(author);
            if (user is not null)
            {
                Observations = await _obsService.GetObservationsByUserAsync(
                    user.Id,
                    PublicModel.PAGE_SIZE,
                    PublicModel.PAGE_SIZE * (page - 1)
                );
            }
            return Page();
        }
    }
}
