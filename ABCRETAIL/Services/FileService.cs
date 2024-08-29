using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Files.Shares;

namespace ABCRETAIL.Services
{
    public class FileService
    {
        private readonly ShareClient _shareClient;

        public FileService(string connectionString, string shareName)
        {
            _shareClient = new ShareClient(connectionString, shareName);
            _shareClient.CreateIfNotExists();
        }

        public async Task UploadFileAsync(string localFilePath, string remoteFileName)
        {
            ShareDirectoryClient directoryClient = _shareClient.GetRootDirectoryClient();
            ShareFileClient fileClient = directoryClient.GetFileClient(remoteFileName);

            // Check if the file already exists and delete it if necessary
            if (await fileClient.ExistsAsync())
            {
                await fileClient.DeleteAsync();
            }

            // Open the file stream to read the local file
            using FileStream fs = File.OpenRead(localFilePath);

            // Create or overwrite the file
            await fileClient.CreateAsync(fs.Length);

            // Upload the file
            await fileClient.UploadAsync(fs);
        }

        public async Task DownloadFileAsync(string remoteFileName, string localFilePath)
        {
            ShareDirectoryClient directoryClient = _shareClient.GetRootDirectoryClient();
            ShareFileClient fileClient = directoryClient.GetFileClient(remoteFileName);

            ShareFileDownloadInfo download = await fileClient.DownloadAsync();

            // Create or overwrite the local file
            using FileStream fs = File.Create(localFilePath);
            await download.Content.CopyToAsync(fs);
        }

        public async Task ListFilesAsync()
        {
            ShareDirectoryClient directoryClient = _shareClient.GetRootDirectoryClient();
            await foreach (ShareFileItem fileItem in directoryClient.GetFilesAndDirectoriesAsync())
            {
                Console.WriteLine($"File Name: {fileItem.Name}");
            }
        }
    }
}

