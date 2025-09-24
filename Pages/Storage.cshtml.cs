using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AzureStorageWebApp.Services;

namespace AzureStorageWebApp.Pages;

public class StorageModel : PageModel
{
    private readonly BlobStorageService _blobStorageService;
    private readonly ILogger<StorageModel> _logger;

    public StorageModel(BlobStorageService blobStorageService, ILogger<StorageModel> logger)
    {
        _blobStorageService = blobStorageService;
        _logger = logger;
    }

    [BindProperty]
    public IFormFile? UploadedFile { get; set; }

    [BindProperty]
    public string? TextContent { get; set; }

    [BindProperty]
    public string? TextFileName { get; set; }

    public List<string> BlobList { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            BlobList = await _blobStorageService.ListBlobsAsync("samples");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading blob list");
            // Handle the error appropriately
        }
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
        if (UploadedFile == null || UploadedFile.Length == 0)
        {
            return Page();
        }

        try
        {
            using var stream = UploadedFile.OpenReadStream();
            await _blobStorageService.UploadBlobAsync("samples", UploadedFile.FileName, stream);
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            // Handle the error appropriately
            return Page();
        }
    }

    public async Task<IActionResult> OnGetDownloadAsync(string blobName)
    {
        try
        {
            var stream = await _blobStorageService.DownloadBlobAsync("samples", blobName);
            return File(stream, "application/octet-stream", blobName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading blob {BlobName}", blobName);
            return RedirectToPage();
        }
    }

    public async Task<IActionResult> OnPostSaveTextAsync()
    {
        if (string.IsNullOrEmpty(TextContent) || string.IsNullOrEmpty(TextFileName))
        {
            return Page();
        }

        try
        {
            // Ensure the filename has a .txt extension
            var fileName = TextFileName.EndsWith(".txt") ? TextFileName : $"{TextFileName}.txt";
            await _blobStorageService.SaveTextAsync("samples", fileName, TextContent);
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving text content");
            return Page();
        }
    }
}