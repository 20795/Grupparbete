using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grupparbete
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PopulatingGraph data1 = new PopulatingGraph(chart1, "_age_"); //skapar objekt för att populera grafen chart1
            PopulatingGraph data2 = new PopulatingGraph(chart2, "_job_"); //skapar objekt för att populera grafen chart2
            PopulatingGraph data3 = new PopulatingGraph(chart3, "_marital_"); //skapar objekt för att populera grafen chart3
            PopulatingGraph data4 = new PopulatingGraph(chart4, "_education_"); //skapar objekt för att populera grafen chart4
        }
    }
}
