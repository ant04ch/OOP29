using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace OOP29
{
    public partial class Form2 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public string TextToSend;
        private string username;
        public Form2()
        {
            InitializeComponent();

            // За замовчуванням поле для вводу імені вимкнене
            UsernameTextBox.Enabled = true;

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ServerIPtextBox.Text = address.ToString();
                }
            }
            ChatScreentextBox.Multiline = true; // Enable multiline
            ChatScreentextBox.WordWrap = true; // Enable word wrap
        }

        private void button1_Click(object sender, EventArgs e) // Старт
        {
            // Перевіряємо, чи введене ім'я користувача
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                MessageBox.Show("Please enter your name.");
                return;
            }

            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(ServerPorttextBox.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;

            // Зберігаємо введене ім'я
            username = UsernameTextBox.Text;

            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        private void button2_Click(object sender, EventArgs e) // Конект
        {// Перевіряємо, чи введене ім'я користувача
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                MessageBox.Show("Please enter your name.");
                return;
            }

            client = new TcpClient();
            IPEndPoint IpEnd = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(ClientPorttextBox.Text));
            try
            {
                client.Connect(IpEnd);

                if (client.Connected)
                {
                    ChatScreentextBox.AppendText("Connect to Server" + "\n");
                    STW = new StreamWriter(client.GetStream());
                    STR = new StreamReader(client.GetStream());
                    STW.AutoFlush = true;

                    // Зберігаємо введене ім'я
                    username = UsernameTextBox.Text;

                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    this.ChatScreentextBox.Invoke(new MethodInvoker(delegate ()
                    {
                        ChatScreentextBox.AppendText(username + ": " + recieve + "\n\n");
                    }));
                    recieve = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                // Використовуємо ім'я користувача при відправці повідомлення
                string message = username + ": " + TextToSend;

                // Додаємо символ нового рядка (\n) в кінець повідомлення
                message += "\n";

                STW.WriteLine(message);
                this.ChatScreentextBox.Invoke(new MethodInvoker(delegate ()
                {
                    ChatScreentextBox.AppendText(message);
                }));
            }
            else
            {
                MessageBox.Show("Sending Failed");
            }
            backgroundWorker2.CancelAsync();
        }

        private void button3_Click(object sender, EventArgs e) // Відправити
        {
            if (MessagetextBox.Text != "")
            {
                TextToSend = MessagetextBox.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            MessagetextBox.Text = "";
        }

        private void MessagetextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ChatScreentextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ServerPorttextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ServerIPtextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ClientPorttextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void UsernameTextBox_TextChanged(object sender, EventArgs e)
        {
            // Вмикаємо або вимикаємо поля для вводу IP та порту в залежності від наявності імені
            textBox1.Enabled = !string.IsNullOrWhiteSpace(UsernameTextBox.Text);
            ClientPorttextBox.Enabled = !string.IsNullOrWhiteSpace(UsernameTextBox.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            string filename = "";
            saveDlg.Filter = "Text File (*.txt)|*.txt";
            saveDlg.DefaultExt = "txt";
            saveDlg.Title = "Save the contents";
            DialogResult result = saveDlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                filename = saveDlg.FileName;
                File.WriteAllText(filename, ChatScreentextBox.Text);
            }
        }
    }
}