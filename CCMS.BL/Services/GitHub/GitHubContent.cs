using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace CCMS.BL.Services.GitHub
{
    public class GitHubContent : IDisposable
    {
        private string directoryPath;

        private string filePath;

        public DirectoryInfo ContentDirectory { get; private set; }

        public void Dispose()
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);
        }

        public static async Task<GitHubContent> Create(byte[] content)
        {
            var instance = new GitHubContent();
            await instance.CreateInstance(content);
            return instance;
        }

        public async Task CreateInstance(byte[] content)
        {
            directoryPath = "repos/" + Guid.NewGuid();
            filePath = directoryPath + ".zip";
            try
            {
                Directory.CreateDirectory(directoryPath);
                await File.WriteAllBytesAsync(filePath, content);
                await Task.Run(() => ZipFile.ExtractToDirectory(filePath, directoryPath));
                await Task.Run(() => File.Delete(filePath));
                ContentDirectory = new DirectoryInfo(directoryPath).GetDirectories().FirstOrDefault();
            }
            catch
            {
                Dispose();
            }
        }
    }
}