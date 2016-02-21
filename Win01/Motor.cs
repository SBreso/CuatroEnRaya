using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Win01
{
    class Motor
    {
        public string version = "v.003";
        Grid board;
        Ellipse piece;
        Ellipse[,] space;// = new Ellipse[x, y];//para los espacios del tablero. Igual tengo que hacer una lista para cambiar de color????
        Player p1, p2;
        int turn;//controlar el turno de la partida: 1->p1 y -1->p2
        private enum OPPONENT { HUMAN, PC };
        private OPPONENT opponent;
        Configuration confi;
        int[,] A;
        double boardProportion;
        public Motor(Configuration c)
        {
            confi = c;
            //establecemos el tipo de juego
            if (confi.pcOption)
            {
                opponent = OPPONENT.PC;
            }
            else
            {
                opponent = OPPONENT.HUMAN;
            }
            p1 = confi.playerList.ElementAt(0);
            p2 = confi.playerList.ElementAt(1);
            A = new int[confi.xDim, confi.yDim];
            fillInA();
            turn = 1;
        }
        /// <summary>
        /// Llenar la matriz de ceros
        /// </summary>
        private void fillInA()
        {
            try
            {
                for (int i = 0; i < confi.xDim; i++)
                {
                    for (int j = 0; j < confi.yDim; j++)
                    {
                        A[i, j] = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// En marcha
        /// </summary>
        public void run()
        {
            buildBoard();
        }
        /// <summary>
        /// Metodo para construir el tablero del 4 en raya
        /// </summary>
        private void buildBoard()
        {
            try
            {
                int x = confi.xDim;
                int y = confi.yDim;
                //Ellipse[,]
                space = new Ellipse[x, y];//para los espacios del tablero. Igual tengo que hacer una lista para cambiar de color????                
                int diametre;//tamaño de la ficha
                Grid table = (Grid)Application.Current.MainWindow.FindName("table");//gird donde se encuentra el tablero
                table.SizeChanged += new SizeChangedEventHandler(table_OnResize);
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

                //board.Background = new SolidColorBrush(Colors.Red);//fondo del tablero
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
                        //space[i - 1, j].Height = diametre;
                        //space[i - 1, j].Width = diametre;
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
                piece = new Ellipse();
                resizePiece(diametre + 5);
                //piece.Height = diametre + 5;//le añodo lo que le quite
                //piece.Width = diametre + 5;
                piece.Fill = p1.ColorPieza;
                borderPiece.Child = piece;
                piece.HorizontalAlignment = HorizontalAlignment.Left;
                boardProportion = board.Height / board.Width;
                //eventos
                board.MouseMove += new MouseEventHandler(board_OnMouseMove);
                board.MouseLeftButtonUp += new MouseButtonEventHandler(board_OnClick);
            }
            catch (Exception ex) { Debugger.WriteException(ex, this); }
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
        /// Asignar tamaño a la ficha del jugador
        /// </summary>
        /// <param name="diametre"></param>
        private void resizePiece(double diametre)
        {
            try
            {
                piece.Height = diametre;
                piece.Width = diametre;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
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
                Grid b = (Grid)sender;
                double widthBoard = b.ActualWidth;
                int radio = (int)(piece.Width / 2);
                double x = e.GetPosition(b).X;
                if (x >= radio && x <= widthBoard - radio)
                {
                    piece.Margin = new Thickness(x - radio, 0, 0, 0);
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
                    while (colum < numColums && x >= (colum + 1) * xCell)
                    {
                        colum++;
                    }
                    row = searchNextZero(colum);//si devuelve -1 la columna esta llena y toca elegir nueva columna
                    if (row != -1)//controlamos que quede espacio, si no, no camnbiamos de turno
                    {
                        updateBoard(row, colum);
                        changeTurn();
                        //hasta aqui el turno de la persona. Ahora a la maquina
                        colum = randomColum();
                        row = searchNextZero(colum);
                        while (row == -1)
                        {
                            colum = randomColum();
                            row = searchNextZero(colum);
                        }
                        updateBoard(row, colum);
                        changeTurn();
                    }
                }
                else//con un amigo
                {
                    int numColums = confi.yDim;//numero de columnas                    
                    double boardWidth = b.ActualWidth;//ancho del tablero
                    double x = e.GetPosition(b).X;//posicion X del puntero respecto al tablero
                    double xCell = boardWidth / numColums;//ancho de la celda
                    colum = 0;
                    while (colum < numColums && x >= (colum + 1) * xCell)
                    {
                        colum++;
                    }
                    row = searchNextZero(colum);
                    if (row != -1)//controlamos que quede espacio, si no, no camnbiamos de turno
                    {
                        updateBoard(row, colum);
                        changeTurn();
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// debuelve un numero entero entre 0 y el numero de columanas. Para la maquina
        /// </summary>
        /// <returns></returns>
        private int randomColum()
        {
            Random rdm = new Random();
            return rdm.Next(0, confi.yDim);
        }
        int numPieces = 1;
        /// <summary>
        /// Actualizar tablero, matriz y cambiar turno
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colum"></param>
        private void updateBoard(int row, int colum)
        {
            try
            {
                if (turn == 1)
                {
                    A[row, colum] = 1;
                    if (numPieces > 6)//hasta que no se poenen 7 piezas no hace falta hacer chequeos
                    {
                        checkA(row, colum);
                    }
                    space[row, colum].Fill = p1.ColorPieza;
                }
                else
                {
                    A[row, colum] = -1;
                    if (numPieces > 6)//hasta que no se poenen 7 piezas no hace falta hacer chequeos
                    {
                        checkA(row, colum);
                    }
                    space[row, colum].Fill = p2.ColorPieza;
                }
                if (numPieces < 7) numPieces++;//controlar los primeros turnos
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
                if (turn == 1)
                {
                    turn = -1;
                    piece.Fill = p2.ColorPieza;
                }
                else
                {
                    turn = 1;
                    piece.Fill = p1.ColorPieza;
                }

            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Buscamos la posicion donde hay que dejar caer la ficha
        /// </summary>
        /// <param name="colum"></param>
        /// <returns></returns>
        private int searchNextZero(int colum)
        {
            try
            {
                int numRows = confi.xDim;
                int row = numRows - 1;
                while (row >= 0 && A[row, colum] != 0)
                {
                    row--;
                }
                if (row == -1)//por si llegamos arriba del tablero
                {
                    return -1;
                }
                else
                {
                    return row;
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                return -1;
            }
        }
        /// <summary>
        /// Checkea la matriz en busca de un cuatro en raya
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void checkA(int x, int y)
        {
            try
            {
                int des;//representa el desplazamiento respecto a la posicion original, para saber exactamento donde se encuentran las 4 fichas
                //hasta que no haya llegado al nivel, no hace falta hacer esta comprobacion
                if (x + 4 <= confi.xDim && checkVerticalArray(x, y, out des))
                {
                    //lanzar evento partida ganada    
                    Console.WriteLine("1\t"+des);
                } 
                //esta hay que hacerla 'siempre'               
                if (checkHorizontalArray(x, y, out des))
                {
                    //lanzar evento partida ganada con el desplazamiento
                    Console.WriteLine("2\t" + des);
                }              
                //no hace falta que haga la comprobacion si esta en las esquinas
                if (!(x < 3 && y <= 2 - x) && !(x - confi.xDim + 3 < 3 && y > confi.xDim + confi.yDim - 5 - x) && checkNoMainDiagonal(x, y, out des))
                {
                    //lanzar evento
                    Console.WriteLine("3\t" + des);
                }
                //no hace falta que haga la comprobacion si esta en las esquinas
                if ( !(x<3 && y>confi.yDim-4+x) && !(y<3 && x>confi.xDim-4+y) && checkMainDiagonal(x, y, out des))//!(x<3 && y<confi.yDim-3+x) && !(y<3 && x<confi.xDim-3+y) &&)
                {
                    Console.WriteLine("4\t" + des);
                }

            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Calcula el array vertical y devuelve si suma 4, o no
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool checkVerticalArray(int x, int y, out int des)
        {
            try
            {
                int i=1;
                while (i < 4 && A[x,y]==A[x+i,y])
                {
                    i++;
                }               
                des = i-1;
                return i==4;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                des = -1;
                return false;
            }
        }
        /// <summary>
        /// Calcula los posibles vectores horizontales y si suman 4. Si da que si, devuelve el desplazamiento respecto a la posicion dada
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="des"></param>
        /// <returns></returns>
        private bool checkHorizontalArray(int x, int y, out int des)//des es el desplazamiento respecto al origen
        {
            try
            {
                int k = 1;
                //compruevo que existe el elemento A[x,y-k]
                //compruevo que no me voy mas alla del cuatro en raya
                //compruevo que los elementos coinciden
                while (y - k >= 0 && k < 4 && A[x, y] == A[x, y - k])
                {
                    k++;
                }
                des = k;
                /*tres posibles resultados h=1,2,3
                *si k=1 tengo que hacer la comprobacion hacia la derecha hasta maximo el tercer elemento
                 *si k=2 comprobacion hacia la derecha maximo 2
                 *si k=3 comprobacion hacia la derecha de un elemento
                 *si k=4 ha habido un cuatro en raya
                 */
                int n = 1;
                switch (k)
                {
                    case 1:
                        {
                            while (y + n < confi.yDim && A[x, y] == A[x, y + n] && n <= 4 - k)
                            {
                                n++;
                            }
                            return n == 4;
                        }
                    case 2:
                        {
                            while (y + n < confi.yDim && A[x, y] == A[x, y + n] && n <= 4 - k)
                            {
                                n++;
                            }
                            return n == 3;
                        }
                    case 3:
                        {
                            while (y + n < confi.yDim && A[x, y] == A[x, y + n] && n <= 4 - k)
                            {
                                n++;
                            }
                            return n == 2;
                        }
                    case 4:
                        {
                            return true;
                        }
                    default:
                        {
                            des = -1;
                            return  false;
                        }
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                des = -1;
                return false;
            }          
        }
        /// <summary>
        /// Ckequear las posibles diagonales no principales
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="des"></param>
        /// <returns></returns>
        private bool checkNoMainDiagonal(int x, int y, out int des)
        {
            try
            {
                int k = 1;
                while (x + k < confi.xDim && y - k >= 0 && A[x, y] == A[x + k, y - k] && k < 4) { k++;}
                des = k;
                if (k == 4)//ya ha habido un un cuatro en raya
                {
                    return true;
                }
                else//toca checkear hacia la derecha
                {
                    return checkNonMainDiagonalRight(x, y, k);
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                des = -1;
                return false;
            }
        }
        private bool checkNonMainDiagonalRight(int x, int y, int k)
        {
            int n = 1;
            while (x - n >= 0 && y + n < confi.yDim && A[x, y] == A[x - n, y + n] && n <= 4 - k) { n++; }
                        return n == 4;
        }
        /// <summary>
        /// Ckequear las posibles diagonales principales
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="des"></param>
        /// <returns></returns>
        private bool checkMainDiagonal(int x, int y, out int des)
        {
            try
            {
                int k = 1;
                while (x - k >= 0 && y - k >= 0 && A[x, y] == A[x - k, y - k] && k < 4) { k++; }
                des = k;
                if (k == 4)//ya ha habido un cuatro en raya
                {
                    return true;
                }
                else//hay que chequear hacia la derecha
                {
                    return checkMainDiagonalRight(x, y, k);
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                des = -1;
                return false;
            }
        }
        private bool checkMainDiagonalRight(int x, int y, int k)
        {
            int n = 1;
            while (x + n < confi.xDim && y + n < confi.yDim && A[x, y] == A[x + n, y + n] && n <= 4 - k)
            {
                n++;
            }
            return n == 4;
        }
        /// <summary>
        /// Metodo para sumar los elementos de un array
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private int sumArray(int[] v)
        {
            try
            {
                int acc = 0;
                foreach (int a in v)
                {
                    acc += a;
                }
                return acc;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                return 0;
            }
        }
    }
}