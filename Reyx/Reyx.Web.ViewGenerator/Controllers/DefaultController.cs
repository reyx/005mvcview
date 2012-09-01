using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Reyx.Web.ViewGenerator.Models;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Reyx.Web.ViewGenerator.Controllers
{
    public class DefaultController : Controller
    {
        private string Pasta
        {
            get { return Server.MapPath("~/Assets/Views"); }
        }

        private string ConnectionString
        {
            get { return (string)Session["ConnectionString"]; }
            set { Session["ConnectionString"] = value; }
        }

        private List<Database> Databases
        {
            get
            {
                if (Session["Databases"] == null)
                {
                    Session["Databases"] = new List<Database>();
                }

                return (List<Database>)Session["Databases"];
            }
            set { Session["Databases"] = value; }
        }

        private string Database
        {
            get { return (string)Session["Database"]; }
            set { Session["Database"] = value; }
        }

        private List<Column> Columns
        {
            get
            {
                if (Session["Columns"] == null)
                {
                    List<Column> columns = new List<Column>();
                    
                    using (SqlConnection SqlCon = new System.Data.SqlClient.SqlConnection(ConnectionString))
                    {
                        SqlCon.Open();
                        SqlCommand SqlCom = new System.Data.SqlClient.SqlCommand();
                        SqlCom.Connection = SqlCon;
                        SqlCom.CommandText =
@"select 
    sys.columns.name			as column_p,
    sys.types.name				as type_p,
    sys.types.max_length		as max_length_p,
    sys.types.precision			as precision_p,
    sys.columns.is_nullable		as nullable_p,
    sys.columns.is_identity		as identity_p,
    sys.columns.is_filestream	as filestream_p,
    sys.tables.name				as table_p
from 
    sys.columns
inner join 
    sys.tables 
on 
    sys.tables.object_id = sys.columns.object_id
inner join 
    sys.types
on 
    sys.types.system_type_id = sys.columns.system_type_id
where type = 'U'
";

                        System.Data.SqlClient.SqlDataReader SqlDR;
                        SqlDR = SqlCom.ExecuteReader();

                        while (SqlDR.Read())
                        {
                            columns.Add(new Column()
                            {
                                Name = SqlDR.GetString(0),
                                Type = SqlDR.GetString(1),
                                MaxLength = SqlDR.GetInt16(2),
                                Precision = SqlDR.GetByte(3),
                                Nullable = SqlDR.GetBoolean(4),
                                Identity = SqlDR.GetBoolean(5),
                                FileStream = SqlDR.GetBoolean(6),
                                Table = Tables.FirstOrDefault(t => t.Name == SqlDR.GetString(7)) ?? new Table() { Name = SqlDR.GetString(7) }
                            });
                        }
                    }

                    Session["Columns"] = columns;
                }

                return (List<Column>)Session["Columns"];
            }
        }

        private List<Table> Tables
        {
            get
            {
                if (Session["Tables"] == null)
                {
                    Session["Tables"] = new List<Table>();
                }

                return (List<Table>)Session["Tables"];
            }
            set { Session["Tables"] = value; }
        }        

        private List<Table> SelectedTables
        {
            get
            {
                if (Request["Table"] == null)
                {
                    return new List<Table>();
                }

                return Request["Table"].Split(',').Select(t => new Table { Name = t }).ToList();
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new DbConnection());
        }

        [HttpPost]
        public ActionResult Index(DbConnection connection)
        {
            if (SelectedTables.Any())
            {
                if (Directory.Exists(Pasta))
                    Directory.Delete(Pasta, true);

                Directory.CreateDirectory(Pasta);

                FileInfo file;

                foreach (Table table in SelectedTables)
                {
                    string TableDirectory = Path.Combine(Pasta, table.ClassName);
                    Directory.CreateDirectory(TableDirectory);

                    file = new FileInfo(Path.Combine(TableDirectory, "Detail.cshtml"));
                    using (StreamWriter sw = file.CreateText())
                    {
                        sw.WriteLine("@{");
                        sw.WriteLine("    Layout = \"~/Views/Shared/_Pages.cshtml\";");
                        sw.WriteLine("}");

                        sw.WriteLine("@session Scripts {");
                        sw.WriteLine("    <script type=\"text/javascript\" src=\"@Url.Content(\"~/Scripts/cadastros/" + table.ClassName.ToLower() + ".js\")\"></script>");
                        sw.WriteLine("}");

                        sw.WriteLine("<form action=\"/\" method=\"post\" class=\"form-horizontal\">");
                        sw.WriteLine("    <fieldset>");
                        sw.WriteLine("        <legend>" + table.HumanName + "</legend>");
                        foreach (Column c in Columns.Where(t => t.Table.Name == table.Name))
                        {
                            if (c.Identity)
                            {
                                sw.WriteLine("    @Html.Hidden(\"" + c.Name + "\")");
                            }
                            else
                            {
                                sw.WriteLine("        <div class=\"control-group\">");
                                sw.WriteLine("            <label class=\"control-label\" for=\"" + c.Name + "\">" + c.HumanName + "</label>");
                                sw.WriteLine("            <div class=\"controls\">");
                                sw.WriteLine("                @Html.EditorFor(model => model." + c.Name + ")");
                                sw.WriteLine("            </div>");
                                sw.WriteLine("        </div>");
                            }
                        }
                        sw.WriteLine("    </fieldset>");
                        sw.WriteLine("</form>");
                    }	

                    file = new FileInfo(Path.Combine(TableDirectory, "Index.cshtml"));
                    using (StreamWriter sw = file.CreateText())
                    {
                        sw.WriteLine("<table class=\"table table-striped table-hover\">");
                        sw.WriteLine("    <thead>");
                        sw.WriteLine("        <tr>");
                        foreach (Column column in Columns.Where(t => t.Table.Name == table.Name))
                        {
                            sw.WriteLine("            <th>" + column.HumanName + "</th>");
                        }
                        sw.WriteLine("        </tr>");
                        sw.WriteLine("    </thead>");
                        sw.WriteLine("</table>");
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(ConnectionString))
                {
                    SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder();
                    connectionString.DataSource = connection.DataSource;
                    connectionString.UserID = connection.User;
                    connectionString.Password = connection.Password;
                    connectionString.MultipleActiveResultSets = true;

                    ConnectionString = connectionString.ConnectionString;
                }

                if (!Databases.Any())
                {
                    using (SqlConnection SqlCon = new System.Data.SqlClient.SqlConnection(ConnectionString))
                    {
                        SqlCon.Open();
                        SqlCommand SqlCom = new System.Data.SqlClient.SqlCommand();
                        SqlCom.Connection = SqlCon;
                        SqlCom.CommandType = CommandType.StoredProcedure;
                        SqlCom.CommandText = "sp_databases";

                        System.Data.SqlClient.SqlDataReader SqlDR;
                        SqlDR = SqlCom.ExecuteReader();

                        while (SqlDR.Read())
                        {
                            Databases.Add(new Database() { Name = SqlDR.GetString(0) });
                        }
                    }
                }

                if (!Tables.Any() && !string.IsNullOrWhiteSpace(Database))
                {
                    SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(ConnectionString);
                    connectionString.InitialCatalog = connection.Database;
                    ConnectionString = connectionString.ConnectionString;

                    using (SqlConnection SqlCon = new System.Data.SqlClient.SqlConnection(ConnectionString))
                    {
                        SqlCon.Open();
                        SqlCommand SqlCom = new System.Data.SqlClient.SqlCommand("SELECT NAME FROM SYS.TABLES", SqlCon);
                        System.Data.SqlClient.SqlDataReader SqlDR;
                        SqlDR = SqlCom.ExecuteReader();

                        while (SqlDR.Read())
                        {
                            Tables.Add(new Table() { Name = SqlDR.GetString(0) });
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(connection.Database) && string.IsNullOrWhiteSpace(Database))
                Database = connection.Database;
            else
                connection.Database = Database;
            
            connection.Databases = Databases;
            connection.Tables = Tables;

            return View(connection);
        }
    }
}