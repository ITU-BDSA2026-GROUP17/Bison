using Bison.Database.Services;
using Bison.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class PublicModel(IObservationService service) : PageModel
{
    private readonly IObservationService _service = service;
    public List<Observation>? Observations { get; set; }

    public async Task<ActionResult> OnGet()
    {
        Observations = await _service.GetObservationsAsync();
        return Page();
    }
}
