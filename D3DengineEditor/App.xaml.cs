using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;

namespace D3DengineEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        public App()
        {
            AllocConsole();
            Console.WriteLine("Console is now available.");
        }
    }

}
