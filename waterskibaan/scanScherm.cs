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
using waterskibaan;
using Newtonsoft.Json;

namespace groenschermfrom
{   

    public partial class scanScherm : Form
    {
        class jsonDeserialize
        {
            public int id { get; set; }
            public int klant_id { get; set; }
        }

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
            GroenPanel.Visible = false;
            roodPanel.Visible = false;
            panel1.Visible = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            
            
            int timeLeft = tijd.get_time_left();
            if(errorCheck.Checked == true) { 
            if (roodPanel.Visible == false)
            {
                roodPanel.Visible = true;
            }
            else if (roodPanel.Visible == true)
            {
                roodPanel.Visible = false;
            }
            } else { 
            if (GroenPanel.Visible == false)
            {
                GroenPanel.Visible = true;
            } else if(GroenPanel.Visible == true){
                 GroenPanel.Visible = false;
            }
            };
            

            braceletCode.braceletCode = /*"> apicall <"*/ 0 ;
            string name = GetUser.FirstName + " " + GetUser.LastName + "\n" + dates.bookingStart + " tot " + dates.bookingEnd;
            nameOutput.Text = "Welkom " + GetUser.FirstName + " " + GetUser.LastName;
            labelTime.Text = "time left: " + timeLeft.ToString() + " min";
            textBox1.Text = name;
            RESTClient rClient = new RESTClient();
            rClient.endPoint = "https://demo.recras.nl/api2/contacten/9034/afbeelding";
            string response = rClient.makeRequest();
            Console.WriteLine(response);
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
            if (cardAtr != null)
            {
                if (InvokeRequired)
                {
                    this.BeginInvoke(new onCardChangeInvoker(onCardChange), readerState, cardAtr);
                    return;
                }
                channel = new SCardChannel(cardReader);
                if (!channel.Connect())
                {
                    return;
                }
                CAPDU capdu = new CAPDU(0xff, 0xca, 0x00, 0x00);
                RAPDU rapdu = channel.Transmit(capdu);
                if (rapdu.SW != 0x9000)
                {
                    textBox1.Text = "Get UID APDU failed!";
                    return;
                }
                byte[] rapduB = rapdu.data.GetBytes();
                string hexadecimalResult = BitConverter.ToString(rapduB);
                textBox1.Text = hexadecimalResult;

                switch (hexadecimalResult)
                {
                    case "04-BA-B7-82-E0-60-80":
                        braceletCode.braceletCode = 8417;
                        break;
                    case "04-BA-B7-82-E0-60-80----2":
                        braceletCode.braceletCode = 9033;
                        break;
                    default:
                        textBox1.Text = "Unknown code";
                        Console.WriteLine(hexadecimalResult);
                        return;
                }

                RESTClient rClient = new RESTClient();
                rClient.endPoint = "https://demo.recras.nl/api2/klanten/" + braceletCode.braceletCode;
                string response = rClient.makeRequest();
                Console.WriteLine(response);
                ///rClient.endPoint = "https://demo.recras.nl/api2/boekingen";
                ///string boekingen = rClient.makeRequest();
                ///List<jsonDeserialize> boekinenDeser = JsonConvert.DeserializeObject<List<jsonDeserialize>>(boekingen);
                ///foreach(var item in boekinenDeser)
                ///{
                ///    if(item.klant_id == braceletCode.braceletCode)
                ////   {
                ///        Console.WriteLine(item.id + " " + item.klant_id);
                ///    }
                ///}                

                channel.Disconnect();
                channel = null;
                cardReader.StopMonitor();

                ///SCARD.Connect(SCARD.PCI_RAW(), cardReader, 0x00000003, 0x00000002, Handle, ));
            }
        }

        private void errorButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void errorButton_CheckedChanged_1(object sender, EventArgs e)
        {

        }

    }
}
