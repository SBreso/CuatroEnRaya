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
        public List<Player> playerList;
        public List<SolidColorBrush> colorsList;
        public List<BitmapImage> iconList;
        public Configuration(List<BitmapImage> il, List<SolidColorBrush> cl)
        {
            iconList = il;
            colorsList = cl;
            resetConfi();
        }

        public void resetConfi()
        {            
            pcOption = false;
            xDim = 10;
            yDim = 10;
            time = 10;
            playerList = new List<Player>();
            loadDefaultIconAndColor();
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
                defaultIcon2PlayerOne = r.Next(0, iconList.Count-2);//dejamos el ultimo para el pc
                defaultColor2PlayerOne = defaultIcon2PlayerOne % (colorsList.Count-1);                
                int m = r.Next(0, iconList.Count);
                while ( m== defaultIcon2PlayerTwo)
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
        public Player createPC()
        {
            try
            {
                Player pc = new Player();
                pc.Nombre = "Maquina";
                pc.Foto = iconList[iconList.Count-1];
                pc.ColorPieza = colorsList[colorsList.Count-1];                
                if (pcOption) { playerList.Add(pc); }
                return pc;
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
