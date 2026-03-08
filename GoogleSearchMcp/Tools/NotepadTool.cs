using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using FlaUI.UIA3;
using ModelContextProtocol.Server;

namespace GoogleSearchMcp.Tools
{
    [McpServerToolType]
    public class NotepadTool
    {
        private static readonly string LogFile = @"C:\agents\notepad_tool.log";

        private static void Log(string message)
        {
            try
            {
                File.AppendAllText(LogFile, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}{Environment.NewLine}");
            }
            catch { }
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        [McpServerTool, Description("Opens Notepad, types the given text, takes a screenshot and returns it as a base64 string.")]
        public static string OpenNotepadAndType(
            [Description("The text to type into Notepad")] string text)
        {
            Log($"Tool called with text: {text}");
            try
            {
                Process.Start(new ProcessStartInfo("notepad.exe") { UseShellExecute = true });
                Log("Notepad launched");
                Thread.Sleep(2000);

                Process notepadProcess = null;
                IntPtr savedHandle = IntPtr.Zero;

                foreach (var p in Process.GetProcesses())
                {
                    try
                    {
                        IntPtr h = p.MainWindowHandle;
                        if (p.ProcessName.ToLower().Contains("notepad") && h != IntPtr.Zero)
                        {
                            notepadProcess = p;
                            savedHandle = h;
                            Log($"Found Notepad process: {p.Id} handle: {h}");
                            break;
                        }
                    }
                    catch { }
                }

                if (notepadProcess == null || savedHandle == IntPtr.Zero)
                {
                    Log("Error: Could not find Notepad process with a window.");
                    return "Error: Could not find Notepad process with a window.";
                }

                using var automation = new UIA3Automation();
                var app = FlaUI.Core.Application.Attach(notepadProcess);
                Log("Attached to Notepad");
                var window = app.GetMainWindow(automation);
                Log("Got main window");

                ShowWindow(savedHandle, SW_RESTORE);
                SetForegroundWindow(savedHandle);
                Thread.Sleep(500);

                var bounds = window.BoundingRectangle;
                Log($"Window bounds: {bounds}");
                FlaUI.Core.Input.Mouse.Click(new System.Drawing.Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2));
                Log("Clicked center of window");
                Thread.Sleep(300);

                try
                {
                    Log("Typing text...");
                    System.Windows.Forms.SendKeys.SendWait(text);
                    Log("Text typed successfully");
                }
                catch (Exception ex)
                {
                    Log($"Error typing: {ex.Message}\n{ex.StackTrace}");
                    return $"Error typing: {ex.Message} | {ex.StackTrace}";
                }

                Thread.Sleep(800);

                using var bitmap = new Bitmap(bounds.Width, bounds.Height);
                using var graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, new Size(bounds.Width, bounds.Height));
                Log("Screenshot taken");

                using var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                string base64 = Convert.ToBase64String(ms.ToArray());
                Log($"Returning base64 of length: {base64.Length}");
                return base64;
            }
            catch (Exception ex)
            {
                Log($"Unhandled error: {ex.Message}\n{ex.StackTrace}");
                return $"Error: {ex.Message} | {ex.StackTrace}";
            }
            finally
            {
                try
                {
                    foreach (var p in Process.GetProcesses())
                    {
                        try
                        {
                            if (p.ProcessName.ToLower().Contains("notepad"))
                            {
                                p.Kill();
                                Log("Notepad killed");
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }
    }
}