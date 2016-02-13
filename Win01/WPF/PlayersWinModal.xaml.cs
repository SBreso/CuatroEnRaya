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
        public List<Player> Players { get { return players; } set { players = value; } }
        public bool PcOption { get { return pcOption; } set { pcOption = value; } }
        private List<Player> players;
        private bool pcOption;

        public PlayersWinModal()
        {
            InitializeComponent();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tabControl.ItemsSource =players;
            tabControl.SelectedIndex = 0;
            
            //tabControl.GetBindingExpression(Image.SourceProperty).UpdateTarget();
        }
        /// <summary>
        /// Aceptar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        /// <summary>
        /// Cancelar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;          
        }

        private void openFileDialog(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "Archivos de imágen (.jpg)|*.jpg|All Files (*.*)|*.*";
                openFile.Multiselect = false;
                if (openFile.ShowDialog() == true)
                {
                    players[tabControl.SelectedIndex].URIFoto = openFile.FileName;
                    //players[tabControl.SelectedIndex].RaisePropertyChanged("URIFoto");
                }
                //tabControl.Items.Refresh();
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Controlamos que el nombre del jugador cambie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox textBox = (TextBox)sender;
                players[tabControl.SelectedIndex].Nombre = textBox.Text;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
    }
}
