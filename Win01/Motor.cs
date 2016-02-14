using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public string version = "v.001";
        Grid board;
        Ellipse piece;
        Ellipse[,] space;// = new Ellipse[x, y];//para los espacios del tablero. Igual tengo que hacer una lista para cambiar de color????
        Player p1, p2;
        int turn;//controlar el turno de la partida: 1->p1 y -1->p2
        private enum OPPONENT { HUMAN, PC };
        private OPPONENT opponent;
        Configuration confi;
        int[,] A;
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
                Grid t = (Grid)Application.Current.MainWindow.FindName("table");//gird donde se encuentra el tablero
                t.Children.Clear();//limpiamos el grid
                board = new Grid();//creamos un tablero
                board.Height = t.ActualHeight;
                board.Width = t.ActualWidth;
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
                    diametre = (int)xCell - 5;
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
                        space[i - 1, j] = new Ellipse();
                        space[i - 1, j].Height = diametre;
                        space[i - 1, j].Width = diametre;
                        space[i - 1, j].Fill = new SolidColorBrush(Colors.White);
                        borderCell.Child = space[i - 1, j];
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
                piece.Height = diametre + 5;
                piece.Width = diametre + 5;
                piece.Fill = p1.ColorPieza;
                border.Child = piece;
                piece.HorizontalAlignment = HorizontalAlignment.Left;
                //eventos
                board.MouseMove += new MouseEventHandler(board_OnMouseMove);
                board.MouseLeftButtonUp += new MouseButtonEventHandler(board_OnClick);
            }
            catch (Exception ex) { Debugger.WriteException(ex, this); }
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
                if (opponent == OPPONENT.PC && turn == -1)
                {
                    colum = randomColum();
                    row = searchNextZero(colum);
                }
                else
                {
                    Grid b = (Grid)sender;
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
                }
                updateBoard(row, colum);

            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }

        private int randomColum()
        {
            Random rdm = new Random();
            return rdm.Next(0, confi.yDim);
        }
        /// <summary>
        /// Actualizar tablero y cambiar turno
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colum"></param>
        private void updateBoard(int row, int colum)
        {
            try
            {
                //si devuelve -1 es porque ya no queda espacio donde dejar la ficha
                if (row != -1)
                {
                    updateA(row, colum);
                    changeTurn();
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Actualiza la matriz y deja caer la ficha. Aqui tocara controlar si se ha producido el cuatro en raya
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colum"></param>
        private void updateA(int row, int colum)
        {
            try
            {
                if (turn == 1)
                {
                    A[row, colum] = 1;
                    space[row, colum].Fill = p1.ColorPieza;
                }
                else
                {
                    A[row, colum] = -1;
                    space[row, colum].Fill = p2.ColorPieza;
                }
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
                int row = numRows;
                while (row >= 0 && A[row - 1, colum] != 0)
                {
                    row--;
                }
                if (row == 0)
                {
                    return -1;
                }
                else
                {
                    return row - 1;
                }
            }//por si llegamos arriba del tablero
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
