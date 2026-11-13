using Bison.Database.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public partial class PublicModel(IObservationService service, ILogger<PublicModel> logger) : PageModel
{
    internal static readonly int PAGE_SIZE = 32;

    private readonly IObservationService _service = service;
    private readonly ILogger<PublicModel> _logger = logger;

    public List<ObservationDTO>? Observations { get; set; }

    public async Task<ActionResult> OnGet([FromQuery] int page, [FromQuery] string taxonId)
    {
        BisonLogger.LogRequestWithParams(_logger, "Public", [page, taxonId]);
        try
        {
            page = page < 2 ? 0 : page;
            LogGetByTaxonId(_logger, page, taxonId);
            Observations = await _service.GetObservationsByTaxonAsync(taxonId, PAGE_SIZE, PAGE_SIZE * (page - 1));
        }
        catch
        {
            LogGetWithoutTaxonId(_logger, page);
            Observations = page < 2
                ? await _service.GetObservationsAsync(PAGE_SIZE)
                : await _service.GetObservationsAsync(PAGE_SIZE, PAGE_SIZE * (page - 1));
        }
        BisonLogger.ReturningPage(_logger);
        return Page();
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Trying to get observations, by taxonId '{taxonId}', on page {page}")]
    public static partial void LogGetByTaxonId(ILogger logger, int page, string taxonId);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Failed to get observations filtered by taxon, defaulting to getting all observations on page {page}")]
    public static partial void LogGetWithoutTaxonId(ILogger logger, int page);
}
