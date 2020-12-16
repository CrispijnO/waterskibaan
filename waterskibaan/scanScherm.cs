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

            testTHIS();

            /*int timeLeft = tijd.get_time_left();
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
            

            braceletCode.braceletCode =  0 ;
            string name = GetUser.FirstName + " " + GetUser.LastName + "\n" + dates.bookingStart + " tot " + dates.bookingEnd;
            nameOutput.Text = "Welkom " + GetUser.FirstName + " " + GetUser.LastName;
            labelTime.Text = "time left: " + timeLeft.ToString() + " min";
            textBox1.Text = name;
            RESTClient rClient = new RESTClient();
            rClient.endPoint = "https://demo.recras.nl/api2/contacten/9034/afbeelding";
            string response = rClient.makeRequest();
            Console.WriteLine(response);*/
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
                        braceletCode.braceletCode = 51;
                        break;
                    default:
                        textBox1.Text = "Unknown code";
                        Console.WriteLine(hexadecimalResult);
                        return;
                }
                string response = string.Empty;
                RESTClient rClient = new RESTClient();
                rClient.endPoint = "https://demo.recras.nl/api2/klanten/" + braceletCode.braceletCode;
                response = rClient.makeRequest();
                klanten responseKlant = JsonConvert.DeserializeObject<klanten>(response);
                rClient.endPoint = "https://demo.recras.nl/api2/boekingen?klant.id=" + braceletCode.braceletCode;
                response = rClient.makeRequest();
                boekingen responseBoeking = JsonConvert.DeserializeObject<boekingen>(response);


                channel.Disconnect();
                channel = null;
                cardReader.StopMonitor();

                ///SCARD.Connect(SCARD.PCI_RAW(), cardReader, 0x00000003, 0x00000002, Handle, ));
            }
        }
        private void testTHIS()
        {
            string log = string.Empty;
            Console.WriteLine("GETTING EXECUTED.");
            string braceletCode = "8417";
            string response = string.Empty;
            RESTClient rClient = new RESTClient();
            rClient.endPoint = "https://demo.recras.nl/api2/klanten/51";
            response = rClient.makeRequest();
            klanten responseKlant = JsonConvert.DeserializeObject<klanten>(response);
            rClient.endPoint = "https://demo.recras.nl/api2/boekingen?klant.id=51&embed=boekingsregels";
            response = rClient.makeRequest();
            List<boekingen> responseBoeking = JsonConvert.DeserializeObject<List<boekingen>>(response);
            Console.WriteLine("Boutta log the output!!");
            DateTime test2 = new DateTime().AddHours(1);
            int index = 0;
            int maxIndex = 1;
            bool found = false;
            responseBoeking.ForEach(boeking =>
            {
                // setting the maxIndex to the right number.
                // doing -1 because the index starts at 0 and not 1.
                maxIndex = boeking.boekingsregels.Count - 1;
                // the time of now -1 hour.
                DateTime DateNow = DateTime.Now.AddHours(-1);
                while (found == false || index < maxIndex)
                {
                    var boekingsregelW = boeking.boekingsregels[index];
                    Console.WriteLine("getting executed. " + boekingsregelW.begin);
                    if (boekingsregelW.begin <= DateNow)
                    {
                        index++;
                    } else
                    {
                        found = true;
                    }
                }
                var boekingsregel = boeking.boekingsregels[index];
                TimeSpan t = timeLeft(boekingsregel.begin, boekingsregel.eind);
                if (t.Days > 0 || t.Hours > 1)
                {
                    GroenPanel.Visible = true;
                    roodPanel.Visible = true;
                } else
                {
                    GroenPanel.Visible = true;
                }
                string formattedTimeString = formatTime(t);
                Console.WriteLine(formattedTimeString);
                // TODO verwisselen van scherm en de vakken in vullen.


                ///going over an array for the start / end time.
                /*boeking.boekingsregels.ForEach(boekingsregel =>
                {
                    


                    if (boekingsregel.begin > DateNow)
                    {
                        TimeSpan t = timeLeft(boekingsregel.begin, boekingsregel.eind);
                        string pleaseWork = string.Format("{0} Days, {1} Hours, {2} Minutes, {3} Seconds til launch.", t.Days, t.Hours, t.Minutes, t.Seconds);
                        Console.WriteLine(pleaseWork);
                    }

                });*/
            });
            Console.WriteLine(log);
        }
        public TimeSpan timeLeft(DateTime start, DateTime end)
        {
            DateTime now = DateTime.Now;
            Console.WriteLine("[NOW | END] " + now + " | " + end);
            TimeSpan result = end - now;
            return result;
        }

        public string formatTime(TimeSpan time)
        {
            string formattedTime = string.Empty;
            bool days = time.Days > 0;
            bool hours = time.Hours > 0;
            bool minutes = time.Minutes > 0;
            bool seconds = time.Seconds > 0;
            if (days)
            {
                if (time.Days == 1)
                {
                    formattedTime += string.Format("{0} Dag, ", time.Days);
                }
                else
                {
                    formattedTime += string.Format("{0} Dagen, ", time.Days);
                }
            }
            if (days || hours)
            {
                if (time.Hours == 1)
                {
                    formattedTime += string.Format("{0} Uur, ", time.Hours);
                }
                else
                {
                    formattedTime += string.Format("{0} Uren, ", time.Hours);
                }
            }
            if (days || hours || minutes)
            {
                if (time.Minutes == 1)
                {
                    formattedTime += string.Format("{0} Minuut, ", time.Minutes);
                } else
                {
                    formattedTime += string.Format("{0} Minuten, ", time.Minutes);
                }
            }
            if (days || hours || minutes || seconds)
            {
                if (time.Seconds == 1)
                {
                    formattedTime += string.Format("{0} Seconde", time.Seconds);
                }
                else
                {
                    formattedTime += string.Format("{0} Secondes", time.Seconds);
                }
            }
            return formattedTime;
        }
        private void errorButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void errorButton_CheckedChanged_1(object sender, EventArgs e)
        {

        }

    }
}
