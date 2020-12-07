using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using waterskibaan;

namespace groenschermfrom
{
    public partial class roodScherm : Form
    {
        public roodScherm()
        {
            InitializeComponent();
        }
        class Todo
        {
            public int id { get; set; }
            public int klant_id { get; set; }
            public string datum { get; set; }
        }

        private void roodScherm_Load(object sender, EventArgs e)
        {
            Form1 Form1 = new Form1();
            Form1.Show();
            scanScherm scanScherm = new scanScherm();
            scanScherm.Show();

            RESTClient rClient = new RESTClient();
            rClient.endPoint = "https://demo.recras.nl/api2/boekingen";

            string strResponse = rClient.makeRequest();
            //boekingenModel boekingen = JsonConvert.DeserializeObject<boekingenModel>(strResponse);
            //Console.Out.WriteLine(boekingen.Boekingen.Count);
            List<Todo> todo = JsonConvert.DeserializeObject<List<Todo>>(strResponse);
            string contentString = "";
            foreach (var item in todo)
            {
                contentString += $"{item.id} : {item.klant_id} : {item.datum}\n";
            }

            richTextBox1.Text = contentString;
        }
    }
}
