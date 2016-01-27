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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Win01
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void checkNewGame(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void newGame(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("hola");
        }
    }
    //Comandos    
    public static class Commands
    {
        public static readonly RoutedUICommand NewGame = new RoutedUICommand(
            "Accion cuando se pulsa",//descripcion
            "NewGame",//accion
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.N,ModifierKeys.Control)
            }
            );
        public static readonly RoutedUICommand ShowGamers = new RoutedUICommand(
            "Accion cuando se pulsa",//descripcion
            "ShowGamers",//accion
            typeof(Commands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.V,ModifierKeys.Control)
            }
            );
    }
}
