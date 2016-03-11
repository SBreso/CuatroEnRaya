using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Win01
{
    public class Configuration
    {
        private int defaultIcon2PlayerOne;
        private int defaultIcon2PlayerTwo;
        private int defaultColor2PlayerOne;
        private int defaultColor2PlaterTwo;
        public bool pcOption;
        public bool isTimer;
        public int xDim;
        public int yDim;
        public int time;
        public int Objective;
        public List<Player> playerList;
        public List<SolidColorBrush> colorsList;
        public List<BitmapImage> iconList;
        public int level;
        
        public String[] machineName;
        public Configuration(List<BitmapImage> il, List<SolidColorBrush> cl)
        {
            iconList = il;
            colorsList = cl;
            resetConfi();
        }

        public void resetConfi()
        {            
            pcOption = true;
            xDim = 6;
            yDim = 6;
            time = 10;
            level = 1;
            Objective = 4;
            playerList = new List<Player>();
            loadDefaultIconAndColor();
            machineName = new String[5] { "Han Solo", "Soldado Imperial", "Boba Fett", "Darth Vader", "Emperador Palpatine" };
        }
        public void DefaultGamers()
        {
            try
            {
                Player p1 = new Player();
                p1.Nombre = "Jugador 1";
                p1.Foto = iconList[defaultIcon2PlayerOne];
                p1.ColorPieza = colorsList[defaultColor2PlayerOne];
                playerList.Add(p1);
                if (pcOption)//vs maquina
                {

                    
                }
                else//vs jugador
                {
                    Player p2 = new Player();
                    p2.Nombre = "Jugador 2";
                    p2.Foto = iconList[defaultIcon2PlayerTwo];
                    p2.ColorPieza = colorsList[defaultColor2PlaterTwo];
                    playerList.Add(p2);
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }

        public void clearPlayers()
        {
            try
            {
                playerList.Clear();
            }
            catch (Exception ex) 
            {
                Debugger.WriteException(ex, this);
            }
        }

        private void loadDefaultIconAndColor()
        {
            try
            {
                Random r = new Random();
                defaultIcon2PlayerOne = r.Next(0, iconList.Count-5);//dejamos el ultimo para el pc
                defaultColor2PlayerOne = defaultIcon2PlayerOne % (colorsList.Count-1);                
                int m = r.Next(0, iconList.Count);
                while ( m== defaultIcon2PlayerOne)
                {
                    m=r.Next(0, iconList.Count);
                }
                defaultIcon2PlayerTwo = m;
                defaultColor2PlaterTwo = m % colorsList.Count;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Crea la lista de contrincantes machine
        /// </summary>
        /// <returns></returns>
        public List<Player> createPCOpponents()
        {
            try
            {
                List<Player> listOpponent =new List<Player>();
                int i = 0;
                foreach (BitmapImage bi in iconList.GetRange(7, 5))
                {
                    listOpponent.Add(new Player(machineName[i++], bi, colorsList[colorsList.Count - 1]));
                }
                return listOpponent;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                return null;
            }

        }

        public void showPlayersDetail()
        {
            foreach (Player p in playerList)
            {
                Console.WriteLine(p.Nombre + "\t" + p.ColorPieza.ToString());
            }
        }

        public String toString()
        {
            return "1 or 2 players? " + pcOption + "\n with time: " + isTimer + "\nWhat time? " + time + "\nX: " + xDim + "\tY: " + yDim;
        }
    }
}
