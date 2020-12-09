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
        SCardReader cardReader;
        SCardChannel channel;
        private tijd dates = new tijd();
        private bracelet braceletCode = new bracelet();
        private User GetUser = new User();

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
            braceletCode.braceletCode = /*"> apicall <"*/ 0 ;
            string name = GetUser.FirstName + " " + GetUser.LastName + "\n" + dates.bookingStart + " tot " + dates.bookingEnd;
            textBox1.Text = name;
        }
        public void getReaders()
        {

            try
            {
                string[] readers = SCARD.Readers;
                if (readers.Length == 0)
                {
                    textBox1.AppendText("Geen readers");
                }
                cardReader = new SCardReader(0, readers[0]);
                textBox1.AppendText(cardReader.StatusAsString);
                cardReader.StartMonitor(onCardChange);
                ///SCARD.Connect(SCARD.PCI_RAW(), readers[0], 0x00000003, 0x00000002, Handle,);
                ///SCARD.ListCards();
                ///SCARD.ListReaders(textBox1.Text);
            }
            catch (Exception)
            {
                textBox1.AppendText(SCARD.ErrorToMessage(0));
            }

            
        }

        delegate void onCardChangeInvoker(uint readerState, CardBuffer cardAtr);

        public void onCardChange(uint readerState, CardBuffer cardAtr)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new onCardChangeInvoker(onCardChange), readerState, cardAtr);
                return;
            }
            channel = new SCardChannel(cardReader);
            if(channel.Connect())
            {
                textBox1.Text = "Success";
            }
            CAPDU capdu = new CAPDU(0x00, 0x00, 0x00, 0x00);
            
            channel.Transmit(capdu);
            RAPDU rapdu = channel.Transmit(capdu);
            Console.WriteLine(rapdu);

            
            //textBox1.Text = rapdu.ToString();
            ///SCARD.Connect(SCARD.PCI_RAW(), cardReader, 0x00000003, 0x00000002, Handle, ));
        }
    }
}
