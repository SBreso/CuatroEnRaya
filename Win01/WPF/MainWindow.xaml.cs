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
        Ellipse piece;
        Grid board;
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
                        this.mainDock.Visibility = Visibility.Visible;
                        buildBoard(confi.xDim, confi.yDim);
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
        /// Metodo para construir el tablero del 4 en raya
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void buildBoard(int x,int y)
        {
            try
            {
                Ellipse space;
                int radio;
                Grid t = (Grid)Application.Current.MainWindow.FindName("table");//gird donde se encuentra el tablero
                t.Children.Clear();
                board = new Grid();                
                Double tableActualHeight = t.ActualHeight;
                Double tableActualWidth = t.ActualWidth;
                board.Height = tableActualHeight;
                board.Width = tableActualHeight;
                Double yCell = board.Height / (x+1);
                Double xCell = board.Width / y;
                if (yCell <= xCell)
                {
                    board.Height = yCell * (x + 1);
                    board.Width = yCell * y;
                    radio = (int)yCell;
                }
                else
                {
                    board.Width = yCell * y;
                    board.Height = yCell * (x);
                    radio = (int)xCell;
                }
                //board.Margin = new Thickness(0,50 ,0, 0);
                board.Background = new SolidColorBrush(Colors.Red);
                ColumnDefinition col;
                for (int i = 0; i < y; i++)
                {
                    col = new ColumnDefinition();                    
                    board.ColumnDefinitions.Add(col);
                }
                RowDefinition row;
                for (int i = 0; i < x+1; i++)
                {
                    row = new RowDefinition();
                    board.RowDefinitions.Add(row);
                }
                for (int i = 1; i < board.RowDefinitions.Count; i++)
                {
                    for (int j = 0; j < board.ColumnDefinitions.Count; j++)
                    {
                        space = new Ellipse();
                        space.Height = radio;
                        space.Width = radio;
                        space.Fill = new SolidColorBrush(Colors.Purple);
                        board.Children.Add(space);
                        Grid.SetRow(space, i);
                        Grid.SetColumn(space, j);
                    }
                }
                Border border = new Border();
                border.Background = new SolidColorBrush(Colors.Blue);
                board.Children.Add(border);
                Grid.SetColumn(border, 0);
                Grid.SetRow(border, 0);
                Grid.SetColumnSpan(border, y);
                t.Children.Add(board);
                piece = new Ellipse();
                piece.Height = radio;
                piece.Width = radio;
                piece.Fill = new SolidColorBrush(Colors.Pink);
                border.Child = piece;
                piece.HorizontalAlignment = HorizontalAlignment.Left;
                board.ShowGridLines = true;
                board.MouseMove += new MouseEventHandler(OnMouseMoveOnBoard);
            }
            catch (Exception ex) { Debugger.WriteException(ex, this); }
        }

        private void OnMouseMoveOnBoard(object sender, MouseEventArgs e)
        {
            try
            {
                Grid b = (Grid)sender;
                double x = e.GetPosition(b).X;
                piece.Margin = new Thickness(x,0,0,0);
                Console.WriteLine(x);
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        #endregion

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            buildBoard(4,4);
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
