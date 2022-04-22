using Cygnus2_0.General;
using Cygnus2_0.Security;
using Cygnus2_0.ViewModel.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.IO;

namespace Cygnus2_0
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            pCargarDlls();

            //Create a custom principal with an anonymous identity at startup
            CustomPrincipal customPrincipal = new CustomPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);
            customPrincipal.Identity = new CustomIdentity("Anonimo", "", 0);
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("es-CO");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es-ES");
        }

        private void pCargarDlls()
        {
            string file;
            string[] dlls = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            string nombre;
            int nuIndex;
            string[] carpetas = { "x86", "x64"};
            string libSobreescribir = "System.Data.SQLite.dll";
            string path;

            for (int i = 0; i < dlls.Length; i++)
            {
                path = Environment.CurrentDirectory;
                nombre = dlls[i];

                if (nombre.IndexOf("Libs") > 0) //(nombre.EndsWith(".dll") || nombre.EndsWith(".config") || nombre.EndsWith(".exe") || nombre.Equals("od.txt"))
                {
                    nuIndex = nombre.IndexOf("Libs") + 5;

                    nombre = nombre.Substring(nuIndex, nombre.Length - nuIndex);

                    for (int j = 0; j < carpetas.Length; j++)
                    {
                        if (nombre.IndexOf(carpetas[j]) >= 0)
                        {
                            path = Environment.CurrentDirectory;

                            nuIndex = nombre.IndexOf(carpetas[j]) + carpetas[j].Length+1;
                            nombre = nombre.Substring(nuIndex, nombre.Length - nuIndex);
                            path = Path.Combine(path,carpetas[j]);

                            if (!Directory.Exists(path))
                            {
                                // Try to create the directory.
                                DirectoryInfo di = Directory.CreateDirectory(path);
                            }
                        }
                    }

                    file = System.IO.Path.Combine(path, nombre);

                    if (!File.Exists((string)file) || nombre.Equals(libSobreescribir))
                    {
                        CopyResource(dlls[i], file);
                    }
                }
            }
        }

        private void CopyResource(string resourceName, string file)
        {          
            using (Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                {
                    throw new ArgumentException("No such resource", "resourceName");
                }

                using (Stream output = File.OpenWrite(file))
                {
                    resource.CopyTo(output);
                }
            }
        }
    }

    /*public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            //Create a custom principal with an anonymous identity at startup
            CustomPrincipal customPrincipal = new CustomPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

            base.OnStartup(e);

            //Show the login view
            AuthenticationViewModel viewModel = new AuthenticationViewModel(new AuthenticationService());
            IView loginWindow = new LoginWindow(viewModel);
            loginWindow.Show();
        }
    }*/
}
