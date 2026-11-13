using Bison.Database.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class ObservationModel(
    IObservationService observationService,
    ICommentService commentService,
    IProposalService proposalService,
    ILogger<ObservationModel> logger
) : PageModel
{
    private readonly IObservationService _observationService = observationService;
    private readonly ICommentService _commentService = commentService;
    private readonly IProposalService _proposalService = proposalService;
    private readonly ILogger<ObservationModel> _logger = logger;

    public ObservationDTO? Observation { get; set; }
    public List<CommentDTO> Comments { get; set; } = [];
    public List<ProposalDTO> Proposals { get; set; } = [];

    public async Task<ActionResult> OnGet([FromRoute] int id)
    {
        BisonLogger.LogRequestWithParams(_logger, "Observation", [id]);
        Observation = await _observationService.GetObservationByIdAsync(id);
        Comments = await _commentService.GetCommentsByObservationIdAsync(id);
        Proposals = await _proposalService.GetProposalsByObservationIdAsync(id);
        BisonLogger.ReturningPage(_logger);
        return Page();
    }
    // TODO: pagination for comments and proposals, when they have a superclass
    // TODO: make function "GetSubPostsForObservationAsync(int id)
}
