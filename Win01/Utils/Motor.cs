using System;
using System.Windows;

namespace Win01
{
    public class Motor
    {
        //ATRIBUTOS DEL MOTOR
        #region
        public enum MODE { ON, OFF };
        public MODE mode { get; set; }
        public string version = "v.004";
        public enum CONNECT { VERTICAL, HORIZONTAL, NOMAIN, MAIN,NULL };//el tipo de cuatro en raya
        public delegate void victoryDel(int x, int y, int des, CONNECT type);
        public event victoryDel victoryEvent;
        int[,] A;//matriz de control
        int m;//filas
        int n;//columnas
        int total;//para controlar el empate
        public int Objective { get; set; }//objetivo, para poder jugar a mas opciones
        public int Level { get; set; }//nivel de deteccion
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
                int des;//representa el desplazamiento respecto a la posicion original, para saber exactamento donde se encuentran las 4 fichas
                //hasta que no haya llegado al nivel, no hace falta hacer esta comprobacion
                if (x + (Objective-1) < m && checkVertical(x, y, out des))
                {
                    //lanzar evento partida ganada    
                    //Console.WriteLine("1\t"+des);
                    victoryEvent(x, y, des, CONNECT.VERTICAL);
                    return true;
                }
                //esta hay que hacerla 'siempre'               
                else if (checkHorizontal(x, y, out des))
                {
                    //lanzar evento partida ganada con el desplazamiento
                    //Console.WriteLine("2\t" + des);
                    victoryEvent(x, y, des, CONNECT.HORIZONTAL);
                    return true;
                }
                //no hace falta que haga la comprobacion si esta en las esquinas
                else if (!(x <= Objective-2 && y <=Objective-2-x) && !(x>=m-(Objective-1) && y >= n-(x-m+Objective)) && checkNoMainDiagonal(x, y, out des))
                {
                    //lanzar evento
                    //Console.WriteLine("3\t" + des);
                    victoryEvent(x, y, des, CONNECT.NOMAIN);
                    return true;
                }
                //no hace falta que haga la comprobacion si esta en las esquinas
                else if ( !(x<=Objective-1 && y>=n-Objective+1+x) && !(x>=m-Objective+1 && y<=x-m+Objective-1) && checkMainDiagonal(x, y, out des))
                {
                    //Console.WriteLine("4\t" + des);
                    victoryEvent(x, y, des, CONNECT.MAIN);
                    return true;
                }
                else if (total == 0)
                {
                    victoryEvent(x, y, 0, CONNECT.NULL);
                    return true;
                }else
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
        //Metodos para las pruebas
        #region
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
        }
        #endregion
        public bool search4K(int x, int y,out int possibleColumn)
        {
            try
            {
                //int k = Objective - nivel;//Segun el nivel, putearemos mas o menos
                //chequeo vertical
                if(x<=m-Level && search4KVertical(x,y,out possibleColumn))
                {
                    Debugger.Write("Vertical: "+possibleColumn);
                    return true;
                }//chequeo horizontal
                else if (search4KHorizontal(x, y,out possibleColumn) )
                {
                    Debugger.Write("Horizontal 1:"+possibleColumn + "");
                    return true;
                }//hacemos una comprobacion horizontal en la fila superior, para taparlo
                else if (x - 1 >= 0 && search4KHorizontal(x - 1, y,out possibleColumn))
                {
                    Debugger.Write("Horizontal 2:" + possibleColumn + "");
                    return true;
                }//chequeo diagonalNoMain
                else if (!(x <= Objective - 2 && y <= Objective - 2 - x) && !(x >= m - (Objective - 1) && y >= n - (x - m + Objective))&& search4KNoMain(x, y,out possibleColumn))
                {
                    Debugger.Write("NoMain 1: "+possibleColumn + "");
                    return true;
                }//chequeo diagonalNoMain sobre la posicion superior
                else if (x - 1 >= 0 && !(x - 1 <= Objective - 2 && y <= Objective - 2 - x - 1) && !(x - 1 >= m - (Objective - 1) && y >= n - (x - 1 - m + Objective)) && search4KNoMain(x - 1, y, out possibleColumn))
                {
                    Debugger.Write("NoMain 2: "+possibleColumn + "");
                    return true;
                }//chequeo diagonalMain
                else if (!(x <= Objective - 1 && y >= n - Objective + 1 + x) && !(x >= m - Objective + 1 && y <= x - m + Objective - 1) && search4KMain(x, y,out possibleColumn))
                {
                    Debugger.Write("Main 1: "+possibleColumn + "");
                    return true;
                }//chequeo diagonalMain sobre la posicion superior
                else if (x - 1 >= 0 && !(x - 1 <= Objective - 1 && y >= n - Objective + 1 + x - 1) && !(x - 1 >= m - Objective + 1 && y <= x - 1 - m + Objective - 1) && search4KMain(x - 1, y, out possibleColumn))
                {
                    Debugger.Write("Main 2: "+possibleColumn + "");
                    return true;
                }
                else
                {
                    Debugger.Write(possibleColumn + "");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                possibleColumn = -1;
                return false;
            }
        }
        /// <summary>
        /// En busca de k posiciones iguales de forma vertical
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Level"></param>
        /// <returns></returns>
        private bool search4KVertical(int x, int y, out int possibleColumn)
        {
            try
            {
                int i = 1;
                while(i<Level && x+i<m && A[x, y] == A[x + i, y])
                {
                    i++;
                }
                possibleColumn = y;
                return i == Level;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                possibleColumn = -1;
                return false;
            }
        }
        /// <summary>
        /// Metodo para buscar posibles jugadas del usuario para putear
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Level"></param>
        /// <returns></returns>
        private bool search4KHorizontal(int x, int y, out int possibleColumn)
        {
            //si encuentra una columna donde colocar la ficha la devuelve, si no devuelve -1
            try
            {
                possibleColumn = -1;
                int[] v = new int[Objective];
                int j = 0;
                int pos = y - (Objective - 1) + j;                
                while(j<Objective && pos+Objective<n)
                {
                    pos = y - (Objective - 1) + j;
                    if (pos<0)//la posicion desde donde quiero comprobar esta cerca del borde izq
                    {
                        pos = y - (Objective - 1) + j;
                        j++;
                    }
                    else
                    {
                        v = buildArrayFromA(new Point(x,pos),DIRECTION.HORIZONTAL);
                        if (sumArray(v) >= Level)
                        {
                            possibleColumn= searchZeroInArray(v)+pos;
                            if(possibleColumn<n && isPosibleThisPosition(x, possibleColumn))
                            {
                                return true;
                            }
                            else
                            {
                                pos = y - (Objective - 1) + j;
                                j++;
                            }
                        }
                        else
                        {
                            pos = y - (Objective - 1) + j;
                            j++;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                possibleColumn = -1;
                return false;
            }
        }
        /// <summary>
        /// Chequea la diagonal no principal para colocar una ficha
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Level"></param>
        /// <returns></returns>
        private bool search4KNoMain(int x, int y, out int possibleColumn)
        {
            try
            {
                possibleColumn = -1;
                int possibleRow = -1;
                int[] v = new int[Objective];
                int posX = x+(Objective-1);
                int posY = y-(Objective-1);
                int j = 0;
                while (j < Objective && posX-(Objective-1)>=0 && posY+(Objective-1)<n)
                {
                   
                    if(posX>m-1 || posY < 0)
                    {                        
                        posX = x + (Objective - 1) - j;
                        posY = y - (Objective - 1) + j;
                        j++;
                    }
                    else
                    {
                        v = buildArrayFromA(new Point(posX, posY), DIRECTION.NOMAIN);
                        if (sumArray(v) >= Level)
                        {
                            int zero = searchZeroInArray(v);
                            possibleColumn = zero+posY;
                            possibleRow = posX-zero;
                            if(possibleColumn<n && isPosibleThisPosition(possibleRow, possibleColumn))
                            {
                                return true;
                            }
                            else
                            {
                                posX = x + (Objective - 1) - j;
                                posY = y - (Objective - 1) + j;
                                j++;
                            }

                        }
                        else
                        {
                            posX = x + (Objective - 1) - j;
                            posY = y - (Objective - 1) + j;
                            j++;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                possibleColumn = -1;
                return false;
            }
        }
        /// <summary>
        /// Chequea la diagonal principal para colocar una ficha
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Level"></param>
        /// <returns></returns>
        private bool search4KMain(int x, int y,out int possibleColumn)
        {
            try
            {
                possibleColumn = -1;
                int possibleRow = -1;
                int[] v = new int[Objective];
                int posX = x - (Objective - 1);
                int posY = y - (Objective - 1);
                int j = 0;
                while (j < Objective && posX + (Objective - 1) <m && posY + (Objective - 1) < n)
                {

                    if (posX <0 || posY < 0)
                    {
                        posX = x - (Objective - 1) - j;
                        posY = y - (Objective - 1) + j;
                        j++;
                    }
                    else
                    {
                        v = buildArrayFromA(new Point(posX, posY), DIRECTION.MAIN);
                        if (sumArray(v) >= Level)
                        {
                            int zero = searchZeroInArray(v);
                            possibleColumn = zero + posY;
                            possibleRow = posX - zero;
                            if (possibleColumn < n && isPosibleThisPosition(possibleRow, possibleColumn))
                            {
                                return true;
                            }
                            else
                            {
                                posX = x - (Objective - 1) - j;
                                posY = y - (Objective - 1) + j;
                                j++;
                            }

                        }
                        else
                        {
                            posX = x - (Objective - 1) - j;
                            posY = y - (Objective - 1) + j;
                            j++;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                possibleColumn = -1;
                return false;
            }
        }
        private enum DIRECTION { HORIZONTAL, NOMAIN, MAIN};
        /// <summary>
        /// Construir un array dado una posicion y el sentido
        /// </summary>
        /// <param name="x"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int[] buildArrayFromA(Point p,DIRECTION type)
        {
            int[] v = new int[Objective];
            if (type == DIRECTION.HORIZONTAL)
            {
                int j = 0;
                while (j < Objective && p.Y + j < n)
                {
                    v[j] = A[(int)p.X, (int)p.Y + j];
                    j++;
                }
                return v;
            }else if (type == DIRECTION.NOMAIN)
            {
                int j = 0;
                while(j<Objective && (int)p.X+(Objective-1)-j>=0 && (int)p.Y - (Objective - 1) + j < n)
                {
                    v[j] = A[(int)p.X  - j, (int)p.Y + j];
                    j++;
                }
                return v;
            }else//DIRECTION.MAIN
            {
                int j = 0;
                while (j < Objective && (int)p.X - (Objective - 1) + j <m && (int)p.Y - (Objective - 1) + j < n)
                {
                    v[j] = A[(int)p.X  + j, (int)p.Y + j];
                    j++;
                }
                return v;
            }
            
        }
        /// <summary>
        /// Esta funcion nos dice si puedo poner una ficha en una posicion o no
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool isPosibleThisPosition(int x, int y)
        {
            try
            {
                if (x == m - 1)//la fila inferior
                {
                    return A[x, y] == 0;                    
                }
                else
                {
                    return (A[x + 1, y] != 0 && A[x,y]==0);
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                return false;
            }
        }
        /// <summary>
        /// Recorre un vector en busca de un elemento cero
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private int searchZeroInArray(int[] v)
        {
            int i = 0;
            while(i<v.Length && v[i] != 0)
            {
                i++;
            }
            if (i < v.Length)
            {
                return i;
            }
            else
            {
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
                return Math.Abs(acc);
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                return 0;
            }
        }
    }
}