using System;

namespace Win01
{
    public class Motor
    {
        //ATRIBUTOS DEL MOTOR
        #region
        public enum MODE { ON, OFF };
        public MODE mode { get; set; }
        public string version = "v.004";
        public enum FOUR_CONNECT { VERTICAL, HORIZONTAL, NOMAIN, MAIN,NULL };//el tipo de cuatro en raya
        public delegate void victoryDel(int x, int y, int des, FOUR_CONNECT type);
        public event victoryDel victoryEvent;
        int[,] A;//matriz de control
        int m;//filas
        int n;//columnas
        int total;//para controlar el empate
        public int Objective { get; set; }//objetivo, para poder jugar a mas opciones
        #endregion
        public Motor(int xDim, int yDim)
        {
            mode = MODE.OFF;
            m = xDim;
            n = yDim;
            A = new int[m, n];
            
        }
        /// <summary>
        /// Llenar la matriz de ceros
        /// </summary>
        private void fillInA()
        {
            try
            {
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
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
            total = m * n;
            mode = MODE.ON;
            fillInA();
        }
        public int randomColum()
        {
            Random rdm = new Random();
            return rdm.Next(0, n);
        }
        /// <summary>
        /// Actualizar tablero, matriz y cambiar turno
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colum"></param>
        public void updateA(int row, int colum, int turn)
        {
            try
            {
                if (turn == 1)
                {
                    A[row, colum] = 1;
                }
                else
                {
                    A[row, colum] = -1;
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
        public int searchNextZero(int colum)
        {
            try
            {
                int row = m - 1;
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
        public bool checkA(int x, int y)
        {
            try
            {
                total--;
                if (total == 0)
                {                    
                    victoryEvent(x, y, 0, FOUR_CONNECT.NULL);
                    return true;
                }
                //return true;
                int des;//representa el desplazamiento respecto a la posicion original, para saber exactamento donde se encuentran las 4 fichas
                //hasta que no haya llegado al nivel, no hace falta hacer esta comprobacion
                if (x + (Objective-1) < m && checkVertical(x, y, out des))
                {
                    //lanzar evento partida ganada    
                    //Console.WriteLine("1\t"+des);
                    victoryEvent(x, y, des, FOUR_CONNECT.VERTICAL);
                    return true;
                }
                //esta hay que hacerla 'siempre'               
                else if (checkHorizontal(x, y, out des))
                {
                    //lanzar evento partida ganada con el desplazamiento
                    //Console.WriteLine("2\t" + des);
                    victoryEvent(x, y, des, FOUR_CONNECT.HORIZONTAL);
                    return true;
                }
                //no hace falta que haga la comprobacion si esta en las esquinas
                else if (!(x < Objective-1 && y <Objective-1) && !(x>m-(Objective-1) && y > n-(Objective-1)) && checkNoMainDiagonal(x, y, out des))
                {
                    //lanzar evento
                    //Console.WriteLine("3\t" + des);
                    victoryEvent(x, y, des, FOUR_CONNECT.NOMAIN);
                    return true;
                }
                //no hace falta que haga la comprobacion si esta en las esquinas
                else if (!(x < 3 && y > n - 4 + x) && !(y < 3 && x > m - 4 + y) && checkMainDiagonal(x, y, out des))//!(x<3 && y<confi.yDim-3+x) && !(y<3 && x<confi.xDim-3+y) &&)
                {
                    //Console.WriteLine("4\t" + des);
                    victoryEvent(x, y, des, FOUR_CONNECT.MAIN);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                return false;
            }
        }
        /// <summary>
        /// Devuelve true si se ha producido un 4 en raya vertical
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool checkVertical(int x, int y, out int des)
        {
            try
            {
                int i = 1;
                while (i < Objective && A[x, y] == A[x + i, y])
                {
                    i++;
                }
                des = i - 1;
                return i == Objective;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                des = -1;
                return false;
            }
        }
        /// <summary>
        /// Devuelve true si se ha producido un 4 en raya horizontal
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="des"></param>
        /// <returns></returns>
        private bool checkHorizontal(int x, int y, out int des)//des es el desplazamiento respecto al origen
        {
            try
            {
                int k = 1;
                //compruevo que existe el elemento A[x,y-k]
                //compruevo que no me voy mas alla del cuatro en raya
                //compruevo que los elementos coinciden
                while (y - k >= 0 && k < Objective && A[x, y] == A[x, y - k])
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
                if (k == Objective)
                {
                    return true;
                }
                else
                {
                    return checkHorizontalRight(x, y, k);
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                des = -1;
                return false;
            }
        }
        private bool checkHorizontalRight(int x, int y, int k)
        {
            int j = 1;
            while (y + j < n && A[x, y] == A[x, y + j] && j <= Objective - k)
            {
                j++;
            }
            return k + j == Objective+1;
        }
        /// <summary>
        /// Devuelve true si se ha producido un 4 en raya en la diagonal no principal
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="des"></param>
        /// <returns></returns>
        private bool checkNoMainDiagonal(int x, int y, out int des)
        {
            try
            {
                int i = 1;
                while (x + i < m && y - i >= 0 && A[x, y] == A[x + i, y - i] && i < Objective) { i++; }
                des = i;
                if (i == Objective)//ya ha habido un un cuatro en raya
                {
                    return true;
                }
                else//toca checkear hacia la derecha
                {
                    return checkNonMainDiagonalRight(x, y, i);
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
            int i = 1;
            while (x - i >= 0 && y + i < n && A[x, y] == A[x - i, y + i] && i <= Objective - k) { i++; }
            return k + i == Objective+1;
        }
        /// <summary>
        /// Devuelve true si se ha producido un 4 en raya en la diagonal principal
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
                while (x - k >= 0 && y - k >= 0 && A[x, y] == A[x - k, y - k] && k < Objective) { k++; }
                des = k;
                if (k == Objective)//ya ha habido un cuatro en raya
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
            int i = 1;
            while (x + i < m && y + i < n && A[x, y] == A[x + i, y + i] && i <= Objective - k)
            {
                i++;
            }
            return k + i == Objective+1;
        }

        private void showA()
        {
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(A[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        private void fillADifferent()
        {
            int k = 0;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    A[i, j] = k++;
                }
            }
        }
        public void fillAToTest(int option)
        {
            fillADifferent();
            switch (option)
            {
                case 0:
                    {
                        //la matriz es toda diferente   
                        break;
                    }
                case 1:
                    {
                        for (int i = 0; i <4; i++)
                        {
                            A[i, 0] = 1;//columna que hace cuatro en raya                           
                        }
                        break;
                    }
                case 2:
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            A[0, j] = 1;//esta es la fila del cuatro en raya
                        }
                            break;
                    }
                case 3:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            A[i, i] = 1;//diagonal en la principal                            
                        }
                        break;
                    }
                case 4:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            A[m-1-i, 0+i] = 1;//diagonal no principal
                        }
                        break;
                    }
            }
            showA();
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