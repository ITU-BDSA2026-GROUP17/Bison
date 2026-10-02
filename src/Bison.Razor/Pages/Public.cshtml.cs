using Bison.Database.Services;
using Bison.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class PublicModel(IObservationService service) : PageModel
{
    internal static readonly int PAGE_SIZE = 32;
    private readonly IObservationService _service = service;
    public List<Observation>? Observations { get; set; }

    public async Task<ActionResult> OnGet()
    {
        Observations = await _service.GetObservationsAsync(PAGE_SIZE);
        return Page();
    }
    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        if (page < 2)
        {
            return await OnGet();
        }
        else
        {
            Observations = await _service.GetObservationsAsync(PAGE_SIZE, PAGE_SIZE * (page - 1));
            return Page();
        }
    }
}
