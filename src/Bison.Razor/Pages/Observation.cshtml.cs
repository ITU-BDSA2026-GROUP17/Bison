using System.ComponentModel.DataAnnotations;

using Bison.Database.Services;
using Bison.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bison.Razor.Pages;

public class CreateSubpostInputModel
{
    [Required]
    [StringLength(
        512,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 4)
    ]
    [DataType(DataType.Text)]
    [Display(Name = "Text")]
    public string Text { get; set; } = default!;

    [DataType(DataType.Text)]
    [Display(Name = "Taxonomy")]
    public string? TaxonId { get; set; } = default!;
}

public partial class ObservationModel(
    IObservationService observationService,
    ICommentService commentService,
    IProposalService proposalService,
    ITaxonService taxonService,
    ILogger<ObservationModel> logger,
    UserManager<User> userService,
    SignInManager<User> signInManager
) : PageModel
{
    private readonly IObservationService _observationService = observationService;
    private readonly ICommentService _commentService = commentService;
    private readonly IProposalService _proposalService = proposalService;
    private readonly ITaxonService _taxonService = taxonService;
    private readonly ILogger<ObservationModel> _logger = logger;

    private readonly UserManager<User> _userManager = userService;
    private readonly SignInManager<User> _signInManager = signInManager;

    public string? BackwardUrl;
    public string? ForwardUrl;
    public ObservationDTO? Observation { get; set; }
    public List<SelectListItem> Taxons { get; set; } = [];
    public List<SubPostDTO> Subposts { get; set; } = [];

    [BindProperty]
    public CreateSubpostInputModel Input { get; set; } = default!;

    public async Task<ActionResult> OnGet([FromRoute] int id, [FromQuery] int page)
    {
        BisonLogger.LogRequestWithParams(_logger, "GET", "Observation", [id, page]);
        Observation = await _observationService.GetObservationByIdAsync(id);

        if (User.Identity is not null && User.Identity.IsAuthenticated)
            Taxons = [
                new SelectListItem("None", "none", true),
                ..(await _taxonService.GetTaxonsAsync())
                .Where(t => t.VernacularName != null)
                .Select(t => new SelectListItem(t.VernacularName, t.DwCId))
            ];

        page = page > 1 ? page : 1;

        if (page > 1)
        {
            BackwardUrl = $"/?page={page - 1}";
        }

        Subposts = await _observationService.GetSubPostsAsync(
            id,
            PublicModel.PAGE_SIZE,
            (page - 1) * PublicModel.PAGE_SIZE
        );


        if (Subposts.Count == PublicModel.PAGE_SIZE)
        {
            ForwardUrl = $"/?page={page + 1}";
        }

        BisonLogger.ReturningPage(_logger);
        return Page();
    }

    public async Task<ActionResult> OnPostAsync([FromRoute] int id, [FromQuery] int page = 0)
    {
        BisonLogger.LogRequestWithParams(_logger, "POST", "Observation", [page, Input.Text, Input.TaxonId]);

        var userId = _userManager.GetUserId(User);
        if (!_signInManager.IsSignedIn(User) || userId is null)
            return Forbid();

        if (!ModelState.IsValid)
            return await OnGet(id, page);

        BisonLogger.LogIsValidModel(_logger);

        var taxon = await _taxonService.GetTaxonById(Input.TaxonId ?? "N/A");
        if (taxon is not null)
        {
            // valid taxon => create proposal
            var ppl = await _proposalService.CreateProposalAsync(userId, id, taxon.DwCId, Input.Text);

            LogCreatedComment(_logger, ppl.Id);
        }
        else
        {
            // invalid taxon => create comment
            var com = await _commentService.CreateCommentAsync(userId, id, Input.Text);

            LogCreatedComment(_logger, com.Id);
        }

        Input = default!;
        ModelState.Clear();

        BisonLogger.ReturningPage(_logger);
        return await OnGet(id, 0);
    }


    [LoggerMessage(Level = LogLevel.Trace, Message = "Created comment with id {obsId}")]
    public static partial void LogCreatedComment(ILogger logger, int obsId);
}
