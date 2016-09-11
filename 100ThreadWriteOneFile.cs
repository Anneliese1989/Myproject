using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace _100Thread
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow ( )
        {
            InitializeComponent ( );

            Loaded += MainWindow_Loaded;

        }
        CancellationTokenSource cts;
        private void MainWindow_Loaded ( object sender, RoutedEventArgs e )
        {
            txt.TextWrapping = TextWrapping.Wrap;
            if (!File.Exists ( "temp.txt" ))
            {
                File.Create ( "temp.txt" );
            }
            FileStream file = File.Open ( "temp.txt", FileMode.Truncate );
            for (int i = 0; i < 100; i++)
            {
                Task task = new Task ( writetxt );
                task.Start ( );
            }
        }
        string temp;

        public string Temp
        {
            get
            {
                return temp;
            }

            set
            {
                temp = value;
                UpdateProperty ( "Temp" );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void UpdateProperty ( string str )
        {
            PropertyChanged?.Invoke ( this, new PropertyChangedEventArgs ( str ) );
        }

        object obj = new object ( );

        private void writetxt ( )
        {
            // lock (obj)
            // {
            for (int i = 0; i < 10000000; i++)
            {
                Temp += i + "     ";
                fileIO ( i + "     " );
            }
            //}

        }

        private void fileIO ( string v )
        {
            lock (obj)
            {
                if (!File.Exists ( "temp.txt" ))
                {
                    File.Create ( "temp.txt" );
                }
                FileStream file = File.Open ( "temp.txt", FileMode.Append );
                byte[] bytes = new byte[1024];
                bytes = Encoding.UTF8.GetBytes ( v );
                file.WriteAsync ( bytes, 0, bytes.Length );
                file.Close ( );
            }
        }

    }
}
