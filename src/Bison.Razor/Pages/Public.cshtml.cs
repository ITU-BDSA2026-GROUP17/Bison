using System.ComponentModel.DataAnnotations;

using Bison.Database.Services;
using Bison.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bison.Razor.Pages;

[RequestFormLimits(MultipartBodyLengthLimit = 33_554_432)]
public class CreateObservationInputModel
{
    [Required]
    [StringLength(
        512,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 4)
    ]
    [DataType(DataType.Text)]
    [Display(Name = "Observation")]
    public string Observation { get; set; } = default!;

    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Taxonomy")]
    public string TaxonId { get; set; } = default!;

    [DataType(DataType.Upload)]
    [Validators.ContentHeader(
        "image/png,image/jpeg,image/gif", // make sure to update accept in .cshtml if it changes
        ErrorMessage = "Photo must be a png, jpeg, or gif.")
    ]
    [Validators.FileSize(
        5_242_880, // 5MB
        ErrorMessage = "Your photo must be less than 5 megabytes"
    )]
    public IFormFile? Photo { get; set; } = default;
}

public partial class PublicModel(
    IObservationService service,
    ITaxonService taxonService,
    ILogger<PublicModel> logger,
    UserManager<User> userService,
    SignInManager<User> signInManager,
    IConfiguration options
) : PageModel
{
    internal static readonly int PAGE_SIZE = 32;

    private readonly IObservationService _service = service;
    private readonly ITaxonService _taxonService = taxonService;
    private readonly ILogger<PublicModel> _logger = logger;

    private readonly UserManager<User> _userManager = userService;
    private readonly SignInManager<User> _signInManager = signInManager;

    private readonly string _pictureBasePath = options.GetValue<string>("PictureBasePath")!;

    public string? BackwardUrl;
    public string? ForwardUrl;
    public List<ObservationDTO>? Observations { get; set; }
    public List<SelectListItem> Taxons { get; set; } = [];

    [BindProperty]
    public CreateObservationInputModel Input { get; set; } = default!;

    public async Task<ActionResult> OnGet([FromQuery] int page = 0, [FromQuery] string taxonId = "")
    {
        BisonLogger.LogRequestWithParams(_logger, "GET", "Public", [page, taxonId]);

        if (User.Identity is not null && User.Identity.IsAuthenticated)
            Taxons = [..
                (await _taxonService.GetTaxonsAsync())
                .Where(t => t.VernacularName != null)
                .Select(t => new SelectListItem(t.VernacularName, t.DwCId))
            ];

        page = page < 2 ? 1 : page;

        if (page > 1)
        {
            BackwardUrl = $"/?page={page - 1}";
            if (taxonId != "")
            {
                BackwardUrl += $"&taxonId={taxonId}";
            }
        }

        try
        {
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

        if (Observations.Count == PAGE_SIZE)
        {
            ForwardUrl = $"/?page={page + 1}";
            if (taxonId != "")
            {
                ForwardUrl += $"&taxonId={taxonId}";
            }
        }

        BisonLogger.ReturningPage(_logger);
        return Page();
    }

    public async Task<ActionResult> OnPostAsync([FromQuery] int page = 0, [FromQuery] string taxonId = "")
    {
        BisonLogger.LogRequestWithParams(_logger, "POST", "Public", [page, taxonId, Input.Observation, Input.TaxonId, Input.Photo]);

        var userId = _userManager.GetUserId(User);
        if (!_signInManager.IsSignedIn(User) || userId is null)
            return Forbid();

        if (!ModelState.IsValid)
            return await OnGet(page, taxonId);

        var taxon = await _taxonService.GetTaxonById(Input.TaxonId);
        if (taxon is null)
        {
            LogInvalidTaxon(_logger, Input.TaxonId);
            ModelState.AddModelError("Input.TaxonId", "Invalid taxonomy chosen, please choose another.");
            return await OnGet(page, taxonId ?? "");
        }

        LogIsValidModel(_logger);

        string? picturePath = null;

        try
        {
            if (Input.Photo is { Length: > 0 })
            {
                var ext = Path.GetExtension(Input.Photo.FileName)
                    .ToLowerInvariant()
                    .Replace(".", string.Empty);
                var guid = Guid.NewGuid();

                Directory.CreateDirectory(_pictureBasePath);
                var path = Path.Combine(_pictureBasePath, $"{guid}.{ext}");

                await using var stream = System.IO.File.Create(path);
                await Input.Photo.CopyToAsync(stream);
                picturePath = path;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        await _service.CreateObservationAsync(userId, taxon.DwCId, Input.Observation, picturePath);

        Input = default!;
        ModelState.Clear();

        BisonLogger.ReturningPage(_logger);
        return await OnGet(0, "");
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Trying to get observations, by taxonId '{taxonId}', on page {page}")]
    public static partial void LogGetByTaxonId(ILogger logger, int page, string taxonId);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Failed to get observations filtered by taxon, defaulting to getting all observations on page {page}")]
    public static partial void LogGetWithoutTaxonId(ILogger logger, int page);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Invalid taxon id '{taxon}' is given, returning page")]
    public static partial void LogInvalidTaxon(ILogger logger, string taxon);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Input model is valid")]
    public static partial void LogIsValidModel(ILogger logger);
}
