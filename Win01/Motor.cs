﻿using System;
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
        public string version = "v.002";
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
                    checkA(row,colum);
                    space[row, colum].Fill = p1.ColorPieza;
                }
                else
                {
                    A[row, colum] = -1;
                    checkA(row, colum);
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
                int des;
                //hasta que no haya llegado al nivel, no hace falta hacer esta comprobacion
                if (x + 4 <= confi.xDim && checkVerticalArray(x, y, out des)==4)
                {
                    //lanzar evento partida ganada    
                    Console.WriteLine("1");
                } 
                //esta hay que hacerla 'siempre'               
                if (checkHorizontalArray(x, y, out des)==4)
                {
                    //lanzar evento partida ganada con el desplazamiento
                    Console.WriteLine("2");
                }              
                //no hace falta que haga la comprobacion si esta en las esquinas
                if (!(x < 3 && y <= 2 - x) && !(x - confi.xDim + 3 < 3 && y > confi.xDim + confi.yDim - 5 - x) && checkNoMainDiagonal(x, y, out des)==4)
                {
                    //lanzar evento
                    Console.WriteLine("3");
                }
                //controlar que no entre en las esquinas
                if (checkMainDiagonal(x, y, out des) == 4)
                {
                    Console.WriteLine("4");
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
        private int checkVerticalArray(int x, int y, out int des)
        {
            try
            {
                int sum = -1;
                int i=1;
                while (i < 4 && A[x,y]==A[x+i,y])
                {
                    i++;
                }
                if (i == 4)
                {
                    sum = 4;
                }
                des = x;
                return sum;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                des = -1;
                return 0;
            }
        }

        /*
         * se puede optimizar la comprobacion, haciendo calculos intermedios
         * para que no siga haciendolo en caso de que se produzcan ciertas condicionesç
         */
        /// <summary>
        /// Calcula los posibles vectores horizontales y si suman 4. Si da que si, devuelve el desplazamiento respecto a la posicion dada
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="des"></param>
        /// <returns></returns>
        private int checkHorizontalArray(int x, int y, out int des)//des es el desplazamiento respecto al origen
        {
            try
            {
                int[] v = new int[4];
                int sum = -1;
                int sumTem = 0;
                //int[,] B = new int[4, 4];//en cada fila guardare los posibles vectores, si no se puede formar se generara en cero
                //for (int k = 3; k >= 0; k--)
                int k = 3;
                while (k >= 0)
                {
                    if (y - k >= 0 && y + (3 - k) < confi.yDim)//puedo construir el vector horizontal
                    {
                        //no se, no se
                        int j = 1;
                        while (j < 4 && A[x, y] == A[x, y - k + j])
                        {
                            j++;
                        }
                        //comprobacion si todos son iguales, o no
                        if (j < 4)//son distinto, paso al siguiente vector
                        {
                            k--;
                        }
                        else//son iguales, ya he encontrado un 4 en raya
                        {
                            des=k;
                            return 4;
                        }
                        //sum = j;
                        //for (int j = 0; j < 4; j++)//lo construyo
                        //{
                        //    v[j] = A[x, y - k + j];
                        //}
                        //sum = Math.Abs(sumArray(v));
                        //if ( sum== 4)//suma 4? paro de hacer cosas
                        //{
                        //    break;
                        //}
                        //else
                        //{
                        //    k--;
                        //}
                    }//no puedo construirlo, paso a la siguiente posibilidad
                    else//no suma 4...sigo probando
                    {
                        k--;
                    }
                }
                if (k < 0)
                {
                    des = -1;
                    return sum;
                }
                else
                {
                    des = k;
                    return sum;
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                des = -1;
                return -1;
            }
        }
        /// <summary>
        /// Ckequear las posibles diagonales no principales
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="des"></param>
        /// <returns></returns>
        private int checkNoMainDiagonal(int x, int y, out int des)
        {
            try
            {
                int[] v = new int[4];
                int sum = -1;
                //controlar que x e y pueden formar vector en la llamada
                int k = 3;
                while (k >= 0)
                {
                    if (x + k < confi.xDim && y - k >=0 && x - (3 - k) >= 0 && y+(3-k)<confi.yDim)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            v[j] = A[x + k - j, y - k + j];
                        }
                        sum = Math.Abs(sumArray(v));
                        if (sum == 4)
                        {
                            break;
                        }
                        else
                        {
                            k--;
                        }
                    }
                    else
                    {
                        k--;
                    }
                }
                if (k < 0)
                {
                    des = -1;
                    return sum;
                }
                else
                {
                    des = k;
                    return sum;
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                des = -1;
                return -1;
            }
        }
        /// <summary>
        /// Ckequear las posibles diagonales principales
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="des"></param>
        /// <returns></returns>
        private int checkMainDiagonal(int x, int y, out int des)
        {
            try
            {
                int[] v = new int[4];
                int sum = -1;
                //controlar que x e y pueden formar vector en la llamada
                int k = 3;
                while (k >= 0)
                {
                    if (x + k < confi.xDim && y + k < confi.yDim && y - (3 - k) >= 0 && x - (3 - k) >= 0)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            v[j] = A[x + k - j, y + k - j];
                        }
                        sum = Math.Abs(sumArray(v));
                        if (sum== 4)
                        {
                            break;
                        }
                        else
                        {
                            k--;
                        }
                    }
                    else
                    {
                        k--;
                    }
                }
                if (k < 0)
                {
                    des = -1;
                    return sum;
                }
                else
                {
                    des = k;
                    return sum;
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                des = -1;
                return -1;
            }
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