using Bison.Database.Services;
using Bison.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class ObservationModel(IObservationService service) : PageModel
{
    private readonly IObservationService _service = service;
    public Observation? Observation { get; set; }

    public async Task<ActionResult> OnGet(int id)
    {
        Observation = await _service.GetObservationByIdAsync(id);
        return Page();
    }
    //TODO: pagination for comments and proposals, when they have a superclass
}
