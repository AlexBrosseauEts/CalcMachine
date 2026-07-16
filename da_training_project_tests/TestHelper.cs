using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CalcMachineTests
{
    public static class TestRunner
    {
        public static string RunWithTimeout(string input, int timeoutMs = 5000)
        {
            var projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            var csprojPath = Path.Combine(projectDir, "CalcMachine", "CalcMachine.csproj");

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{csprojPath}\" --no-build",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = new Process { StartInfo = psi };
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null) outputBuilder.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null) errorBuilder.AppendLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.StandardInput.Write(input);
            process.StandardInput.Close();

            if (!process.WaitForExit(timeoutMs))
            {
                try { process.Kill(true); } catch { }
                process.WaitForExit(1000);
            }

            return outputBuilder.ToString();
        }
    }
}
