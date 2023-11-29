using Cygnus2_0.General.Documentacion;
using Cygnus2_0.Model.Conexion;
using Cygnus2_0.Model.Empresa;
using Cygnus2_0.Model.History;
using Cygnus2_0.Model.Html;
using Cygnus2_0.Model.Objects;
using Cygnus2_0.Model.Permisos;
using Cygnus2_0.Model.Repository;
using Cygnus2_0.Model.Settings;
using Cygnus2_0.Model.User;
using Cygnus2_0.Model.Version;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.BaseDatos.sqlite
{
    public class DataBaseContext: DbContext
    {
        private const string DBName = "Cygnus.db";

        public DataBaseContext() :
            base(new SQLiteConnection()
            {
                ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = DBName, ForeignKeys = true }.ConnectionString
            }, true)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public static void test()
        {
            using (var ctx = GetInstance())
            {
                var query = "select * from paths";

                using (var command = new SQLiteCommand(query, ctx))
                {
                    SQLiteDataReader reader = command.ExecuteReader();
                }
            }
        }

        public static SQLiteConnection GetInstance()
        {
            var db = new SQLiteConnection(
                string.Format("Data Source={0};Version=3;", DBName)
            );
            db.Open();

            return db;
        }

        public DbSet<Repositorio> Repositorios { get; set; }
        public DbSet<RamaRepositorio> RamaRepositorios { get; set; }
        public DbSet<Configuracion> Configuraciones { get; set; }
        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<EmpresaModel> Empresas { get; set; }
        public DbSet<VersionBD> Versiones { get; set; }
        public DbSet<PalabrasClaves> PalabrasReservadas { get; set; }
        public DbSet<GrantsModel> UsuariosGrant { get; set; }
        public DbSet<TipoObjetos> TipoObjetos { get; set; }
        public DbSet<RutaObjetos> RutaObjetos { get; set; }
        public DbSet<HeadModel> EncabezadosObjetos { get; set; }
        public DbSet<DocumentacionHTML> Documentacion { get; set; }
        public DbSet<PlantillasHTMLModel> PlantillasHTML { get; set; }
        public DbSet<ConnModel> Conexiones { get; set; }
        public DbSet<PermisosObjeto> ListaPermisosObjeto { get; set; }
        public DbSet<PermisosModel> ListaPermisos { get; set; }
        public DbSet<UsuariosPDN> ListaUsuariosPDN { get; set; }
        public DbSet<AplicaHistoriaModel> AplicaHistoria { get; set; }
        public DbSet<HistoriaModel> Historia { get; set; }
    }
}
