/* 
 * Adopted from NSMBe's patch maker
 */ 

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace SM64DSe.Patcher
{
    class PatchCompiler
    {

        public static int compilePatch(uint destAddr, DirectoryInfo romDir)
        {
            return runProcess("make CODEADDR=0x" + destAddr.ToString("X8"), romDir.FullName);
        }

        public static int cleanPatch(DirectoryInfo romDir)
        {
            return runProcess("make clean", romDir.FullName);
        }

        public static int runProcess(string proc, string cwd)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd";
            info.Arguments = "/C " + proc + " || pause";
            info.CreateNoWindow = false;
            info.UseShellExecute = false;
            info.WorkingDirectory = cwd;

            Process p = Process.Start(info);
            p.WaitForExit();
            return p.ExitCode;
        }
    }
}
