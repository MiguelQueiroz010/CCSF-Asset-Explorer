using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CCSF_Asset_Explorer
{
    static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
        }

        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }
        /// <summary>
        /// Returns an icon representation of an image that is contained in the specified file.
        /// </summary>
        /// <param name="executablePath"></param>
        /// <returns></returns>
        public static Icon ExtractIconFromFilePath(string executablePath)
        {
            if (!System.IO.File.Exists(executablePath))
                return null;
            Icon result = (Icon)null;

            try
            {
                result = Icon.ExtractAssociatedIcon(executablePath);
            }
            catch (Exception)
            {
                //Console.WriteLine("Unable to extract the icon from the binary");
            }

            return result;
        }
        public static void OpenWithProgram(string path, string Program = "explorer")
        {
            Process fileopener = new Process();

            fileopener.StartInfo.FileName = Program;
            if(Program=="explorer")
                fileopener.StartInfo.Arguments = "\"" + path + "\"";
            else
                fileopener.StartInfo.Arguments = path;
            fileopener.Start();
           //fileopener.Close();
        }
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.WriteLine("Iniciando...");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Principal());
            Console.WriteLine("Saindo...");
        }
    }
}
