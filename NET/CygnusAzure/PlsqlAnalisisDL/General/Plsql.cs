using Antlr4.Runtime.Misc;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Collections;

namespace PlsqlAnalisisDL.General
{
    public static class Plsql
    {
        public static List<InstruccionPL> AnalizarPL(string archivo)
        {
            List<InstruccionPL> instruccionesPL = new List<InstruccionPL>();

            string sqlText = File.ReadAllText(@archivo, Encoding.GetEncoding("Windows-1252"));

            var inputStream = new Antlr4.Runtime.AntlrInputStream(sqlText);

            var lexer = new PlSqlLexer(inputStream);

            /*lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new OracleErrorListener());*/

            var tokenStream = new Antlr4.Runtime.CommonTokenStream((Antlr4.Runtime.ITokenSource)lexer);

            /*tokenStream.Fill();

            foreach (var token in tokenStream.GetTokens())
            {
                Console.WriteLine(
                    $"{token.Type} -> {token.Text}");
            }*/

            var parser = new PlSqlParser(tokenStream);

            parser.RemoveErrorListeners();
            OracleErrorListener errorListener = new OracleErrorListener();
            parser.AddErrorListener(errorListener);

            var tree = parser.sql_script();

            foreach (var dep in errorListener.InstruccionList)
            {
                Console.WriteLine(
                    $"{dep.Token} -> {dep.Valor}");

                instruccionesPL.Add(dep);
            }

            var listener = new TableListener(tokenStream);

            ParseTreeWalker.Default.Walk(listener, tree);

            foreach (var dep in listener.InstruccionList.GroupBy(x => x.Token + "|" + x.Valor).Select(x => x.First()).ToList().OrderBy(x => x.Token))
            {
                Console.WriteLine(
                    $"{dep.Token} -> {dep.Valor} : [{dep.NombreObjeto}] - [{dep.NombreObjeto}]");

                instruccionesPL.Add(dep);
            }

            //comentarios
            var listenerComm = new MethodCommentListener(tokenStream);

            ParseTreeWalker.Default.Walk(listenerComm, tree);

            foreach (var dep in listenerComm.Comentarios)
            {
                Console.WriteLine(
                    $"{dep.Token}:{dep.NombreObjeto} -> {dep.Valor}");

                instruccionesPL.Add(dep);
            }

            foreach (var dep in listenerComm.Comentarios2.GroupBy(x => x.Token + "|" + x.Valor).Select(x => x.First()).ToList().OrderBy(x => x.Token))
            {
                Console.WriteLine(
                    $"{dep.Token}:{dep.NombreObjeto} -> {dep.Valor}");

                instruccionesPL.Add(dep);
            }

            return instruccionesPL;
        }
    }

    public class TableListener : PlSqlParserBaseListener
    {
        public List<InstruccionPL> InstruccionList { get; } = new List<InstruccionPL>();

        private readonly CommonTokenStream _tokens;

        public TableListener(CommonTokenStream tokens)
        {
            _tokens = tokens;
        }

        // TABLAS / VISTAS
        public override void EnterTableview_name([NotNull] PlSqlParser.Tableview_nameContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TABLE_OR_VIEW",
                Valor = context.GetText()
            });
        }

        // LLAMADOS A PACKAGE.PROCEDURE
        public override void EnterGeneral_element([NotNull] PlSqlParser.General_elementContext context)
        {
            string text = context.GetText();

            var match = Regex.Match(
                text,
                @"^([A-Z0-9_]+)\.([A-Z0-9_]+)\s*\(",
                RegexOptions.IgnoreCase);

            if (match.Success)
            {
                InstruccionList.Add(new InstruccionPL
                {
                    Token = "PACKAGE_CALL",
                    Valor =
                        $"{match.Groups[1].Value}"
                });
            }

            /*
             * $"{match.Groups[1].Value}." +
                        $"{match.Groups[2].Value}"
             * if (text.Contains("."))
            {
                Dependencias.Add(new Dependencia
                {
                    Token = "PACKAGE_CALL",
                    Valor = text
                });
            }*/
        }

        public override void EnterVariable_declaration(PlSqlParser.Variable_declarationContext context)
        {
            string text = _tokens.GetText(context.SourceInterval);

            var match = Regex.Match(
                text,
                @"([A-Z0-9_]+)\.([A-Z0-9_]+)%TYPE",
                RegexOptions.IgnoreCase);

            if (match.Success)
            {
                InstruccionList.Add(new InstruccionPL
                {
                    Token = "VARIABLE_TABLE",
                    Valor = match.Groups[1].Value
                });
            }
        }

        public override void EnterTable_ref_aux(PlSqlParser.Table_ref_auxContext context)
        {
            var table = context.table_ref_aux_internal()?.GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TBL_SEL_FROM",
                Valor = table
            });
        }

        public override void EnterParameter(PlSqlParser.ParameterContext context)
        {
            string text =
                _tokens.GetText(
                    context.SourceInterval);

            var match = Regex.Match(
                text,
                @"([A-Z0-9_]+)\.([A-Z0-9_]+)%TYPE",
                RegexOptions.IgnoreCase);

            if (match.Success)
            {
                InstruccionList.Add(new InstruccionPL
                {
                    Token = "PARAM_TABLE",
                    Valor = match.Groups[1].Value
                });
            }
        }

        public override void EnterInsert_into_clause(PlSqlParser.Insert_into_clauseContext context)
        {
            var table = context.general_table_ref().dml_table_expression_clause().tableview_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "INSERT",
                NombreObjeto = table
            });
        }

        public override void EnterUpdate_statement(PlSqlParser.Update_statementContext context)
        {
            var table = context.general_table_ref().dml_table_expression_clause().tableview_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "UPDATE",
                NombreObjeto = table
            });
        }

        public override void EnterDelete_statement(PlSqlParser.Delete_statementContext context)
        {
            var table = context.general_table_ref().dml_table_expression_clause().tableview_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DELETE",
                NombreObjeto = table
            });
        }

        public override void EnterMerge_statement([NotNull] PlSqlParser.Merge_statementContext context)
        {
            var table = context.selected_tableview()[0].tableview_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "MERGE",
                NombreObjeto = table
            });
        }

        public override void EnterExecute_immediate([NotNull] PlSqlParser.Execute_immediateContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DYNAMIC_SQL",
                NombreObjeto = _tokens.GetText(context.SourceInterval)
            });
        }

        public override void EnterCreate_package(PlSqlParser.Create_packageContext context)
        {
            var objeto = context.package_name().First().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "PAQUETE",
                NombreObjeto = objeto
            });
        }

        public override void EnterCreate_package_body([NotNull] PlSqlParser.Create_package_bodyContext context)
        {
            var objeto = context.package_name().First().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "PAQUETE",
                NombreObjeto= objeto
            });
        }
        public override void EnterCreate_procedure_body([NotNull] PlSqlParser.Create_procedure_bodyContext context)
        {
            var objeto = context.procedure_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "PROCEDIMIENTO",
                NombreObjeto= objeto
            });
        }

        public override void EnterCreate_function_body([NotNull] PlSqlParser.Create_function_bodyContext context)
        {
            var objeto = context.function_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "FUNCION",
                NombreObjeto= objeto
            });
        }

        public override void EnterCreate_index([NotNull] PlSqlParser.Create_indexContext context)
        {
            var objeto = context.index_name().GetText();
            var tabla = context.table_index_clause().tableview_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "INDICE",
                NombreObjeto= tabla
            });
        }

        public override void EnterCreate_directory([NotNull] PlSqlParser.Create_directoryContext context)
        {
            var objeto = context.directory_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DIRECTORIO",
                NombreObjeto= objeto
            });
        }

        public override void EnterCreate_materialized_view([NotNull] PlSqlParser.Create_materialized_viewContext context)
        {
            var objeto = context.tableview_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "VISTA_MAT",
                NombreObjeto = objeto
            });
        }

        public override void EnterCreate_sequence([NotNull] PlSqlParser.Create_sequenceContext context)
        {
            var objeto = context.sequence_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "SECUENCIA",
                NombreObjeto= objeto
            });
        }

        public override void EnterCreate_synonym([NotNull] PlSqlParser.Create_synonymContext context)
        {
            var objeto = context.synonym_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "SINONIMO",
                NombreObjeto= objeto
            });
        }

        public override void EnterCreate_trigger([NotNull] PlSqlParser.Create_triggerContext context)
        {
            var objeto = context.trigger_name().GetText();
            var tabla = context.simple_dml_trigger().dml_event_clause().tableview_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "TRIGGER",
                NombreObjeto= tabla
            });
        }

        public override void EnterCreate_type([NotNull] PlSqlParser.Create_typeContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "TYPE"
            });
        }

        public override void EnterCreate_table([NotNull] PlSqlParser.Create_tableContext context)
        {
            var objeto = context.table_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "TABLA",
                NombreObjeto = objeto
            });
        }

        public override void EnterCreate_tablespace([NotNull] PlSqlParser.Create_tablespaceContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "TABLESPACE"
            });
        }

        public override void EnterCreate_user([NotNull] PlSqlParser.Create_userContext context)
        {
            var objeto = context.user_object_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "USUARIO",
                NombreObjeto= objeto
            });
        }

        public override void EnterCreate_view([NotNull] PlSqlParser.Create_viewContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "VISTA"
            });
        }

        public override void EnterCreate_mv_refresh([NotNull] PlSqlParser.Create_mv_refreshContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "VISTA"
            });
        }

        public override void EnterAlter_table([NotNull] PlSqlParser.Alter_tableContext context)
        {
            var objeto = context.tableview_name().GetText();

            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "ALTER",
                NombreObjeto = objeto
            });
        }

        /*public override void EnterForeign_key_clause([NotNull] PlSqlParser.Foreign_key_clauseContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "LLAVE_FORANEA"
            });
        }*/

        public override void EnterEveryRule(ParserRuleContext context)
        {
            Console.WriteLine(
                context.GetType().Name);
        }

        public override void EnterOut_of_line_constraint([NotNull] PlSqlParser.Out_of_line_constraintContext context)
        {
            string text = _tokens.GetText(context.SourceInterval);
            var tabla = "";

            RuleContext parent = context.Parent;

            while (parent != null)
            {
                if (parent is PlSqlParser.Alter_tableContext createTable)
                {
                    tabla = createTable.tableview_name().GetText();
                    break;
                }

                parent =
                    parent.Parent;
            }

            if (text.Contains("PRIMARY KEY"))
            {
                InstruccionList.Add(new InstruccionPL
                {
                    Token = "TIPO",
                    Valor = "LLAVE_PRIMARIA",
                    NombreObjeto = tabla
                });
            }

            if (text.Contains("CHECK"))
            {
                InstruccionList.Add(new InstruccionPL
                {
                    Token = "TIPO",
                    Valor = "LLAVE_UNICA",
                    NombreObjeto = tabla
                });
            }

            if (text.Contains("UNIQUE"))
            {
                InstruccionList.Add(new InstruccionPL
                {
                    Token = "TIPO",
                    Valor = "LLAVE_UNICA",
                    NombreObjeto = tabla
                });
            }

            if (text.Contains("FOREIGN"))
            {
                InstruccionList.Add(new InstruccionPL
                {
                    Token = "TIPO",
                    Valor = "LLAVE_FORANEA",
                    NombreObjeto = tabla
                });
            }
        }

        public override void EnterGrant_statement([NotNull] PlSqlParser.Grant_statementContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "GRANT"
            });
        }

        public override void EnterAnonymous_block([NotNull] PlSqlParser.Anonymous_blockContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "SCRIPT"
            });
        }

        public override void EnterCreate_java([NotNull] PlSqlParser.Create_javaContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "JAVA"
            });
        }

        public override void EnterDrop_directory([NotNull] PlSqlParser.Drop_directoryContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DROP"
            });
        }

        public override void EnterDrop_constraint_clause([NotNull] PlSqlParser.Drop_constraint_clauseContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DROP",
                NombreObjeto = context.constraint_name().GetText()
            });
        }

        public override void EnterDrop_column_clause([NotNull] PlSqlParser.Drop_column_clauseContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DROP",
                NombreObjeto = context.column_name()[0].GetText()
            });
        }

        public override void EnterDrop_materialized_view([NotNull] PlSqlParser.Drop_materialized_viewContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DROP",
                NombreObjeto = context.tableview_name().GetText()
            });
        }

        public override void EnterDrop_index([NotNull] PlSqlParser.Drop_indexContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DROP",
                NombreObjeto = context.index_name().GetText()
            });
        }

        public override void EnterDrop_package([NotNull] PlSqlParser.Drop_packageContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DROP",
                NombreObjeto = context.package_name().GetText()
            });
        }

        public override void EnterDrop_synonym([NotNull] PlSqlParser.Drop_synonymContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DROP",
                NombreObjeto = context.synonym_name().GetText()
            });
        }

        public override void EnterDrop_procedure([NotNull] PlSqlParser.Drop_procedureContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DROP",
                NombreObjeto= context.procedure_name().GetText()
            });
        }

        public override void EnterDrop_table([NotNull] PlSqlParser.Drop_tableContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DROP",
                NombreObjeto = context.tableview_name()[0].GetText()
            });
        }

        public override void EnterDrop_sequence([NotNull] PlSqlParser.Drop_sequenceContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DROP",
                NombreObjeto = context.sequence_name().GetText()
            });
        }

        public override void EnterDrop_trigger([NotNull] PlSqlParser.Drop_triggerContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DROP",
                NombreObjeto = context.trigger_name().GetText()
            });
        }

        public override void EnterDrop_view([NotNull] PlSqlParser.Drop_viewContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "DROP",
                NombreObjeto = context.tableview_name().GetText()
            });
        }

        public override void EnterSql_plus_command([NotNull] PlSqlParser.Sql_plus_commandContext context)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "TIPO",
                Valor = "APLICA"
            });
        }
    }

    public class MethodCommentListener : PlSqlParserBaseListener
    {
        private readonly CommonTokenStream _tokens;
        public List<InstruccionPL> Comentarios { get; } = new List<InstruccionPL>();
        public List<InstruccionPL> Comentarios2 { get; } = new List<InstruccionPL>();

        public MethodCommentListener(CommonTokenStream tokens)
        {
            _tokens = tokens;
        }

        public override void EnterCreate_trigger([NotNull] PlSqlParser.Create_triggerContext context)
        {
            var comentarios_ = new HashSet<string>();

            int start =
                context.Start.TokenIndex;

            int stop =
                context.Stop.TokenIndex;

            for (int i = start; i <= stop; i++)
            {
                var hidden =
                    _tokens.GetHiddenTokensToLeft(i);

                if (hidden == null)
                    continue;

                foreach (var token in hidden)
                {
                    string text =
                        token.Text.Trim();

                    if (token.Text.StartsWith("/*") && comentarios_.Add(text))
                    {
                        Comentarios.Add(new InstruccionPL { NombreObjeto = context.trigger_name().GetText(), Token = "COMMENT_OUT", Valor = token.Text });
                    }

                    if (token.Text.StartsWith("--") && comentarios_.Add(text))
                    {
                        Comentarios2.Add(new InstruccionPL { NombreObjeto = context.trigger_name().GetText(), Token = "COMMENT_IN", Valor = token.Text });
                    }
                }
            }
        }

        public override void EnterCreate_procedure_body([NotNull] PlSqlParser.Create_procedure_bodyContext context)
        {
            var comentarios_ = new HashSet<string>();

            int start = context.Start.TokenIndex;

            int stop = context.Stop.TokenIndex;

            string nombreObjeto = context.procedure_name().GetText();

            for (int i = start; i <= stop; i++)
            {
                var hidden = _tokens.GetHiddenTokensToLeft(i);

                if (hidden == null)
                    continue;

                foreach (var token in hidden)
                {
                    string text = nombreObjeto+"-"+token.Text.Trim();

                    if (token.Text.StartsWith("/*") && comentarios_.Add(text))
                    {
                        Comentarios.Add(new InstruccionPL { NombreObjeto = nombreObjeto, Token = "COMMENT_OUT", Valor = token.Text });
                    }

                    if (token.Text.StartsWith("--") && comentarios_.Add(text))
                    {
                        Comentarios2.Add(new InstruccionPL { NombreObjeto = nombreObjeto, Token = "COMMENT_IN", Valor = token.Text });
                    }
                }
            }
        }

        public override void EnterCreate_function_body([NotNull] PlSqlParser.Create_function_bodyContext context)
        {
            var comentarios_ = new HashSet<string>();

            int start = context.Start.TokenIndex;

            int stop = context.Stop.TokenIndex;

            string nombreObjeto = context.function_name().GetText();

            for (int i = start; i <= stop; i++)
            {
                var hidden =
                    _tokens.GetHiddenTokensToLeft(i);

                if (hidden == null)
                    continue;

                foreach (var token in hidden)
                {
                    string text = nombreObjeto+"-"+token.Text.Trim();

                    if (token.Text.StartsWith("/*") && comentarios_.Add(text))
                    {
                        Comentarios.Add(new InstruccionPL { NombreObjeto = nombreObjeto, Token = "COMMENT_OUT", Valor = token.Text });
                    }

                    if (token.Text.StartsWith("--") && comentarios_.Add(text))
                    {
                        Comentarios2.Add(new InstruccionPL { NombreObjeto = nombreObjeto, Token = "COMMENT_IN", Valor = token.Text });
                    }
                }
            }
        }

        public override void EnterPackage_obj_spec([NotNull] PlSqlParser.Package_obj_specContext context)
        {
            var startToken = context.Start;

            var hiddenTokens = _tokens.GetHiddenTokensToLeft(startToken.TokenIndex);

            string metodo = "";

            var createPackage = context.Parent as PlSqlParser.Create_packageContext;

            if (createPackage != null)
            {
                metodo = createPackage.package_name().Last().GetText();
            }

            if (hiddenTokens != null)
            {
                foreach (var token in hiddenTokens)
                {
                    if (token.Text.StartsWith("/*"))
                    {
                        Comentarios.Add(new InstruccionPL { NombreObjeto = metodo, Token = "COMMENT_PKG", Valor = token.Text });
                    }
                }
            }
        }

        public override void EnterProcedure_body(PlSqlParser.Procedure_bodyContext context)
        {
            var startToken = context.Start;
            int start = context.Start.TokenIndex;
            int stop = context.Stop.TokenIndex;

            var hiddenTokens = _tokens.GetHiddenTokensToLeft(startToken.TokenIndex);

            string metodo = context.GetChild(1).GetText();

            if (hiddenTokens != null)
            {
                foreach (var token in hiddenTokens)
                {
                    if (token.Text.StartsWith("/*"))
                    {
                        Comentarios.Add(new InstruccionPL { NombreObjeto = metodo, Token = "COMMENT_OUT", Valor = token.Text });
                    }
                }
            }            

            for (int i = start; i <= stop; i++)
            {
                var hidden = _tokens.GetHiddenTokensToLeft(i);

                if (hidden == null)
                    continue;

                foreach (var token in hidden)
                {
                    if (token.Text.StartsWith("--"))
                    {
                        Comentarios2.Add(new InstruccionPL { NombreObjeto = metodo, Token = "COMMENT_IN", Valor = token.Text });
                    }
                }
            }
        }

        public override void EnterFunction_body(PlSqlParser.Function_bodyContext context)
        {
            var startToken = context.Start;
            int start = context.Start.TokenIndex;
            int stop = context.Stop.TokenIndex;

            var hiddenTokens = _tokens.GetHiddenTokensToLeft(startToken.TokenIndex);

            string metodo = context.GetChild(1).GetText();

            if (hiddenTokens != null)
            {
                foreach (var token in hiddenTokens)
                {
                    if (token.Text.StartsWith("/*"))
                    {
                        Comentarios.Add(new InstruccionPL { NombreObjeto = metodo, Token = "COMMENT_OUT", Valor = token.Text });
                    }
                }
            }

            for (int i = start; i <= stop; i++)
            {
                var hidden = _tokens.GetHiddenTokensToLeft(i);

                if (hidden == null)
                    continue;

                foreach (var token in hidden)
                {
                    if (token.Text.StartsWith("--"))
                    {
                        Comentarios2.Add(new InstruccionPL { NombreObjeto = metodo, Token = "COMMENT_IN", Valor = token.Text });
                    }
                }
            }
        }
    }

    public class OracleErrorListener : BaseErrorListener
    {
        public List<InstruccionPL> InstruccionList { get; } = new List<InstruccionPL>();
        public override void SyntaxError(
            TextWriter output,
            IRecognizer recognizer,
            IToken offendingSymbol,
            int line,
            int charPositionInLine,
            string msg,
            RecognitionException e)
        {
            InstruccionList.Add(new InstruccionPL
            {
                Token = "ERROR",
                Valor = $"Línea {line}:{charPositionInLine} -> {msg}"
            });
        }
    }

    /*public override void EnterGeneral_element(
    PlSqlParser.General_elementContext context)
    {
        Console.WriteLine(context.GetText());
    }*/
}
