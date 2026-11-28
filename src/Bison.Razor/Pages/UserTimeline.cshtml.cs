using Bison.Database.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bison.Razor.Pages;

public partial class UserTimelineModel(IObservationService service, IUserService uService, ILogger<UserTimelineModel> logger) : PageModel
{
    private readonly IObservationService _obsService = service;
    private readonly IUserService _userService = uService;
    private readonly ILogger<UserTimelineModel> _logger = logger;

    public string? BackwardUrl;
    public string? ForwardUrl;
    public List<ObservationDTO>? Observations { get; set; }
    public List<SelectListItem> Taxons { get; set; } = [];

    public async Task<ActionResult> OnGet([FromRoute] string author, [FromQuery] int page)
    {
        BisonLogger.LogRequestWithParams(_logger, "GET", "UserTimeline", [author, page]);

        page = page < 2 ? 1 : page;

        if (page > 1)
        {
            BackwardUrl = $"/obs/{author}?page={page - 1}";
        }

        LogGetNormalizedPage(_logger, page);
        var user = await _userService.GetUserByUserNameAsync(author);
        if (user is not null)
        {
            LogUserExists(_logger);
            Observations = await _obsService.GetObservationsByUserAsync(
                user.Id,
                PublicModel.PAGE_SIZE,
                PublicModel.PAGE_SIZE * (page - 1)
            );
        }

        if (Observations?.Count == PublicModel.PAGE_SIZE)
        {
            ForwardUrl = $"/obs/{author}?page={page + 1}";
        }

        BisonLogger.ReturningPage(_logger);
        return Page();
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Getting normalized page {page}")]
    public static partial void LogGetNormalizedPage(ILogger logger, int page);
    [LoggerMessage(Level = LogLevel.Trace, Message = "User exists, trying to get their observations")]
    public static partial void LogUserExists(ILogger logger);
}
