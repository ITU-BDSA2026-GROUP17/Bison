using System.ComponentModel.DataAnnotations;

using Bison.Database.Services;
using Bison.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bison.Razor.Pages;

public class EditPostInputModel
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

[Authorize]
public partial class EditPostModel(
    IPostService postService,
    IObservationService observationService,
    ICommentService commentService,
    IProposalService proposalService,
    ITaxonService taxonService,
    ILogger<ObservationModel> logger,
    UserManager<User> userService,
    SignInManager<User> signInManager
) : PageModel
{
    private readonly IPostService _postService = postService;
    private readonly IObservationService _observationService = observationService;
    private readonly ICommentService _commentService = commentService;
    private readonly IProposalService _proposalService = proposalService;
    private readonly ITaxonService _taxonService = taxonService;
    private readonly ILogger<ObservationModel> _logger = logger;

    private readonly UserManager<User> _userManager = userService;
    private readonly SignInManager<User> _signInManager = signInManager;

    public string? BackwardUrl;
    public string? ForwardUrl;
    public PostDTO Post { get; set; } = default!;
    public List<SelectListItem> Taxons { get; set; } = [];

    [BindProperty]
    public EditPostInputModel Input { get; set; } = new();

    public async Task<ActionResult> OnGet([FromRoute] int id)
    {
        BisonLogger.LogRequestWithParams(_logger, "GET", "Observation", [id]);
        Post = (await _postService.GetPostAsync(id))!;

        if (Post is null || Post.Author.Username != User.Identity?.Name) return Redirect("/");

        if (Post.Comment is null)
        {
            Taxons = [..
                (await _taxonService.GetTaxonsAsync())
                .Where(t => t.VernacularName != null)
                .Select(t => new SelectListItem(t.VernacularName, t.DwCId))
            ];

            if (Post.Observation is ObservationDTO observation)
            {
                Input.TaxonId = observation.Taxon.DwCId;
            }
            else if (Post.Proposal is ProposalDTO proposal)
            {
                Input.TaxonId = proposal.Taxon.DwCId;
            }
            Taxons.ForEach(t =>
            {
                t.Selected = false;
                if (t.Value == Input.TaxonId)
                {
                    t.Selected = true;
                }
            });
        }

        Input.Text = Post.Text;

        BisonLogger.ReturningPage(_logger);
        return Page();
    }

    public async Task<ActionResult> OnPostAsync([FromRoute] int id, [FromQuery] int page = 0, [FromQuery] string returnUrl = "/")
    {
        BisonLogger.LogRequestWithParams(_logger, "POST", "EditPost", [page, Input.Text, Input.TaxonId]);

        var post = (await _postService.GetPostAsync(id))!;
        var userId = _userManager.GetUserId(User);
        if (!_signInManager.IsSignedIn(User) || userId is null || post is null || post.Author.Id != userId)
            return Redirect("/");

        if (!ModelState.IsValid)
            return await OnGet(id);

        BisonLogger.LogIsValidModel(_logger);

        await UpdatePost(id, post);
        LogEditedPost(_logger, post.Type, id);

        Input = new();
        ModelState.Clear();

        BisonLogger.ReturningPage(_logger);
        return Redirect(returnUrl);
    }

    private async Task UpdatePost(int obsId, PostDTO post)
    {
        if (post.Observation is not null)
        {
            await _observationService.EditObservationAsync(obsId, Input.TaxonId, Input.Text);
        }
        if (post.Comment is not null)
        {
            await _commentService.EditCommentAsync(obsId, Input.Text);
        }
        if (post.Proposal is not null)
        {
            await _proposalService.EditProposalAsync(obsId, Input.TaxonId, Input.Text);
        }
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Edited {type} with id {id}")]
    public static partial void LogEditedPost(ILogger logger, string type, int id);
}
