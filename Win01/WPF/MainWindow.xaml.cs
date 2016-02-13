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
        /// <summary>
        /// Metodo para construir el tablero del 4 en raya
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void buildBoard(int x,int y)
        {
            try
            {
                Ellipse space;//para los espacios del tablero. Igual tengo que hacer una lista para cambiar de color????
                int diametre;//tamaño de la ficha
                Grid t = (Grid)Application.Current.MainWindow.FindName("table");//gird donde se encuentra el tablero
                t.Children.Clear();//limpiamos el grid
                board = new Grid();//creamos un tablero
                board.HorizontalAlignment = HorizontalAlignment.Stretch;
                board.VerticalAlignment = VerticalAlignment.Stretch;
                board.Height = t.ActualHeight;
                board.Width = t.ActualWidth;
                Double yCell = board.Height / (x+1);//tamaño de alto de la fila, ponemos una fila de mas para mover la ficha
                Double xCell = board.Width / y;//tamaño de ancho de la columna
                //comparamos para quedarnos con el mas pequeño, asi cabe en el inicio
                if (yCell <= xCell)
                {
                    board.Height = yCell * (x + 1);
                    board.Width = yCell * y;
                    diametre = (int)yCell-5;
                }
                else
                {
                    board.Width = yCell * y;
                    board.Height = yCell * (x);
                    diametre = (int)xCell-5;
                }
                //board.Background = new SolidColorBrush(Colors.Red);//fondo del tablero
                //definimos las filas y columnas
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
                //insertamos los espacios en cada celda
                Border borderCell;
                for (int i = 1; i < board.RowDefinitions.Count; i++)
                {
                    for (int j = 0; j < board.ColumnDefinitions.Count; j++)
                    {
                        borderCell = new Border();
                        borderCell.Background = new SolidColorBrush(Colors.Red);
                        Grid.SetRow(borderCell, i);
                        Grid.SetColumn(borderCell, j);
                        board.Children.Add(borderCell);
                        space = new Ellipse();
                        space.Height = diametre;
                        space.Width = diametre;
                        space.Fill = new SolidColorBrush(Colors.White);
                        borderCell.Child = space;
                    }
                }
                Border border = new Border();//borde superior para poner la ficha
                border.Background = new SolidColorBrush(Colors.White);
                board.Children.Add(border);
                Grid.SetColumn(border, 0);
                Grid.SetRow(border, 0);
                Grid.SetColumnSpan(border, y);
                t.Children.Add(board);
                piece = new Ellipse();
                piece.Height = diametre+5;
                piece.Width = diametre+5;
                piece.Fill = new SolidColorBrush(Colors.OrangeRed);
                border.Child = piece;
                piece.HorizontalAlignment = HorizontalAlignment.Left;
                //eventos
                board.MouseMove += new MouseEventHandler(OnMouseMoveOnBoard);
                board.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseButtonOnBoard);
            }
            catch (Exception ex) { Debugger.WriteException(ex, this); }
        }
        /// <summary>
        /// Manejador del movimiento de la ficha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMoveOnBoard(object sender, MouseEventArgs e)
        {
            try
            {
                Grid b = (Grid)sender;
                double widthBoard = b.ActualWidth;
                int radio = (int)(piece.Width / 2);
                double x = e.GetPosition(b).X;
                if (x >= radio && x <= widthBoard - radio)
                {
                    piece.Margin = new Thickness(x-radio,0,0,0);
                }
                
                //Console.WriteLine(x);
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Manejador para dejar caer la ficha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseButtonOnBoard(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int numColums = confi.yDim;                
                Grid b = (Grid)sender;
                double boardWidth = b.ActualWidth;
                double x=e.GetPosition(b).X;
                double xCell = boardWidth / numColums;
                int n = 0;
                int colum=-1;
                while (n < numColums)
                {
                    if(n*xCell<=x && x<(n+1)*xCell){
                        colum=n;
                        break;
                    }
                    n++;
                }
                Debugger.Write(colum+"");
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
