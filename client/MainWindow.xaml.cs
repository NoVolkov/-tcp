using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace Чат_tcp_client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string hostIP = "127.0.0.1";
        private int port = 8888;
        private string userName;
        private TcpClient client;
        private NetworkStream stream;
        private bool state = false;
      
        public MainWindow()
        {
            InitializeComponent();
            t_Message.IsReadOnly = true;
            t_Message.Background= new SolidColorBrush(Colors.LightGray);
            btn_Send_text.IsEnabled = false;
        }

        
        [Obsolete]
        private void btn_Conn(object sender, RoutedEventArgs e)
        {
            if (client != null && state)
            {
                disconnect();
                btn_Conn_text.Content = "Подключиться";
                t_Port.IsReadOnly = false;
                t_Port.Background = new SolidColorBrush(Colors.White);
                t_UserName.IsReadOnly = false;
                t_UserName.Background = new SolidColorBrush(Colors.White);
                state = false;
                t_Message.IsReadOnly = true;
                t_Message.Background = new SolidColorBrush(Colors.LightGray);
                btn_Send_text.IsEnabled = false;
            }
            else
            {
                if (int.TryParse(t_Port.Text, out int p))
                {
                    try
                    {
                        port = p;
                        userName = t_UserName.Text;
                        
                            client = new TcpClient();
                            client.Connect(hostIP, port);
                            stream = client.GetStream();
                            byte[] data = Encoding.Unicode.GetBytes(t_UserName.Text);
                            stream.Write(data, 0, data.Length);
                           
                            //поток для получения сообщений
                            Thread dataThread = new Thread(new ThreadStart(receiceMessage));
                            dataThread.Start();
                        

                        btn_Conn_text.Content = "Отключиться";
                        t_Port.IsReadOnly = true;
                        t_Port.Background = new SolidColorBrush(Colors.LightGray);
                        t_UserName.IsReadOnly = true;
                        t_UserName.Background = new SolidColorBrush(Colors.LightGray);
                        state = true;
                        t_Message.IsReadOnly = false;
                        t_Message.Background = new SolidColorBrush(Colors.White);
                        btn_Send_text.IsEnabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Dispatcher.Invoke(() => l_ListMesseges.Items.Add(new Mmessage("Разрыв подключения.")));
                        disconnect();
                    }

                }
                else
                {
                    MessageBox.Show("Порт должен быть целым числом.");
                }
            }
        }
        [Obsolete]
        private void btn_Send(object sender, RoutedEventArgs e)
        {
            if (state)
            {
                string mes = t_Message.Text;
                byte[] data = Encoding.Unicode.GetBytes(mes);
                stream.Write(data, 0, data.Length);
                Dispatcher.Invoke(() => l_ListMesseges.Items.Add(new Mmessage("ВЫ: " + mes)));
                t_Message.Text = "";
            }
        }

        public class Mmessage
        {
            public string message { get; set; }
            public Mmessage(string m)
            {
                message = new string(m);
            }
        }

        private void receiceMessage()
        {
            while (state)
            {
                try
                {
                    byte[] data=new byte[64];
                    while (stream.DataAvailable)
                    {
                        stream.Read(data, 0, data.Length);
                        string mes = Encoding.Unicode.GetString(data);
                        Mmessage m = new Mmessage(mes);
                        //чтобы не было конфликтов потоков (ui-элементы должны работать в главном потоке)
                        Dispatcher.Invoke(() => l_ListMesseges.Items.Add(m));
                    }
                    
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message+'\n'+ex.Source+'\n'+ex.TargetSite);
                    Dispatcher.Invoke(() => l_ListMesseges.Items.Add(new Mmessage("Разрыв подключения.")));
                    disconnect();
                }
            }
        }
        private void disconnect()
        {
            if (stream != null) stream.Close();
            if (client != null) client.Close();
        }
    }
}
