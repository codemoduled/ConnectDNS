using System;
using System.Diagnostics;
using System.Security.Principal;

class Program
{
    static void Main(string[] args)
    {
        if (!IsAdministrator())
        {
            var proc = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true,
                Verb = "runas"
            };
            try { Process.Start(proc); } catch { }
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("================================");
        Console.WriteLine("         Connect DNS");
        Console.WriteLine("================================");
        Console.ResetColor();
        Console.WriteLine();

        Console.Write("[1/2] Setting DNS to Cloudflare... ");
        RunNetsh("interface ip set dns \"Wi-Fi\" static 1.1.1.1");
        RunNetsh("interface ip add dns \"Wi-Fi\" 1.0.0.1 index=2");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Done!");
        Console.ResetColor();

        Console.WriteLine();
        Console.Write("[2/2] Flushing DNS cache... ");
        RunCommand("ipconfig", "/flushdns");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Done!");
        Console.ResetColor();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("================================");
        Console.WriteLine("   All done! You're good to go.");
        Console.WriteLine("================================");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static void RunNetsh(string args)
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            }
        };
        p.Start();
        p.WaitForExit();
    }

    static void RunCommand(string cmd, string args)
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = cmd,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            }
        };
        p.Start();
        p.WaitForExit();
    }

    static bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
