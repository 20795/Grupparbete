using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data.SqlClient;
using System.Data;

namespace Grupparbete
{
    class PopulatingGraph
    {
        public PopulatingGraph(Chart tempChart, string tempVar)
        {
            SqlConnection conn = new SqlConnection(); //Vi skapar ett objekt at typen SqlConnection
            conn.ConnectionString = "server=(local);Integrated Security = True;"; //såkallad connection String
            conn.Open(); //Vi öppnar länken mellan C# och SQL
            //skickar en query till SQL-servern som returnerar alla uträknade punkter för grafen              
            SqlCommand myQuery = new SqlCommand("SELECT DISTINCT T1." + tempVar + ", (select count(*) FROM [bankMarketing].[dbo].[bank-full] where "
                + tempVar + " = T1." + tempVar + " AND _y_ = 'yes') AS 'Yes'," +
            "(select count(*) FROM [bankMarketing].[dbo].[bank-full] where " + tempVar + " = T1." + tempVar + " AND _y_ = 'no') AS 'No' " +
            "FROM [bankMarketing].[dbo].[bank-full] T1 GROUP BY T1." + tempVar + " ORDER BY T1." + tempVar + ";", conn);

            SqlDataReader myReader = myQuery.ExecuteReader(); //nu kan vi läsa data från SQL
            tempChart.Titles.Add(tempVar);//sätta title för grafen

            while (myReader.Read())
            { //Sålänge rader finns att läsa så läser vi in rader
              //tilldela data till grafen
                tempChart.Series[0].Points.AddXY(myReader[tempVar].ToString(), double.Parse(myReader[1].ToString()) / (double.Parse(myReader[1].ToString()) + double.Parse(myReader[2].ToString())));
                //bar1(blå) - personer i kategorin som svarade ja/alla personer i kategorin
                tempChart.Series[1].Points.AddXY(myReader[tempVar].ToString(), (double.Parse(myReader[1].ToString()) + double.Parse(myReader[2].ToString())) / 45211);
                //bar2(orange) - personer i kategorin/alla personer i datasetet
            }
            conn.Close();
        }
    }
}
