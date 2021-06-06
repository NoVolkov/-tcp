using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Чат_tcp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //поток прослушивания
        private Thread listen;
        private Server server;
        public MainWindow()
        {
            InitializeComponent();
            
        }



        [Obsolete]
        public void btn_St(object sender, RoutedEventArgs e)
        {
            if(server!=null && server.State)
            {
                btn_St_text.Content = "Запустить";
                t_Port.IsReadOnly = false;
                t_Port.Background = new SolidColorBrush(Colors.White);
                server.State = false;
                server.disconnect();
            }
            else
            {
                if(int.TryParse(t_Port.Text, out int p))
                {
                    try
                    {
                        server = new Server(p);
                        listen = new Thread(new ThreadStart(server.listen));
                        listen.Start();
                    }
                    catch
                    {
                        server.disconnect();
                    }
                    btn_St_text.Content = "Остановить";
                    t_Port.IsReadOnly = true;
                    t_Port.Background = new SolidColorBrush(Colors.LightGray);
                    server.State = true;
                }
            }
        }
    }
}
