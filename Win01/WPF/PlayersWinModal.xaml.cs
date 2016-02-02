using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Win01
{
    /// <summary>
    /// Lógica de interacción para PlayersModal.xaml
    /// </summary>
    public partial class PlayersWinModal : Window
    {
        public List<Player> players = new List<Player>();
        public bool pcOption;

        public PlayersWinModal()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tabControl.ItemsSource = players;
            tabControl.SelectedIndex = 0;
        }

        private void button_click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void button_click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;          
        }

        private void openFileDialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Archivos de imágen (.jpg)|*.jpg|All Files (*.*)|*.*";
            openFile.Multiselect = false;
            if (openFile.ShowDialog() == true)
            {
                players[tabControl.SelectedIndex].URIFoto = openFile.FileName.ToString();
            }
        }
    }
}
