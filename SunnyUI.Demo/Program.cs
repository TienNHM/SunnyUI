using System;
using System.Windows.Forms;

namespace Sunny.UI.Demo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Add a built-in resource configuration for Traditional Chinese
            UIStyles.BuiltInResources.TryAdd(CultureInfos.en_US.LCID, new en_US_Resources());
            //UIStyles.BuiltInResources.TryAdd(CultureInfos.zh_TW.LCID, new zh_TW_Resources());

            // Generate multi-language configuration files from the project solution, files are generated in the Language directory under the executable folder
            // After generation, if the interface has not been modified, you can comment out the next line, only run once
            //TranslateHelper.LoadCsproj(@"D:\repos\SunnyUI\SunnyUI.Demo\SunnyUI.Demo.csproj");

            Application.Run(new FMain());
        }
    }
}
