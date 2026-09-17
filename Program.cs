using System;
using System.Windows.Forms;
using Pongspiel; // oder den korrekten Namespace deiner Form1-Datei


namespace PongGame
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}