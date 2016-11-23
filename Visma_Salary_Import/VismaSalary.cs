using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;
using System.Xml;
using System.IO;
using System.Configuration;

namespace Visma_Salary_Import
{
    public partial class SalaryXML : Form
    {
        public SalaryXML()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTimePicker dtStart = new DateTimePicker();
            dtStart.Value = StartDate.Value;
            DateTimePicker dtEnd = new DateTimePicker();
            dtEnd.Value = EndDate.Value;
            if (dtStart.Value > dtEnd.Value)
            {
                MessageBox.Show("Start Date has to before end date.");
                return;
            }

            int enddate = Int32.Parse(dtEnd.Value.ToString("yyyyMMdd"));
            int startdate = Int32.Parse(dtStart.Value.ToString("yyyyMMdd"));
            CreateXMLStoreProcedure(enddate, startdate);
            //UpdateAggr(enddate);
            //UpdateFreeInf1(enddate);

        }


        public static void CreateXMLStoreProcedure(int vEndDate, int vStartDate)
        {
            //String sConnection = "Data Source=mindsql\\mindsql;Initial Catalog=F0001;Persist Security Info=False;User ID=LonXML;Password=Vinter2016!;";
            String sConnection = System.Configuration.ConfigurationManager.ConnectionStrings["VismaSalary"].ConnectionString;
            String filename = System.Configuration.ConfigurationManager.AppSettings["filename"];
            SqlConnection mySqlConnection = new SqlConnection(sConnection);
            mySqlConnection.Open();

            // 1. Get the same data through the provider.
            SqlCommand cmd = new SqlCommand("SalaryCreateXML", mySqlConnection);

            // 2. set the command object so it knows to execute a stored procedure
            cmd.CommandType = CommandType.StoredProcedure;

            // 3. add parameter to command, which will be passed to the stored procedure
            cmd.Parameters.Add(new SqlParameter("@EndDate", vEndDate));
            cmd.Parameters.Add(new SqlParameter("@StartDate", vStartDate));
            
            using (XmlReader reader = cmd.ExecuteXmlReader())
            {
                XmlDocument dom = new XmlDocument();
                dom.Load(reader);

                var settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = false;
                settings.ConformanceLevel = ConformanceLevel.Document;
                settings.Encoding = UTF8Encoding.UTF8;

                //using (var writer = XmlTextWriter.Create("c:/temp/file2.xml", settings))
                using (var writer = XmlTextWriter.Create(filename, settings))
                {
                    dom.WriteContentTo(writer);
                }
            }

            // Write data to files: data1.xml and data2.xml for comparison.
            MessageBox.Show("The file " + filename + " has been created.");

        }



        public static void UpdateFreeInf1(int vEndDate)
        {
            //String sConnection = "Data Source=mindsql\\mindsql;Initial Catalog=F0001;Persist Security Info=False;User ID=LonXML;Password=Vinter2016!;";
            String sConnection = System.Configuration.ConfigurationManager.ConnectionStrings["VismaSalary"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(sConnection);
            mySqlConnection.Open();

            // Get the same data through the provider.
            // Use enddate
            String commandText = "update dbo.FreeInf1 set val5=1 where InfCatNo=3 and Val4=1 and Val5=0 and Dt1 < @EndDate";
            SqlCommand command = new SqlCommand(commandText, mySqlConnection);
            command.Parameters.Add("@EndDate", SqlDbType.Int);
            command.Parameters["@EndDate"].Value = vEndDate;

            int rows = command.ExecuteNonQuery();

        }

        public static void UpdateAggr(int vEndDate, int vStartDate)
        {
            //String sConnection = "Data Source=mindsql\\mindsql;Initial Catalog=F0001;Persist Security Info=False;User ID=LonXML;Password=Vinter2016!;";
            String sConnection = System.Configuration.ConfigurationManager.ConnectionStrings["VismaSalary"].ConnectionString;
            SqlConnection mySqlConnection = new SqlConnection(sConnection);
            mySqlConnection.Open();

            // Get the same data through the provider.
            // Use enddate
            String commandText = "set c.FinDt = " + vEndDate + " from Agr c inner join Prod P on c.ProdNo = P.ProdNo inner join Ord O on c.ProdNo = O.OrdNo  where P.ProdPrGr = 4 and c.ProdNo not in ('099', '059') and O.DelDt <= c.FinDt and c.FinDt < @EndDate and c.FinDt > @StartDate";
            SqlCommand command = new SqlCommand(commandText, mySqlConnection);
            command.Parameters.Add("@EndDate", SqlDbType.Int);
            command.Parameters["@EndDate"].Value = vEndDate;
            command.Parameters.Add("@StartDate", SqlDbType.Int);
            command.Parameters["@StartDate"].Value = vStartDate;

            int rows = command.ExecuteNonQuery();

        }

        static void GetConnectionStrings()
        {
            ConnectionStringSettingsCollection settings =
                ConfigurationManager.ConnectionStrings;

            if (settings != null)
            {
                foreach (ConnectionStringSettings cs in settings)
                {
                    Console.WriteLine(cs.Name);
                    Console.WriteLine(cs.ProviderName);
                    Console.WriteLine(cs.ConnectionString);
                }
            }
        }
    }
     }
