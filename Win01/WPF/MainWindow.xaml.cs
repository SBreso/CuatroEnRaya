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
using System.Windows.Threading;

namespace Win01
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region
        //un flag para saber cuando se ha iniciado una partida
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
        //clase configuracion para establecerla en cada partida
        private Configuration confi;
        //el motor que controlara la matriz
        private Motor motor;
        Grid board;//tablero de juego
        double boardProportion;//proporcion del tablero, para controlar la redimension
        Ellipse piece;//ficha de jeugo
        Ellipse[,] space;//para los espacios del tablero
        private enum OPPONENT { HUMAN, PC };//posibles contrincantes
        private OPPONENT opponent;//contrincante
        int Turn { get; set; }//turno de juego
        LinearGradientBrush brushFill2PlayerOne;
        LinearGradientBrush brushFill2PlayerTwo;
        LinearGradientBrush brushStroke2PlayerOne;
        LinearGradientBrush brushStroke2PlayerTwo;
        LinearGradientBrush brushFillVictory;
        LinearGradientBrush brusStrokeVictory;
        //timer
        DispatcherTimer turnTime;
        //
        Point lastPlaceUser;
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
                turnTime = new DispatcherTimer();
                turnTime.Interval = TimeSpan.FromSeconds(1);
                turnTime.Tick += new EventHandler(turnTime_Tick);                
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex,this);
            }
        }
        private void startTurnTime()
        {
            if (confi.isTimer)
            {
                turnTime.Start();
            }
        }
        private void stopTurnTime()
        {
            turnTime.Stop();
        }
        private void resetProgressBar(int max)
        {
            if (confi.isTimer)
            {
                progressBar.Maximum = max;
                progressBar.Value = progressBar.Maximum;
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
                newGame.Objective = confi.Objective;
                newGame.Level = confi.level;
                newGame.pcOpponents = confi.createPCOpponents();
                newGame.ShowDialog();                
                if (newGame.DialogResult == true)
                {
                    //recibir los parametros
                    confi.xDim = newGame.X;
                    confi.yDim = newGame.Y;
                    confi.pcOption = newGame.Oponent;
                    confi.time = newGame.Time;
                    confi.isTimer = newGame.TimeIsChecked;
                    confi.Objective = newGame.Objective;
                    confi.level = newGame.Level;
                    //configuramos la progressBar
                    resetProgressBar(confi.time);
                    //mostramos la de jugadores
                    PlayersWinModal playersWinModal = new PlayersWinModal();
                    playersWinModal.Owner = this;                    
                    //pasar parametros, pasamos los jugadores
                    playersWinModal.PcOption = confi.pcOption;
                    confi.DefaultGamers();//creamos los jugadores por defecto 1 ó 2                   
                    playersWinModal.Players = confi.playerList;
                    playersWinModal.iconList = confi.iconList.GetRange(0,7);
                    //playersWinModal.config = confi;
                    playersWinModal.ShowDialog();
                    if (playersWinModal.DialogResult == true)
                    {
                        //recibir parametros 
                        confi.playerList = playersWinModal.Players;                        
                        //mostramos el panel
                        this.playerOne.Content = confi.playerList[0];
                        if (confi.pcOption)//si es contra la maquina, lo creo y lo muestro
                        {
                            opponent = OPPONENT.PC;
                            this.playerTwo.Content = confi.createPC();
                        }
                        else//si es contra un amigo, ya lo tengo creado por defecto, y lo muestro
                        {
                            opponent = OPPONENT.HUMAN;
                            this.playerTwo.Content = confi.playerList[1];
                        }
                        //confi.showPlayersDetail();
                        showTime(confi.isTimer);
                        this.mainDock.Visibility = Visibility.Visible;
                        buildBoard();
                        motor = new Motor(confi.xDim,confi.yDim);
                        motor.victoryEvent += new Motor.victoryDel(victoryEvent);
                        statuBar.Text = motor.version;
                        motor.Objective = confi.Objective;
                        motor.Level = confi.Objective-confi.level;
                        motor.run();
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
        /// Chequeo para ver si se pueden ver o no los jugadores
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
                if (gridPlayers.Visibility==Visibility.Visible)
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
        //parte grafica del juego, creacion del tablero y control de eventos
        #region
            /// <summary>
            /// Manejador del evento tick, para controlar el tiempo de turno
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
        private void turnTime_Tick(object sender, EventArgs e)
        {
            try
            {
                progressBar.Value--;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Metodo para construir el tablero del 4 en raya
        /// </summary>
        private void buildBoard()
        {
            try
            {               
                Turn = 1;//establecemos el turno aqui, asi cuando juego la partida de nuevo, siempre empieza el jugador 1
                int x = confi.xDim;
                int y = confi.yDim;
                //Ellipse[,]
                space = new Ellipse[x, y];//para los espacios del tablero. Igual tengo que hacer una lista para cambiar de color????                
                int diametre;//tamaño de la ficha
                Grid table = (Grid)Application.Current.MainWindow.FindName("table");//gird donde se encuentra el tablero
                table.SizeChanged += new SizeChangedEventHandler(table_OnResize);//controlar la redimension
                table.Children.Clear();//limpiamos el grid
                board = new Grid();//creamos un tablero
                board.Height = table.ActualHeight;
                board.Width = table.ActualWidth;
                Double yCell = board.Height / (x + 1);//tamaño de alto de la fila, ponemos una fila de mas para mover la ficha
                Double xCell = board.Width / y;//tamaño de ancho de la columna
                //comparamos para quedarnos con el mas pequeño, asi cabe en el inicio
                if (yCell <= xCell)
                {
                    board.Height = yCell * (x + 1);
                    board.Width = yCell * y;
                    diametre = (int)yCell - 5;
                }
                else
                {
                    board.Width = yCell * y;
                    board.Height = yCell * (x);
                    diametre = (int)xCell - 5;//le quito un poco para que parezca un tablero
                }
                //definimos las filas y columnas
                ColumnDefinition col;
                for (int i = 0; i < y; i++)
                {
                    col = new ColumnDefinition();
                    board.ColumnDefinitions.Add(col);
                }
                RowDefinition row;
                for (int i = 0; i < x + 1; i++)
                {
                    row = new RowDefinition();
                    board.RowDefinitions.Add(row);
                }
                //insertamos los espacios en cada celda dentro de un border
                Border borderCell;

                for (int i = 1; i < board.RowDefinitions.Count; i++)//empieza en el 1 xq la primera fila es para la ficha
                {
                    for (int j = 0; j < board.ColumnDefinitions.Count; j++)
                    {
                        borderCell = new Border();
                        borderCell.Background = new SolidColorBrush(Colors.Red);
                        Grid.SetRow(borderCell, i);
                        Grid.SetColumn(borderCell, j);
                        board.Children.Add(borderCell);
                        space[i - 1, j] = new Ellipse();
                        space[i - 1, j].Fill = new SolidColorBrush(Colors.White);
                        borderCell.Child = space[i - 1, j];
                    }
                }
                resizeSpace(diametre);
                Border borderPiece = new Border();//borde superior para poner la ficha
                borderPiece.Background = new SolidColorBrush(Colors.White);
                board.Children.Add(borderPiece);
                Grid.SetColumn(borderPiece, 0);
                Grid.SetRow(borderPiece, 0);
                Grid.SetColumnSpan(borderPiece, y);//colspan del num de columnas
                table.Children.Add(board);
                loadStyles();
                piece = new Ellipse();
                piece.Fill = brushFill2PlayerOne;
                piece.Stroke = brushStroke2PlayerOne;
                resizePiece(diametre);     
                borderPiece.Child = piece;
                piece.HorizontalAlignment = HorizontalAlignment.Left;
                boardProportion = board.Height / board.Width;
                //eventos
                board.MouseMove += new MouseEventHandler(board_OnMouseMove);
                board.MouseLeftButtonUp += new MouseButtonEventHandler(board_OnClick);
                //
                if (confi.isTimer)
                {
                    startTurnTime();
                }
            }
            catch (Exception ex) { Debugger.WriteException(ex, this); }
        }
        /// <summary>
        /// Manejador del evento resize del contenedor del tablero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void table_OnResize(object sender, SizeChangedEventArgs e)
        {
            try
            {
                Size newTableSize = e.NewSize;
                if (newTableSize.Height > boardProportion * newTableSize.Width)
                {
                    board.Width = newTableSize.Width;
                    board.Height = boardProportion * board.Width;
                }
                else
                {
                    board.Height = newTableSize.Height;
                    board.Width = board.Height / boardProportion;
                }
                resizeSpace(board.Width / confi.yDim - 5);
                resizePiece(board.Width / confi.yDim);
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Manejador del movimiento de la ficha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void board_OnMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (motor.mode == Motor.MODE.ON)
                {
                    Grid b = (Grid)sender;
                    double widthBoard = b.ActualWidth;
                    int radio = (int)(piece.Width / 2);
                    double x = e.GetPosition(b).X;
                    if (x >= radio && x <= widthBoard - radio)
                    {
                        piece.Margin = new Thickness(x - radio, 0, 0, 0);
                    }
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
        private void board_OnClick(object sender, MouseButtonEventArgs e)
        {
            try
            {                
                if (motor.mode == Motor.MODE.ON)
                {
                    int row;
                    int colum;
                    Grid b = (Grid)sender;
                    if (opponent == OPPONENT.PC)//contra la maquina
                    {
                        int numColums = confi.yDim;//numero de columnas                    
                        double boardWidth = b.ActualWidth;//ancho del tablero
                        double x = e.GetPosition(b).X;//posicion X del puntero respecto al tablero
                        double xCell = boardWidth / numColums;//ancho de la celda
                        colum = 0;
                        //averiguamos en que columna deja caer la ficha
                        while (colum < numColums && x >= (colum + 1) * xCell)
                        {
                            colum++;
                        }
                        row = motor.searchNextZero(colum);//si devuelve -1 la columna esta llena y toca elegir nueva columna
                        if (row != -1)//controlamos que quede espacio, si no, no camnbiamos de turno
                        {
                            lastPlaceUser = new Point(row, colum);                   
                            updateBoard(row, colum);//actualizamos tablero
                            motor.updateA(row, colum,Turn);//actualizamos matriz
                            confi.playerList[0].TiempoAcumulado = (int)(progressBar.Maximum - progressBar.Value);
                            confi.playerList[0].TiempoAcumuladoTotal =(int)(progressBar.Maximum - progressBar.Value);
                            if (motor.checkA(row, colum))//comprobamos si ha habido un cuatro en raya
                            {
                                return;
                            }                            
                            changeTurn();                           
                            //hasta aqui el turno de la persona. Ahora a la maquina
                            machineTurn();
                        }
                    }
                    else//con un amigo
                    {
                        int numColums = confi.yDim;//numero de columnas                    
                        double boardWidth = b.ActualWidth;//ancho del tablero
                        double x = e.GetPosition(b).X;//posicion X del puntero respecto al tablero
                        double xCell = boardWidth / numColums;//ancho de la celda
                        colum = 0;
                        while (colum < numColums && x >= (colum + 1) * xCell)//donde deja caer la ficha
                        {
                            colum++;
                        }
                        row = motor.searchNextZero(colum);//fila donde se colaca la ficha
                        if (row != -1)//controlamos que quede espacio, si no, no camnbiamos de turno
                        {
                            motor.updateA(row, colum, Turn);
                            updateBoard(row, colum);
                            if (Turn == 1)
                            {
                                confi.playerList[0].TiempoAcumulado = (int)(progressBar.Maximum - progressBar.Value);
                                confi.playerList[0].TiempoAcumuladoTotal = (int)(progressBar.Maximum - progressBar.Value);
                            }
                            else
                            {
                                confi.playerList[1].TiempoAcumulado = (int)(progressBar.Maximum - progressBar.Value);
                                confi.playerList[1].TiempoAcumuladoTotal = (int)(progressBar.Maximum - progressBar.Value);
                            }
                            if (motor.checkA(row, colum))
                            {
                                return;
                            }
                            changeTurn();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Controla el turno del PC
        /// </summary>
        private void machineTurn()
        {
            Random r = new Random();
            int row;
            int colum;
            int possibleColumn;
            if (confi.level!=1 && motor.search4K((int)lastPlaceUser.X, (int)lastPlaceUser.Y, out possibleColumn))
            {
                colum = possibleColumn;
                row = motor.searchNextZero(colum);
                if (row < 0)//controlamos que nos de una linea correcta
                {
                    colum = motor.randomColum();//columna al azar
                    row = motor.searchNextZero(colum);
                    while (row == -1)
                    {
                        colum = motor.randomColum();
                        row = motor.searchNextZero(colum);
                    }
                }
            }
            else
            {
                colum = motor.randomColum();//columna al azar
                row = motor.searchNextZero(colum);
                while (row == -1)
                {
                    colum = motor.randomColum();
                    row = motor.searchNextZero(colum);
                }
            }
            updateBoard(row, colum);
            if (confi.isTimer)
            { 
                int n = r.Next(3, (int)progressBar.Maximum);
                confi.playerList[1].TiempoAcumulado = n;
                confi.playerList[1].TiempoAcumuladoTotal = n;
            }
            motor.updateA(row, colum, Turn);
            if (motor.checkA(row, colum))
            {
                return;
            }
            changeTurn();
        }
        #endregion
        //metodos necesarios para el control grafico del juego
        #region
        /// <summary>
        /// Metodo para cargar los diferentes estilos: fill, stroke; para: p1,p2 y victoria 
        /// </summary>
        private void loadStyles()
        {
            try
            {
                GradientStopCollection gsc2PlayerOne = new GradientStopCollection(){
                    new GradientStop(Colors.Black, 0.066),
                    new GradientStop(confi.playerList.ElementAt(0).ColorPieza.Color,0.581)
                };
                GradientStopCollection gsc2PlayerTwo = new GradientStopCollection(){
                    new GradientStop(Colors.Black, 0.066),
                    new GradientStop(confi.playerList.ElementAt(1).ColorPieza.Color,0.581)
                };
                GradientStopCollection gscVictory = new GradientStopCollection()
                {
                    new GradientStop(Colors.Black,0.066),
                    new GradientStop(Colors.Gold,0.581)
                };
                brushFill2PlayerOne = ((LinearGradientBrush)FindResource("linearGradientFill")).Clone();
                brushFill2PlayerOne.GradientStops = gsc2PlayerOne;
                brushStroke2PlayerOne = ((LinearGradientBrush)FindResource("linearGradientStroke")).Clone();
                brushStroke2PlayerOne.GradientStops = gsc2PlayerOne;

                brushFill2PlayerTwo = ((LinearGradientBrush)FindResource("linearGradientFill")).Clone();
                brushFill2PlayerTwo.GradientStops = gsc2PlayerTwo;
                brushStroke2PlayerTwo = ((LinearGradientBrush)FindResource("linearGradientStroke")).Clone();
                brushStroke2PlayerTwo.GradientStops = gsc2PlayerTwo;

                brushFillVictory = ((LinearGradientBrush)FindResource("linearGradientFill")).Clone();
                brushFillVictory.GradientStops = gscVictory;
                brusStrokeVictory = ((LinearGradientBrush)FindResource("linearGradientStroke")).Clone();
                brusStrokeVictory.GradientStops = gscVictory;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Cambiar el jugador que esta en turno y la pieza
        /// </summary>
        private void changeTurn()
        {
            try
            {
                stopTurnTime();
                resetProgressBar(confi.time);
                if (Turn == 1)
                {
                    Turn = -1;
                    piece.Fill = brushFill2PlayerTwo;
                    piece.Stroke = brushStroke2PlayerTwo;
                }
                else
                {
                    Turn = 1;
                    piece.Fill = brushFill2PlayerOne;
                    piece.Stroke = brushStroke2PlayerOne;
                }
                resizePiece(piece.ActualHeight);
                startTurnTime();
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }        
        /// <summary>
        /// Asignar tamaño y color a la ficha del jugador
        /// </summary>
        /// <param name="diametre"></param>
        private void resizePiece(double diametre)
        {
            try
            {   
                piece.StrokeThickness = diametre / 10;
                piece.Height = diametre;
                piece.Width = diametre;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }       
        /// <summary>
        /// Asignar tamaño a los espacios del tablero
        /// </summary>
        /// <param name="diametre"></param>
        private void resizeSpace(double diametre)
        {
            try
            {
                foreach (Ellipse s in space)
                {
                    s.StrokeThickness = diametre / 10;
                    s.Height = diametre;
                    s.Width = diametre;
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Actualizar tablero
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colum"></param>
        private void updateBoard(int row, int colum)
        {
            try
            {
                if (Turn == 1)
                {
                    space[row, colum].Stroke = brushStroke2PlayerOne;
                    space[row, colum].Fill = brushFill2PlayerOne;
                }
                else
                {
                    space[row, colum].Stroke = brushStroke2PlayerTwo;
                    space[row, colum].Fill = brushFill2PlayerTwo;
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Este metodo cambia el color de las piezas que consiguen el 4 en raya. Añade una partida ganada al jugador que la ha ganado
        /// y muesra la ventana de victoria
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="des"></param>
        /// <param name="type"></param>
        private void victoryEvent(int x, int y, int des, Motor.CONNECT type)
        {
            try
            {
                stopTurnTime();
                if (type == Motor.CONNECT.NULL)
                {
                    confi.playerList.ElementAt(0).Ganadas += (float)1/2;
                    confi.playerList.ElementAt(1).Ganadas += (float)1/2;
                    showVictoryWin("", type);
                }
                else
                {
                    if (type == Motor.CONNECT.VERTICAL)
                    {
                        int k = 0;
                        while (k < motor.Objective)
                        {
                            space[x + k, y].Fill = brushFillVictory;
                            space[x + k, y].Stroke = brusStrokeVictory;
                            k++;
                        }
                        //MessageBox.Show("vertical");
                    }
                    if (type == Motor.CONNECT.HORIZONTAL)
                    {
                        int acc = 0;
                        while (acc < motor.Objective)
                        {
                            space[x, y - (des - 1) + acc].Fill = brushFillVictory;
                            space[x, y - (des - 1) + acc].Stroke = brusStrokeVictory;
                            acc++;
                        }
                        //MessageBox.Show("horizontal");
                    }
                    if (type == Motor.CONNECT.NOMAIN)
                    {
                        int acc = 0;
                        while (acc < motor.Objective)
                        {
                            space[x + (des - 1) - acc, y - (des - 1) + acc].Fill = brushFillVictory;
                            space[x + (des - 1) - acc, y - (des - 1) + acc].Stroke = brusStrokeVictory;
                            acc++;
                        }
                        //MessageBox.Show("nomain");
                    }
                    if (type == Motor.CONNECT.MAIN)
                    {
                        int acc = 0;
                        while (acc < motor.Objective)
                        {
                            space[x - (des - 1) + acc, y - (des - 1) + acc].Fill = brushFillVictory;
                            space[x - (des - 1) + acc, y - (des - 1) + acc].Stroke = brusStrokeVictory;
                            acc++;
                        }
                        //MessageBox.Show("main");
                    }
                    if (Turn == 1)
                    {
                        confi.playerList.ElementAt(0).Ganadas += 1;
                        showVictoryWin(confi.playerList.ElementAt(0).Nombre,type);
                    }
                    else
                    {
                        confi.playerList.ElementAt(1).Ganadas += 1;
                        showVictoryWin(confi.playerList.ElementAt(1).Nombre,type);
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Mostrar la ventana de fin de partida
        /// </summary>
        /// <param name="nomPlayer"></param>
        private void showVictoryWin(String nomPlayer,Motor.CONNECT type)
        {
            try
            {
                String text;
                String caption;
                if (type != Motor.CONNECT.NULL)
                {
                    text = String.Format("¡¡Enhorabuena {0}!!", nomPlayer);
                    caption = "Victoria";
                }
                else
                {
                    text = "¡¡Partida en tablas!!";
                    caption = "Empate";
                }
                MessageBoxResult victoryWin = MessageBox.Show(text + "\n¿quieres jugar una partida con el mismojugador?", caption, MessageBoxButton.YesNo);
                switch (victoryWin)
                {
                    case (MessageBoxResult.Yes):
                        {
                            motor.mode = Motor.MODE.OFF;
                            buildBoard();
                            confi.playerList[0].ResetTiempo();
                            confi.playerList[1].ResetTiempo();
                            resetProgressBar(confi.time);
                            motor.Objective = confi.Objective;
                            motor.run();
                            break;
                        }
                    default:
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
        #endregion

        private void progressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ProgressBar p = (ProgressBar)sender;
            if (p.Value == 0)
            {
                
                MessageBox.Show("Ha agotado el tiempo del turno", "Tiempo agotado", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                if (confi.pcOption ==true)
                {
                    changeTurn();
                    machineTurn();
                }
                else
                {
                    changeTurn();
                }
                //changeTurn();
            }
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
