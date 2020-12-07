using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpringCard.PCSC;
using SpringCard.LibCs;
using SpringCard.PCSC.CardLibraries;

namespace groenschermfrom
{
    public partial class scanScherm : Form
    {

        public scanScherm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getReaders();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
           // bracelet.braceletCode = > apicall <
           // string name = User.FirstName + " " + User.LastName;
           // textBox1.Text = name;
        }

        private void getReaders()
        {

            try
            {
                string[] readers = SCARD.Readers;
                if (readers.Length == 0)
                {
                    textBox1.AppendText("Geen readers");
                }
                foreach (string reader in readers)
                {
                    textBox1.AppendText(reader + " test");
                }
            }
            catch (Exception)
            {
                textBox1.AppendText("Error");
            }

            ///SCARD.Connect();
        }
    }
}
