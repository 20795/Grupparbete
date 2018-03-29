using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data.SqlClient;

namespace Grupparbete
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string filter = "";
            UpdateGraph(chart1, checkedListBox1, "_month_", filter, true); //populerar chart1
            UpdateGraph(chart2, checkedListBox2, "_job_", filter, true); //populerar chart2
            UpdateGraph(chart3, checkedListBox3, "_marital_", filter, true); //populerar chart3
            UpdateGraph(chart4, checkedListBox4, "_education_", filter, true); //populerar chart4
            UpdateMainGraph(filter); //populerar resultat grafen, chart5 
        }
        private string checkingListBox(CheckedListBox checkedListBox, string graphNamn)//sammanfattar incheckade checkBoxarna i en sträng för en checkedListBox
        {
            string s = "";
            for (int x = 0; x <= checkedListBox.CheckedItems.Count - 1; x++)
            {
                if (checkedListBox.CheckedItems.Count == 1)
                {
                    s = " AND " + graphNamn + "='" + checkedListBox.CheckedItems[x].ToString() + "'";
                }
                if (checkedListBox.CheckedItems.Count > 1 && x == 0)
                {
                    s = " AND (" + graphNamn + " ='" + checkedListBox.CheckedItems[0].ToString();
                }
                if (x > 0)
                {
                    s = s + "' OR " + graphNamn + "='" + checkedListBox.CheckedItems[x].ToString();
                }
                if (x > 0 && x == (checkedListBox.CheckedItems.Count - 1))
                {
                    s = s + "')";
                }
            }
            return s;
        }
        private void Filtering()//alla checkbox kollas och skapas en sträng som läggs till sql frågorna(efter WHERE som en filter) när graferna uppdateras
        {
            string s = "";
            s = checkingListBox(checkedListBox1, "_month_") +   //kollar checkedListBox för chart1 och returnerar i en sträng ex.:" AND (_month_= 'april' OR_month_= 'maj')"
                checkingListBox(checkedListBox2, "_job_") +     //kollar checkedListBox för chart1
                checkingListBox(checkedListBox3, "_marital_") +     //kollar checkedListBox för chart1
                checkingListBox(checkedListBox4, "_education_");    //kollar checkedListBox för chart1
                                                                    //här uppdateras graferna:
            UpdateGraph(chart1, checkedListBox1, "_month_", s, false);
            UpdateGraph(chart2, checkedListBox2, "_job_", s, false);
            UpdateGraph(chart3, checkedListBox3, "_marital_", s, false);
            UpdateGraph(chart4, checkedListBox4, "_education_", s, false);
            UpdateMainGraph(s);
        }
        private void UpdateGraph(Chart tempChart, CheckedListBox checkedListBox, string tempVar, string filter, bool init)// de 4 graferna fylls med data från sql
        {
            SqlConnection conn = new SqlConnection(); //Vi skapar ett objekt at typen SqlConnection
            conn.ConnectionString = "server=(local);Integrated Security = True;"; //såkallad connection String
            conn.Open(); //Vi öppnar länken mellan C# och SQL
                         //skickar en query till SQL-servern som returnerar alla uträknade punkter för grafen              
            SqlCommand myQuery = new SqlCommand(
                "SELECT DISTINCT T1." + tempVar + ", (select count(*) FROM [bankMarketing].[dbo].[bank-full] where "
                + tempVar + " = T1." + tempVar + " AND _y_ = 'yes' " + filter + ") AS 'Yes'," +
            "(select count(*) FROM [bankMarketing].[dbo].[bank-full] where " + tempVar + " = T1." + tempVar + " AND _y_ = 'no' " + filter + ") AS 'No' " +
            "FROM [bankMarketing].[dbo].[bank-full] T1 GROUP BY T1." + tempVar + " ORDER BY T1." + tempVar + ";", conn);
            SqlDataReader myReader = myQuery.ExecuteReader(); //nu kan vi läsa data från SQL
            tempChart.Series[0].Points.Clear();     //rensa series 0
            tempChart.Series[1].Points.Clear();     //rensa series 1
            tempChart.Titles.Clear();               //rensa titeln
            tempChart.Titles.Add(//sätta title för grafen
                new Title(
                tempVar,
                Docking.Top,
                new Font("Verdana", 12f, FontStyle.Bold),
                Color.Black
                )
            );
            tempChart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;//visa alla labels(inte bara 6st)
            while (myReader.Read())
            { //Sålänge rader finns att läsa så läser vi in rader
              //tilldela data till grafen
                tempChart.Series[0].Points.AddXY(myReader[tempVar].ToString(), double.Parse(myReader[1].ToString()) / (double.Parse(myReader[1].ToString()) + double.Parse(myReader[2].ToString())));
                //bar1(blå) - personer i kategorin som svarade ja/alla personer i kategorin
                tempChart.Series[1].Points.AddXY(myReader[tempVar].ToString(), (double.Parse(myReader[1].ToString()) + double.Parse(myReader[2].ToString())) / 45211);
                //bar2(orange) - personer i kategorin/alla personer i datasetet
                if (init)
                {
                    checkedListBox.Items.Add(myReader[0].ToString(), true);
                }
            }
            conn.Close();
        }
        private void UpdateMainGraph(string filter) //resultat grafen fylls med data från sql
        {
            SqlConnection conn = new SqlConnection(); //Vi skapar ett objekt at typen SqlConnection
            conn.ConnectionString = "server=(local);Integrated Security = True;"; //såkallad connection String
            conn.Open(); //Vi öppnar länken mellan C# och SQL
                         //skickar en query till SQL-servern som returnerar alla uträknade punkter för grafen              
            SqlCommand myQuery = new SqlCommand("SELECT(SELECT COUNT(*) FROM[bankMarketing].[dbo].[bank-full] WHERE _y_ = 'yes' " + filter + " ) AS 'yes'," +
                "(SELECT COUNT(*) FROM[bankMarketing].[dbo].[bank-full] WHERE _y_ = 'no' " + filter + " ) AS 'no';", conn);
            SqlDataReader myReader = myQuery.ExecuteReader(); //nu kan vi läsa data från SQL
            chart5.Series[0].Points.Clear();     //rensa series 0
            chart5.Series[1].Points.Clear();     //rensa series 1
            chart5.Titles.Clear();              //rensa titel
            while (myReader.Read())
            {
                //tilldela data till grafen OBS! Det är bara en rad.
                chart5.Series[0].Points.AddY((int)myReader["yes"]);
                chart5.Series[1].Points.AddY((int)myReader["no"]);
                chart5.Series[0].Label = "yes " + (int)myReader["yes"];
                chart5.Series[1].Label = "no " + (int)myReader["no"];
                chart5.Titles.Add(new Title("Result: " + Math.Round(double.Parse(myReader["yes"].ToString()) / ((double.Parse(myReader["yes"].ToString()) +
                    (double.Parse(myReader["no"].ToString())))) * 100, 2) + "%", Docking.Top, new Font("Verdana", 24f, FontStyle.Bold), Color.Black));
            }
            conn.Close();
            //den röda linjen i resultat grafen, chart5
            chart5.Annotations.Clear();
            chart5.Annotations.Add(new HorizontalLineAnnotation
            {
                AxisY = chart5.ChartAreas[0].AxisY,
                AllowMoving = true,
                IsInfinitive = true,
                ClipToChartArea = chart5.ChartAreas[0].Name,
                Name = "myLine",
                LineColor = Color.Red,
                LineWidth = 2,
                Y = 11.7
            }
            );
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filtering();
        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filtering();
        }

        private void checkedListBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filtering();
        }

        private void checkedListBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filtering();
        }
    }
}
