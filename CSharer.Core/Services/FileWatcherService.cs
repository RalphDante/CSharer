namespace CSharer.Core.Services
{
    public class FileWatcherService
    {
        private FileSystemWatcher? _watcher;
        public event Action<string>? OnNewVideoDetected;

        private async Task<bool> IsFileReady(string filePath)
        {
            var timeout = TimeSpan.FromSeconds(10);
            var start = DateTime.Now;

            while (DateTime.Now - start < timeout)
            {
                try
                {
                    using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                    return true;
                }
                catch (IOException)
                {
                    await Task.Delay(500);
                }
            }

            return false;
        }

        public void Start(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

            _watcher = new FileSystemWatcher(folderPath)
            {
                Filter = "*.mp4",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime
            };

            // Replace Created with Renamed
            _watcher.Renamed += (sender, e) =>
            {
                _ = Task.Run(async () =>
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] New video ready: {e.Name}");

                    await Task.Delay(2000); // Let the rename fully settle

                    if (!await IsFileReady(e.FullPath))
                    {
                        Console.WriteLine($"⚠️ Skipping {e.Name} - file not ready");
                        return;
                    }

                    OnNewVideoDetected?.Invoke(e.FullPath);
                });
            };
            _watcher.EnableRaisingEvents = true;
            Console.WriteLine($"👀 Watching folder: {folderPath}");
        }

        public void Stop()
        {
            _watcher?.Dispose();
            Console.WriteLine("⏸ Stopped watching folder");
        }
    }
}