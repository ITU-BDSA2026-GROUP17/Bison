using Bison.Database.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class PublicModel(IObservationService service) : PageModel
{
    internal static readonly int PAGE_SIZE = 32;
    private readonly IObservationService _service = service;
    public List<ObservationDTO>? Observations { get; set; }

    public async Task<ActionResult> OnGet([FromQuery] int page, [FromQuery] string taxonId)
    {
        try
        {
            page = page < 2 ? 0 : page;
            Observations = await _service.GetObservationsByTaxonAsync(taxonId, PAGE_SIZE, PAGE_SIZE * (page - 1));
        }
        catch
        {
            Observations = page < 2
                ? await _service.GetObservationsAsync(PAGE_SIZE)
                : await _service.GetObservationsAsync(PAGE_SIZE, PAGE_SIZE * (page - 1));
        }
        return Page();
    }
}
