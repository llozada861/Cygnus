using Cygnus2_0.General;
using Cygnus2_0.General.Documentacion;
using Cygnus2_0.Interface;
using Cygnus2_0.ViewModel.Documentation;
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
using res = Cygnus2_0.Properties.Resources;

namespace Cygnus2_0.Pages.Documentation
{
    /// <summary>
    /// Interaction logic for GenerateHtml.xaml
    /// </summary>
    public partial class GenerateHtml : UserControl
    {
        private Handler handler;        
        private GeneratHtmlViewModel view;
        private DocumentacionHTMLModel docModel;

        public GenerateHtml()
        {
            var myWin = (MainWindow)Application.Current.MainWindow;
            handler = myWin.Handler;            
            view = new GeneratHtmlViewModel(handler);
            DataContext = view;
            
            InitializeComponent();

            view.Model.Usuario = Cygnus2_0.Properties.Cygnus.Default.UserName; 
            docModel = new DocumentacionHTMLModel();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Boolean blParaDesc = false;
            StringBuilder parametrosBuilder = new StringBuilder();
            StringBuilder metodoBuilder = new StringBuilder();
            StringBuilder returnBuilder = new StringBuilder();
            int index;

            try
            {
                if (String.IsNullOrEmpty(view.Model.Descripcion))
                {
                    handler.MensajeError("Debe ingresar una descripción para el proceso.");
                    return;
                }

                if (String.IsNullOrEmpty(view.Model.Usuario))
                {
                    handler.MensajeError("Debe ingresar un usuario para el proceso.");
                    return;
                }

                if (String.IsNullOrEmpty(view.Model.Wo))
                {
                    handler.MensajeError("Debe ingresar la WO para el proceso.");
                    return;
                }

                if (view.Model.ListaParametros.Count > 0)
                {
                    foreach (ParametrosModel parametro in view.Model.ListaParametros)
                    {
                        if (String.IsNullOrEmpty(parametro.Descripcion))
                        {
                            blParaDesc = true;
                            break;
                        }
                    }

                    if (blParaDesc)
                    {
                        handler.MensajeError("Todos los parámetros deben tener descripción.");
                        return;
                    }
                }

                handler.CursorWait();

                docModel.Descripcion = view.Model.Descripcion;
                docModel.Autor = view.Model.Usuario;
                docModel.Fecha = DateTime.Now.ToShortDateString();

                if (docModel.Fuente != null && docModel.Fuente.ToLower().Equals(res.TipoObjetoPaquete.ToLower()))
                {
                    metodoBuilder.Append(handler.HtmlEspecificacion);
                    metodoBuilder.Replace(res.TagHtml_metodo, docModel.Unidad);
                    metodoBuilder.Replace(res.TagHTML_Desc, docModel.Descripcion);
                    metodoBuilder.Replace(res.TagHtml_autor, docModel.Autor);
                    metodoBuilder.Replace(res.TagHtml_desarrollador, Environment.UserName);
                    metodoBuilder.Replace(res.TagHtml_fecha, docModel.Fecha);
                    metodoBuilder.Replace(res.TagHtml_numeroOC, view.Model.Wo);
                }
                else
                {

                    metodoBuilder.Append(handler.HtmlMetodo);
                    metodoBuilder.Replace(res.TagHtml_metodo, docModel.Unidad);
                    metodoBuilder.Replace(res.TagHTML_Desc, docModel.Descripcion);
                    metodoBuilder.Replace(res.TagHtml_autor, docModel.Autor);
                    metodoBuilder.Replace(res.TagHtml_desarrollador, Environment.UserName);
                    metodoBuilder.Replace(res.TagHtml_fecha, docModel.Fecha);
                    metodoBuilder.Replace(res.TagHtml_numeroOC, view.Model.Wo);
                    metodoBuilder.AppendLine();

                    if (docModel.Parametros != null && docModel.Parametros.Count > 0)
                    {
                        foreach (ParametrosModel parameter in docModel.Parametros)
                        {
                            if (!parameter.Tipo.Equals(res.NameReturn) && !parameter.Tipo.Equals(res.DescReturn))
                            {
                                parametrosBuilder.Append(handler.HtmlMetodoParam);
                                parametrosBuilder.Replace(res.TagHtmlParamName, parameter.Nombre);
                                parametrosBuilder.Replace(res.TagHtmlParamType, parameter.Tipo);
                                parametrosBuilder.Replace(res.TagHtmlParamDir, parameter.Direccion);
                                parametrosBuilder.Replace(res.TagHtmlParaDesc, parameter.Descripcion);

                                index = docModel.Parametros.IndexOf(parameter);

                                /*if (index < docModel.Parametros.Count - 1)
                                {
                                    parametrosBuilder.AppendLine();
                                }*/
                            }
                            else
                            {
                                if (docModel.Retorno != null)
                                {
                                    if (parameter.Tipo.Equals(res.NameReturn))
                                        docModel.Retorno.Nombre = parameter.Descripcion;

                                    if (parameter.Tipo.Equals(res.DescReturn))
                                        docModel.Retorno.Descripcion = parameter.Descripcion;
                                }
                            }
                        }

                        metodoBuilder.Replace(res.TagHTML_parametro, parametrosBuilder.ToString());
                    }
                    else
                    {
                        metodoBuilder.Replace(res.TagHTML_parametro, "");
                    }

                    if (docModel.Retorno != null)
                    {
                        returnBuilder.Append(handler.HtmlMetodoReturn);
                        returnBuilder.Replace(res.TagHtmlTipoRetorno, docModel.Retorno.Tipo);
                        returnBuilder.Replace(res.TagHTML_nombre, docModel.Retorno.Nombre);
                        returnBuilder.Replace(res.TagHTML_Desc, docModel.Retorno.Descripcion);
                        metodoBuilder.Replace(res.TagHtmlRetorno, returnBuilder.ToString());
                    }
                    else
                    {
                        metodoBuilder.Replace(res.TagHtmlRetorno, "");
                    }
                }

                richTextBoxResult.Document.Blocks.Clear();
                richTextBoxResult.Document.Blocks.Add(new Paragraph(new Run(metodoBuilder.ToString())));
                Cygnus2_0.Properties.Cygnus.Default.UserName = view.Model.Usuario;
                Cygnus2_0.Properties.Cygnus.Default.Save();

                handler.CursorNormal();
            }
            catch(Exception ex)
            {
                handler.CursorNormal();
                handler.MensajeError(ex.Message);
            }
        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string richText = new TextRange(richTextBoxProce.Document.ContentStart, richTextBoxProce.Document.ContentEnd).Text;
            String parametros = "";
            String head = "";
            String foot = "";
            docModel = new DocumentacionHTMLModel();

            string[] parametrosList;
            string[] encabezado;
            string[] endDoc;
            string paramdesc;
            string paramdirection;
            string paramtype;
            Boolean blPackage = false;
            int nuIndexType=1;

            //Clean the list view
            view.Model.ListaParametros.Clear();

            if (richTextBoxResult != null && richTextBoxResult.Document.Blocks.Count > 0)
            {
                richTextBoxResult.Document.Blocks.Clear();
            }

            richText = System.Text.RegularExpressions.Regex.Replace(richText, @"\r\n+", " ");
            richText = System.Text.RegularExpressions.Regex.Replace(richText, @"\s+", " ");
            richText = System.Text.RegularExpressions.Regex.Replace(richText, @";", "");

            try
            {
                if (!String.IsNullOrEmpty(richText.Trim()))
                {
                    //if not found character (, that mean than the procedure doesn't have parameters
                    if (richText.IndexOf('(') >= 0)
                    {
                        head = richText.Substring(0, richText.IndexOf('('));
                        parametros = richText.Substring(richText.IndexOf('(') + 1, (richText.IndexOf(')') - richText.IndexOf('(')) - 1);
                        foot = richText.Substring(richText.IndexOf(')')+1, (richText.Length - richText.IndexOf(')'))-1);
                    }

                    if (richText.ToLower().IndexOf(res.procedure.ToLower()) >= 0 || richText.ToLower().IndexOf(res.function.ToLower()) >= 0)
                    {
                        if (!String.IsNullOrEmpty(parametros))
                        {
                            parametrosList = parametros.Trim().Split(',');

                            foreach (string param in parametrosList)
                            {
                                string[] descparam = param.Trim().Split(' ');

                                paramdesc = "";
                                paramdirection = res.IN;
                                paramtype = "";
                                nuIndexType = 1;

                                if (descparam.Length > 0)
                                {
                                    //Position ZERO always is the name of parameter
                                    paramdesc = descparam[0];

                                    //If position one is IN or OUT, that indicate than is the position
                                    if (descparam[1].ToUpper().Equals(res.IN) || descparam[1].ToUpper().Equals(res.OUT))
                                    {
                                        paramdirection = descparam[1].ToUpper();
                                        nuIndexType = 2;

                                        if (descparam.Length > 2)
                                        {
                                            //If position one is IN or OUT, that indicate than is the position
                                            if (descparam[2].ToUpper().Equals(res.IN) || descparam[2].ToUpper().Equals(res.OUT))
                                            {
                                                paramdirection = paramdirection + " " + descparam[2].ToUpper();
                                                nuIndexType = 3;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        paramtype = descparam[nuIndexType];
                                    }

                                    if (String.IsNullOrEmpty(paramtype))
                                    {
                                        paramtype = descparam[nuIndexType];
                                    }

                                    view.Model.ListaParametros.Add(new ParametrosModel { Nombre = paramdesc, Tipo = paramtype, Direccion = paramdirection, Descripcion = "" });
                                }
                            }

                            encabezado = head.Trim().Split(' ');
                            endDoc = foot.Trim().Split(' ');

                            docModel.Unidad = encabezado[1];

                            if (encabezado[0].ToLower().Equals(res.function))
                            {
                                docModel.Retorno = new RetornoModel();
                                docModel.Retorno.Tipo = endDoc[1];
                                view.Model.ListaParametros.Add(new ParametrosModel { Nombre = "Var. Retorno", Tipo = res.NameReturn, Direccion = null, Descripcion = "" });
                                view.Model.ListaParametros.Add(new ParametrosModel { Nombre = "Desc. Retorno", Tipo = res.DescReturn, Direccion = null, Descripcion = "" });
                            }

                            docModel.Parametros = view.Model.ListaParametros.ToList<ParametrosModel>();
                        }
                        else
                        {
                            encabezado = richText.Trim().Split(' ');

                            docModel.Unidad = encabezado[1];

                            if (encabezado[0].ToLower().Equals(res.function))
                            {
                                docModel.Retorno = new RetornoModel();
                                docModel.Retorno.Tipo = encabezado[3];
                                view.Model.ListaParametros.Add(new ParametrosModel { Nombre = "Var. Retorno", Tipo = res.NameReturn, Direccion = null, Descripcion = "" });
                                view.Model.ListaParametros.Add(new ParametrosModel { Nombre = "Desc. Retorno", Tipo = res.DescReturn, Direccion = null, Descripcion = "" });
                            }

                            docModel.Parametros = view.Model.ListaParametros.ToList<ParametrosModel>();
                        }
                    }
                    else
                    {
                        string[] namePackage = richText.Trim().Split(' ');

                        for(int i=0;i< namePackage.Length;i++)
                        {
                            if(blPackage)
                            {
                                docModel.Unidad = namePackage[i];
                                docModel.Fuente = res.TipoObjetoPaquete;
                                break;
                            }

                            if (namePackage[i].ToLower().Equals(res.package))
                            {
                                blPackage = true;
                            }
                        }                                                
                    }                                      
                }
            }
            catch
            {
            }                     
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            view.Model.Descripcion = "";
            view.Model.Wo = "";
            view.Model.Usuario = Cygnus2_0.Properties.Cygnus.Default.UserName;
            richTextBoxResult.Document.Blocks.Clear();
            richTextBoxProce.Document.Blocks.Clear();
        }
    }
}
