using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Dictionary;

namespace Win01
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region
        //un flag para saber cuando se ha iniciado una partida
        //cuando se inicie o finalice la partida habra que cambiar el estado
        private bool flag = true;
        //metodo para cambiar el estado de la bandera
        private void changeFlag()
        {
            if (flag)
            {
                flag = false;
            }
            else
            {
                flag = true;
            }
        }
        //calse configuracion para establecerla en cada partida
        private Configuration confi;
        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            List<SolidColorBrush> colorsList = ((Array)FindResource("Colors")).Cast<SolidColorBrush>().ToList();
            List<BitmapImage> iconList = ((Array)FindResource("Icons")).Cast<BitmapImage>().ToList();           
            InitializeComponent();
            confi = new Configuration(iconList,colorsList);
        }
        /// <summary>
        /// Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
               this.mainDock.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Debugger.Print(ex,this);
            }
        }
        //Checks y execute de los comandos
        #region
        /// <summary>
        /// Verificamos si se puede iniciar una nueva partida
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkGameOnCourse(object sender, CanExecuteRoutedEventArgs e)
        {
            if (flag)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
            
        }
        /// <summary>
        /// Iniciar una nueva partida. Abre la ventana NewGameWinModal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newGame(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                NewGameWinModal newGame = new NewGameWinModal();
                //pasar los parametros
                newGame.Owner = this;
                newGame.oponent = confi.pcOption;
                newGame.x = confi.xDim;
                newGame.y = confi.yDim;
                newGame.time = confi.time;
                newGame.ShowDialog();
                if (newGame.DialogResult == true)
                {
                    //recibir los parametros
                    confi.xDim = newGame.x;
                    confi.yDim = newGame.y;
                    confi.pcOption = newGame.oponent;
                    confi.time = newGame.time;
                    confi.isTimer = newGame.timeIsChecked;
                    //mostramos la de jugadores
                    PlayersWinModal playersWinModal = new PlayersWinModal();
                    playersWinModal.Owner = this;                    
                    //pasar parametros, pasamos los jugadores
                    playersWinModal.pcOption = confi.pcOption;
                    confi.DefaoultGamers();//creamos los jugadores por defecto 1 ó 2
                    playersWinModal.players = confi.playerList;
                    playersWinModal.ShowDialog();
                    if (playersWinModal.DialogResult == true)
                    {
                        //recibir parametros 
                        confi.playerList = playersWinModal.players;
                        //mostramos el panel
                        this.playerOne.Content = confi.playerList[0];
                        if (confi.pcOption)//si es contra la maquina, lo creo y lo muestro
                        {
                            this.playerTwo.Content = confi.createPC();                                 
                        }
                        else//si es contra un amigo, ya lo tengo creado por defecto, y lo muestro
                        {
                            this.playerTwo.Content = confi.playerList[1];
                        }
                        this.mainDock.Visibility = Visibility.Visible;
                        changeFlag();
                    }
                    else//si damos a cancelar hay que resetear los jugadores
                    {
                        confi.clearPlayers();
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.Print(ex, this);
            }
        }
        /// <summary>
        /// Verificamos si hay una partida en marcha para mostrar o no los jugadores
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkShowPlayers(object sender, CanExecuteRoutedEventArgs e)
        {
            if (flag)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }
        }
        /// <summary>
        /// Mostrar, o no, el panel de jugadores
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showPlayers(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                //Es raro porque esta chequeado de inicio y lo marca como false ????
                if (!this.itemShowPlayers.IsChecked)
                {
                    this.gridPlayers.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.gridPlayers.Visibility = Visibility.Visible;
                }   
                
            }
            catch (Exception ex)
            {
                Debugger.Print(ex, this);
            }
        }
        #endregion
       
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
