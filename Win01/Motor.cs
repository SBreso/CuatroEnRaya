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
        Player p1, p2, playerOnTurn;
        private enum OPONENT{HUMAN,PC};
        private OPONENT value;
        Configuration confi;
        public Motor(Configuration c)
        {
            playerOnTurn=new Player();
            playerOnTurn = p1;
            confi = c;
            //establecemos el tipo de juego
            if (confi.pcOption)
            {
                value = OPONENT.PC;
            }
            else
            {
                value = OPONENT.HUMAN;
            }
            p1 = confi.playerList.ElementAt(0);
            p2 = confi.playerList.ElementAt(1);
        }
        public void run()
        {
            buildBoard();
        }
        /// <summary>
        /// Metodo para construir el tablero del 4 en raya
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void buildBoard()
        {
            try
            {
                int x = confi.xDim;
                int y = confi.yDim;
                Ellipse space;//para los espacios del tablero. Igual tengo que hacer una lista para cambiar de color????
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
                piece.Height = diametre + 5;
                piece.Width = diametre + 5;
                piece.Fill = p1.ColorPieza;
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
        private void OnMouseButtonOnBoard(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int numColums = confi.yDim;
                Grid b = (Grid)sender;
                double boardWidth = b.ActualWidth;
                double x = e.GetPosition(b).X;
                double xCell = boardWidth / numColums;
                int n = 0;
                int colum = -1;
                while (n < numColums)
                {
                    if (n * xCell <= x && x < (n + 1) * xCell)
                    {
                        colum = n;
                        break;
                    }
                    n++;
                }
                changeTurn();
                Debugger.Write(colum + "");
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Cambiar el jugador que esta en turno
        /// </summary>
        private void changeTurn()
        {
            try
            {
                if (playerOnTurn.Equals(p1))
                {
                    playerOnTurn = p2;                    
                }
                else
                {
                    playerOnTurn = p1;
                }
                piece.Fill = playerOnTurn.ColorPieza;
            }
            catch(Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }

        private void updateBoard(int col)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
    }
}
