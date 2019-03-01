using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Timers;

namespace BlockingSQL
{

 
    public partial class Form1 : Form
    {
        string conString = @"Server=stephenssql\steph;Database=master;Trusted_Connection=True;";
        string queryString = @"SELECT
db.name DBName,
tl.request_session_id,
wt.blocking_session_id,
OBJECT_NAME(p.OBJECT_ID) BlockedObjectName,
tl.resource_type,
h1.TEXT AS RequestingText,
h2.TEXT AS BlockingTest,
tl.request_mode
FROM sys.dm_tran_locks AS tl
INNER JOIN sys.databases db ON db.database_id = tl.resource_database_id
INNER JOIN sys.dm_os_waiting_tasks AS wt ON tl.lock_owner_address = wt.resource_address
INNER JOIN sys.partitions AS p ON p.hobt_id = tl.resource_associated_entity_id
INNER JOIN sys.dm_exec_connections ec1 ON ec1.session_id = tl.request_session_id
INNER JOIN sys.dm_exec_connections ec2 ON ec2.session_id = wt.blocking_session_id
CROSS APPLY sys.dm_exec_sql_text(ec1.most_recent_sql_handle) AS h1
CROSS APPLY sys.dm_exec_sql_text(ec2.most_recent_sql_handle) AS h2";


        public Form1()
        {
            InitializeComponent();
            InitalizeTimer();
        }

        private int counter;

        private void Form1_Load(object sender, EventArgs e)
        {           

           
            
        }

        private void InitalizeTimer()
        {
            counter = 0;
            timer1.Interval = 10000;
            timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            


        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            counter = counter + 1;
            label1.Text = "Last Ran: " + DateTime.Now.ToString();

            btn_block.BackColor = Color.Yellow;

            SqlConnection dbConn = new SqlConnection(conString);
            dbConn.Open();
            SqlCommand queryCommand = new SqlCommand(queryString, dbConn);
            SqlDataReader querycommandreader = queryCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(querycommandreader);
            if (dataTable.Rows.Count == 0)
            {
                btn_block.BackColor = Color.Green;
                btn_block.Text = "No Blocking";
            }
            else
            {
                btn_block.BackColor = Color.Red;
                btn_block.Text = "BLOCKING";
                MessageBox.Show("Blocking Detected on the SQL Server");
            }

            dbConn.Close();

        }
    }
}
