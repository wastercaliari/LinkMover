using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;

internal static class LinkMoverLauncher
{
    [STAThread]
    private static int Main()
    {
        try
        {
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("LinkMoverScript"))
            {
                if (input == null) throw new InvalidOperationException("O script interno nao foi encontrado.");
                string scriptPath = Path.Combine(Path.GetTempPath(), "LinkMover-" + Guid.NewGuid().ToString("N") + ".ps1");
                using (FileStream output = File.Create(scriptPath)) input.CopyTo(output);
                try
                {
                    ProcessStartInfo start = new ProcessStartInfo();
                    start.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell\\v1.0\\powershell.exe");
                    start.Arguments = "-NoProfile -ExecutionPolicy Bypass -File \"" + scriptPath + "\"";
                    start.UseShellExecute = false;
                    start.CreateNoWindow = true;
                    using (Process p = Process.Start(start)) { p.WaitForExit(); return p.ExitCode; }
                }
                finally { try { File.Delete(scriptPath); } catch { } }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Nao foi possivel iniciar o LinkMover:\r\n\r\n" + ex.Message, "LinkMover", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 1;
        }
    }
}