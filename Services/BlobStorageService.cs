using Azure.Identity;
using Azure.Storage.Blobs;

namespace AzureStorageWebApp.Services;

public class BlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
    {
        _logger = logger;
        
        var storageAccountEndpoint = configuration["AzureStorage:AccountEndpoint"];
        if (string.IsNullOrEmpty(storageAccountEndpoint))
        {
            throw new ArgumentException("AccountEndpoint is not configured for Azure Storage");
        }
        
        // Always use DefaultAzureCredential which supports multiple authentication methods
        _blobServiceClient = new BlobServiceClient(
            new Uri(storageAccountEndpoint),
            new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ExcludeSharedTokenCacheCredential = true,
                ExcludeManagedIdentityCredential = false,
                ExcludeVisualStudioCredential = false,
                ExcludeAzureCliCredential = true,
                ExcludeEnvironmentCredential = true
            }));
    }

    public async Task<List<string>> ListBlobsAsync(string containerName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = new List<string>();

            await foreach (var blob in containerClient.GetBlobsAsync())
            {
                blobs.Add(blob.Name);
            }

            return blobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing blobs in container {ContainerName}", containerName);
            throw;
        }
    }

    public async Task UploadBlobAsync(string containerName, string blobName, Stream content)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(content, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading blob {BlobName} to container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    public async Task<Stream> DownloadBlobAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            
            var download = await blobClient.DownloadStreamingAsync();
            return download.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading blob {BlobName} from container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    public async Task SaveTextAsync(string containerName, string blobName, string content)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            
            var blobClient = containerClient.GetBlobClient(blobName);
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            await writer.WriteAsync(content);
            await writer.FlushAsync();
            stream.Position = 0;
            
            await blobClient.UploadAsync(stream, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving text content to blob {BlobName} in container {ContainerName}", blobName, containerName);
            throw;
        }
    }
}