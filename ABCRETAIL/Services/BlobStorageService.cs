using Azure.Storage.Blobs;

namespace ABCRETAIL.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobStorageService(string connectionString, string containerName)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = containerName.ToLower(); // Ensure the container name is lowercase
        }

        public async Task UploadBlobAsync(string blobName, Stream data)
        {
            blobName = SanitizeBlobName(blobName);   // Sanitize the blob name

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();  // Create the container if it does not exist
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(data, overwrite: true);  // Upload the blob
        }

        public async Task<Stream> DownloadBlobAsync(string blobName)
        {
            blobName = SanitizeBlobName(blobName);   // Sanitize the blob name

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var ms = new MemoryStream();
            await blobClient.DownloadToAsync(ms);  // Download the blob to a memory stream
            ms.Position = 0;
            return ms;
        }

        public async Task DeleteBlobAsync(string blobName)
        {
            blobName = SanitizeBlobName(blobName);   // Sanitize the blob name

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();  // Delete the blob if it exists
        }

        private string SanitizeBlobName(string blobName)
        {
            return Uri.EscapeDataString(blobName);  // Sanitize the blob name to escape any invalid characters
        }
    }
}
