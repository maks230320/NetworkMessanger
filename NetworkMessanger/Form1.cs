using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace NetworkMessanger
{
    public partial class Form1 : Form
    {
        delegate void AddTextDelegate(String text);
        NetSender netSender;
        NetReciever netReciever;
        MessageProcessor messageProcessor;
        
        
        public Form1()
        {
            InitializeComponent();
            if (GetIP() == "")
            {
                MessageBox.Show("ip error");
                this.Close();
            }
            netSender = new NetSender();
            netSender.NeedTextIP += Sender_NeedTextIP;
            netSender.NeedPort += Port_Request;

            netReciever = new NetReciever();
            netReciever.NeedTargetIP += NeedTargetIP;
            netReciever.NeedPort += Port_Request;
            netReciever.MessageRecived += NetReciever_MessageRecived;

            messageProcessor = new MessageProcessor();
            messageProcessor.NeedTargetIP += NeedTargetIP;
            messageProcessor.NeedNick += MessageProcessor_NeedNick;
            messageProcessor.NeedSendMessage += MessageProcessor_NeedSendMessage;
            NickTB.Text = ConfigurationManager.AppSettings["nickname"];
            portNumeric.Value = decimal.Parse(ConfigurationManager.AppSettings["port"]);
            SendB.Enabled = false;
        }

        private void Port_Request(object sender, IntEventArgs e)
        {
            e.Number = int.Parse(ConfigurationManager.AppSettings["port"]);
        }

        private void MessageProcessor_NeedSendMessage(object sender, MessageEventArgs e)
        {
            netSender.Send(e.Text, e.Address);
        }

        private void NetReciever_MessageRecived(object sender, MessageEventArgs e)
        {
            string message = messageProcessor.Process(e.Text, e.Address);
            if (!String.IsNullOrEmpty(message))
                ReceiveTB.BeginInvoke(new AddTextDelegate(PublishMessage), message);
        }

        private void MessageProcessor_NeedNick(object sender, TextEventArgs e)
        {
            e.Text = ConfigurationManager.AppSettings["nickname"];
        }

        private void NeedTargetIP(object sender, TextEventArgs e)
        {
            e.Text = GetIP();
        }

        private void Sender_NeedTextIP(object sender, TextEventArgs e)
        {
            e.Text = GetMaskIp();
        }

        private void SendB_Click(object sender, EventArgs e)
        {
            
            netSender.Send(SendTB.Text);
            SendTB.Text = String.Empty;
            if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0)
            {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[1].ConnectionString);
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand($"UPDATE Table_1 SET history='{ReceiveTB.Text}' WHERE login='{textBox1.Text}' AND password={textBox2.Text.GetHashCode()}",conn);
                    int number = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        void PublishMessage(string message)
        {
            ReceiveTB.Text += message;
            ReceiveTB.ScrollToCaret();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SocketError error = netReciever.Start();
            if (error != SocketError.Success)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            netReciever.Stop();
        }

        bool textSended = false;

        private void SendTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (SendB.Enabled == true)
            {
                if (e.KeyCode == Keys.Enter && !e.Shift)
                {
                    netSender.Send(SendTB.Text);
                    textSended = true;
                }
            }
        }

        private void SendTB_TextChanged(object sender, EventArgs e)
        {
            if (textSended)
            {
                SendTB.Text = String.Empty;
                textSended = false;
            }
        }
        public string GetIP()
        {
            try
            {
                string strHostName = System.Net.Dns.GetHostName();

                IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);


                IPAddress[] addr = ipEntry.AddressList;
                int costyl = 0;
                for (int i = 0; i < addr.Length; i++)
                {
                    if (addr[i].ToString().Split('.').Length == 4)
                    {
                        if (costyl == 0) { return addr[i].ToString(); }
                        else
                        {
                            costyl++;
                        }
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
        public string GetMaskIp()
        {
            string ip = GetIP();
            string[] mas = ip.Split('.');
            mas[3] = "255";
            return mas[0] + "." + mas[1] + "." + mas[2] + "." + mas[3];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);


            if (portNumeric.Value > 0)
            {
                config.AppSettings.Settings.Remove("port");
                config.AppSettings.Settings.Add("port", portNumeric.Value.ToString());
            }
            else
            {
                MessageBox.Show("Port must be > 0");
            }

            if (NickTB.Text != "")
            {
                config.AppSettings.Settings.Remove("nickname");
                config.AppSettings.Settings.Add("nickname", NickTB.Text);
            }
            else
            {
                MessageBox.Show("Nickname can not be empty");
            }
            config.Save(ConfigurationSaveMode.Full);
            MessageBox.Show("для вступления изменений в силу перезапустите приложение");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0)
            {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[1].ConnectionString);
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand($"SELECT 1 from Table_1 WHERE login='{textBox1.Text}' AND password={textBox2.Text.GetHashCode()}", conn);
                    object number = cmd.ExecuteScalar();
                    if (number!=null)
                    {
                        SendB.Enabled = true;
                        label5.Visible = false;
                        label6.Visible = false;
                        label7.Visible = false;
                        button2.Enabled = false;
                        button2.Visible = false;
                        button3.Enabled = false;
                        button3.Visible = false;
                        textBox1.Enabled = false;
                        textBox1.Visible = false;
                        textBox2.Enabled = false;
                        textBox2.Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("Incorrect login or password");
                    }
                    cmd = new SqlCommand($"SELECT history from Table_1 WHERE login='{textBox1.Text}' AND password={textBox2.Text.GetHashCode()}", conn);
                    number = cmd.ExecuteScalar();
                    if (number != null)
                    {
                        ReceiveTB.Text = number.ToString();
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                MessageBox.Show("поля не могут быть пустыми");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0)
            {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[1].ConnectionString);
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(String.Format("INSERT INTO Table_1 (login, password) VALUES ('{0}', {1})", textBox1.Text, textBox2.Text.GetHashCode()),conn);
                    int number = cmd.ExecuteNonQuery();
                    if (number > 0) {
                        SendB.Enabled = true;
                        label5.Visible = false;
                        label6.Visible = false;
                        label7.Visible = false;
                        button2.Enabled = false;
                        button2.Visible = false;
                        button3.Enabled = false;
                        button3.Visible = false;
                        textBox1.Enabled = false;
                        textBox1.Visible = false;
                        textBox2.Enabled = false;
                        textBox2.Visible = false;
                    }
                } 
                catch(Exception ex) {
                    MessageBox.Show($"Error: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                MessageBox.Show("поля не могут быть пустыми");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}

