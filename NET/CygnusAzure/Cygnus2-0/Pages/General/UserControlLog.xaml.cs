using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cygnus2_0.Pages.General
{
    /// <summary>
    /// Lógica de interacción para UserControlLog.xaml
    /// </summary>
    public partial class UserControlLog : UserControl
    {
        public UserControlLog(StringBuilder salida)
        {
            InitializeComponent();
            richTextBoxResult.Document.Blocks.Clear();
            richTextBoxResult.Document.Blocks.Add(new Paragraph(new Run(salida.ToString())));
        }
    }
}
