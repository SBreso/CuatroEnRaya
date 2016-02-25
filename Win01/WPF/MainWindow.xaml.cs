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
        private Motor motor;
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
            Debugger d = new Debugger(Debugger.ENVIORMENT.DEVELOPMENT);
            //Debugger d = new Debugger(Debugger.ENVIORMENT.PRODUCTION);
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
                Debugger.WriteException(ex,this);
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
                newGame.Oponent = confi.pcOption;
                newGame.X = confi.xDim;
                newGame.Y = confi.yDim;
                newGame.Time = confi.time;
                newGame.ShowDialog();
                if (newGame.DialogResult == true)
                {
                    //recibir los parametros
                    confi.xDim = newGame.X;
                    confi.yDim = newGame.Y;
                    confi.pcOption = newGame.Oponent;
                    confi.time = newGame.Time;
                    confi.isTimer = newGame.TimeIsChecked;
                    //mostramos la de jugadores
                    PlayersWinModal playersWinModal = new PlayersWinModal();
                    playersWinModal.Owner = this;                    
                    //pasar parametros, pasamos los jugadores
                    playersWinModal.PcOption = confi.pcOption;
                    confi.DefaultGamers();//creamos los jugadores por defecto 1 ó 2
                    playersWinModal.Players = confi.playerList;
                    //playersWinModal.config = confi;
                    playersWinModal.ShowDialog();
                    if (playersWinModal.DialogResult == true)
                    {
                        //recibir parametros 
                        confi.playerList = playersWinModal.Players;
                        //confi= playersWinModal.config;
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
                        showTime(confi.isTimer);
                        this.mainDock.Visibility = Visibility.Visible;
                        motor = new Motor(confi);
                        motor.victoryWinEvent += new Motor.victoryWindel(showVictoryWin);
                        statuBar.Text = motor.version;
                        motor.run();
                        //buildBoard(confi.xDim, confi.yDim);
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
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Recibimos el evento de partida ganada
        /// </summary>
        /// <param name="nomPlayer"></param>
        private void showVictoryWin(String nomPlayer)
        {
            try
            {
                String text = String.Format("¡¡Enhorabuena {0}!!\n¿quieres jugar una partida con el mismojugador?",nomPlayer);
                MessageBoxResult victoryWin = MessageBox.Show(text, "Victoria", MessageBoxButton.YesNo);
                switch (victoryWin)
                {
                    case(MessageBoxResult.Yes):
                    {
                        motor.mode = Motor.MODE.OFF;
                        motor.run();
                        break;
                    }
                    case(MessageBoxResult.No):{
                        changeFlag();//para poder iniciar una nueva partida
                        motor.mode = Motor.MODE.OFF;
                        confi.resetConfi();//reseteamos la configuracion a los valores por defecto
                        break;
                    }
                    case (MessageBoxResult.Cancel):
                        {
                            changeFlag();//para poder iniciar una nueva partida
                            motor.mode = Motor.MODE.OFF;
                            confi.resetConfi();//reseteamos la configuracion a los valores por defecto
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
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
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Para mostrar la barra de tiempo, o no.
        /// </summary>
        /// <param name="b"></param>
        private void showTime(bool b)
        {
            try
            {
                if (b)
                {
                    TimeRectabgle.Visibility = Visibility.Visible;
                }
                else
                {
                    TimeRectabgle.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
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
