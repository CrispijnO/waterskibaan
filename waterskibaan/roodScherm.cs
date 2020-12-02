using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace groenschermfrom
{
    public partial class roodScherm : Form
    {
        public roodScherm()
        {
            InitializeComponent();
        }

        private void roodScherm_Load(object sender, EventArgs e)
        {
            Form1 Form1 = new Form1();
            Form1.Show();
        }
    }
}
