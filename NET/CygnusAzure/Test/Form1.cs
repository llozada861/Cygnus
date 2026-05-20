using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string xmlPath = @"D:\Dirtrab\Repositorios\Cygnus\NET\CygnusAzure\Cygnus2-0\Plantillas\EPM\Test.xml";
            string xsltPath = @"D:\Dirtrab\Repositorios\Cygnus\NET\CygnusAzure\Cygnus2-0\Plantillas\EPM\XSL_html.xslt";
            string outputHtml = @"D:\resultado.html";
            string xmlNormalizado = @"D:\archivo_normalizado.xml";

            XDocument doc = XDocument.Load(xmlPath);

            Normalize(doc.Root);

            doc.Save(@"D:\archivo_normalizado.xml");

            XslCompiledTransform transform = new XslCompiledTransform();

            // Cargar XSLT
            transform.Load(xsltPath);

            // Transformar XML -> HTML
            transform.Transform(xmlNormalizado, outputHtml);

            Console.WriteLine("HTML generado correctamente");
            
        }

        static void Normalize(XElement element)
        {
            // Normalizar nombre nodo
            element.Name = NormalizeName(element.Name.LocalName);

            // Normalizar atributos
            var attrs = element.Attributes().ToList();

            element.RemoveAttributes();

            foreach (var attr in attrs)
            {
                element.SetAttributeValue(
                    NormalizeName(attr.Name.LocalName),
                    attr.Value
                );
            }

            // Hijos
            foreach (var child in element.Elements())
            {
                Normalize(child);
            }
        }

        static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            return name.ToLower();
        }
    }
}
