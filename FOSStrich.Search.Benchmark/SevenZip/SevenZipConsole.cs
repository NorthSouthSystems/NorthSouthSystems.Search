namespace FOSStrich.Search;

using FOSStrich.IO;
using System.Diagnostics;

// Adapted from: https://stackoverflow.com/questions/7994477/extract-7zip-in-c-sharp-code
//               https://stackoverflow.com/a/25786871
internal static class SevenZipConsole
{
    internal static void ExtractAllFiles(string archiveFilepath, string outputDirectory)
    {
        var processStartInfo = new ProcessStartInfo()
        {
            FileName = PathX.GetFullPathRelativeToCallerFilePath("7za.exe"),

            // x : Extract all files
            // -o : Output directory
            // -y : Answer "Yes" to all queries
            Arguments = FormattableString.Invariant($"x \"{archiveFilepath}\" -o\"{outputDirectory}\" -y"),

            UseShellExecute = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        using var process = Process.Start(processStartInfo);
        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new Exception(process.ExitCode.ToString());
    }
}