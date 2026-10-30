using System.Diagnostics;
using System.IO;
namespace USBGuard;
public record Entry(string Relative, bool Blocked);
public static class Pipeline
{
    static readonly HashSet<string> Dangerous = new(StringComparer.OrdinalIgnoreCase) { ".exe", ".dll", ".msi", ".bat", ".cmd", ".scr", ".com", ".js", ".jse", ".vbs", ".vbe", ".ps1", ".psm1", ".hta", ".lnk", ".url", ".reg", ".wsf", ".wsh" };
    public static List<Entry> Inspect(string root, CancellationToken token)
    {
        var result = new List<Entry>();
        Walk(root, root, result, token);
        return result;
    }
    static void Walk(string root, string directory, List<Entry> result, CancellationToken token)
    {
        foreach (var path in Directory.EnumerateFileSystemEntries(directory))
        {
            token.ThrowIfCancellationRequested();
            var attrs = File.GetAttributes(path);
            if ((attrs & FileAttributes.ReparsePoint) != 0) throw new IOException("Links and reparse points are not supported: " + path);
            if ((attrs & FileAttributes.Directory) != 0) { Walk(root, path, result, token); continue; }
            using var stream = File.OpenRead(path);
            int first = stream.ReadByte(), second = stream.ReadByte();
            result.Add(new(Path.GetRelativePath(root, path), Dangerous.Contains(Path.GetExtension(path)) || (first == 0x4d && second == 0x5a)));
        }
    }
    public static async Task<string> ImportAsync(string root, CancellationToken token)
    {
        if (new DriveInfo(root).DriveType != DriveType.Removable) throw new IOException("Select a removable drive.");
        var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "USBGuard");
        if (Path.GetPathRoot(basePath)!.Equals(Path.GetPathRoot(root), StringComparison.OrdinalIgnoreCase)) throw new IOException("Staging must be on a different local drive.");
        var id = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N");
        var incoming = Path.Combine(basePath, "Incoming", id);
        Directory.CreateDirectory(incoming);
        // Re-check the copied bytes, not only the removable source.
        var source = Inspect(root, token);
        foreach (var entry in source)
        {
            token.ThrowIfCancellationRequested();
            var from = Path.Combine(root, entry.Relative);
            if ((File.GetAttributes(from) & FileAttributes.ReparsePoint) != 0) throw new IOException("Source changed to a link.");
            var target = Path.Combine(incoming, entry.Relative);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(from, target, false);
        }
        var copied = Inspect(incoming, token);
        if (copied.Any(e => e.Blocked)) return "Import held in Incoming: executable extensions or MZ signatures found. " + incoming;
        var platform = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft", "Windows Defender", "Platform");
        var executable = Directory.Exists(platform) ? Directory.GetDirectories(platform).OrderByDescending(p => p, StringComparer.OrdinalIgnoreCase).Select(p => Path.Combine(p, "MpCmdRun.exe")).FirstOrDefault(File.Exists) : null;
        executable ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Windows Defender", "MpCmdRun.exe");
        if (!File.Exists(executable)) throw new IOException("Microsoft Defender command-line scanner unavailable. Import retained in Incoming.");
        var info = new ProcessStartInfo(executable) { UseShellExecute = false, CreateNoWindow = true };
        foreach (var arg in new[] { "-Scan", "-ScanType", "3", "-File", incoming, "-DisableRemediation" }) info.ArgumentList.Add(arg);
        using var process = Process.Start(info) ?? throw new IOException("Could not start Defender.");
        try { await process.WaitForExitAsync(token); }
        catch { if (!process.HasExited) process.Kill(true); throw; }
        if (process.ExitCode != 0) throw new IOException($"Defender returned {process.ExitCode}. Import retained in Incoming.");
        var safe = Path.Combine(basePath, "Safe", id);
        Directory.CreateDirectory(Path.GetDirectoryName(safe)!);
        Directory.Move(incoming, safe);
        return "Scan completed with no threats reported. Available at: " + safe;
    }
}
