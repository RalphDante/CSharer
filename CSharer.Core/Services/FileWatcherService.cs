namespace CSharer.Core.Services
{
    public class FileWatcherService
    {
        private FileSystemWatcher? _watcher;
        public event Func<string, Task>? OnNewVideoDetected;


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
                catch (IOException ex)
                {
                        Console.WriteLine($"File locked: {ex.Message}"); // ← add this temporarily
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
                    try  // ← wrap the WHOLE thing
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] New video ready: {e.Name}");
                        await Task.Delay(2000);

                        Console.WriteLine("Checking file readiness...");
                        if (!await IsFileReady(e.FullPath))
                        {
                            Console.WriteLine($"Skipping {e.Name} - file not ready after 10s");
                            return;
                        }

                        Console.WriteLine("File is ready, invoking handler...");

                        // Await the handler properly instead of fire-and-forget
                        if (OnNewVideoDetected != null)
                        {
                            Console.WriteLine("Invoking handler...");
                            await OnNewVideoDetected.Invoke(e.FullPath);  // ← properly awaited now
                            Console.WriteLine("Handler done.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"FileWatcher error: {ex.Message}");
                        Console.WriteLine(ex.StackTrace);  // ← see exactly where it dies
                    }
                });
            };
            _watcher.EnableRaisingEvents = true;
            Console.WriteLine($"Watching folder: {folderPath}");
        }

        public void Stop()
        {
            _watcher?.Dispose();
            Console.WriteLine("Stopped watching folder");
        }
    }
}


