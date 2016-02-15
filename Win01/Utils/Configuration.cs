using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Win01
{
    public class Configuration
    {
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
            pcOption = true;
            xDim = 4;
            yDim = 4;
            time = 10;
            playerList = new List<Player>();
        }

        public void DefaultGamers()
        {
            try
            {
                Player p1 = new Player();
                p1.Nombre = "Jugador 1";
                p1.Foto = iconList[0];
                p1.ColorPieza = colorsList[0];
                playerList.Add(p1);
                if (pcOption)//vs maquina
                {

                    
                }
                else//vs jugador
                {
                    Player p2 = new Player();
                    p2.Nombre = "Jugador 2";
                    p2.Foto = iconList[1];
                    p2.ColorPieza = colorsList[1];
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

        public Player createPC()
        {
            try
            {
                Player pc = new Player();
                pc.Nombre = "Maquina";
                pc.Foto = iconList[11];
                pc.ColorPieza = colorsList[5];                
                if (pcOption) { playerList.Add(pc); }
                return pc;
            }
            catch (Exception ex) 
            {
                Debugger.WriteException(ex, this);
                return null;
            }
        }

        public String toString()
        {
            return "1 or 2 players? " + pcOption + "\n with time: " + isTimer + "\nWhat time? " + time + "\nX: " + xDim + "\tY: " + yDim;
        }
    }
}
