using System;
using CefSharp.RenderProcess;
using CefSharp.Internals;
using CefSharp.BrowserSubprocess;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace gesobrowser
{
    public class Subprocess : IDisposable
    {
        public String parentProcessId { get; set; }
        public Subprocess(IEnumerable<string> args)
        {
            parentProcessId = args.FirstOrDefault((string a) => a.StartsWith("--host-process-id"))?.Split('=').Last();
        }
        public void Dispose()
        {
        }

        public void Submain(string[] args)
        {
            if (parentProcessId != null)
            {
                string AppPath = System.Windows.Forms.Application.ExecutablePath;
                string AppDir = System.IO.Path.GetDirectoryName(AppPath);
                Assembly.LoadFrom(Path.Combine(AppDir, "x64", "CefSharp.BrowserSubprocess.Core.dll"));
                var browserProcessExe = new WcfBrowserSubprocessExecutable();
                SubProcess.EnableHighDPISupport();
                IRenderProcessHandler handler = null;
                browserProcessExe.Main(args, handler);
            }
        }
    }
}

