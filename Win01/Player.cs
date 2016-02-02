using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.ComponentModel;

namespace Win01
{
    public class Player : INotifyPropertyChanged
    {
        private BitmapImage foto;
        private int tiempo;
        private int tiempoTotal;
        private float ganadas;
        private SolidColorBrush colorPieza;

        /// <summary>
        /// Devuelve o asigna el nombre del jugador
        /// </summary>
        public String Nombre { get; set; }

        /// <summary>
        /// Devuele o asigna la imagen del jugador
        /// </summary>
        public BitmapImage Foto
        {
            get { return foto; }
            set { foto = value; }
        }
        /// <summary>
        /// Asigna la URI del la foto
        /// </summary>
        public String URIFoto
        {
            set
            {
                foto = new BitmapImage(new Uri(value));
            }
        }
        /// <summary>
        /// Devuelve o asigna el número de partidas ganadas por el jugador
        /// </summary>
        public float Ganadas
        {
            get { return ganadas; }
            set
            {
                ganadas = value;
                RaisePropertyChanged("Ganadas");
            }
        }

        /// <summary>
        /// Devuelve o asigna el color de la pieza del jugador
        /// </summary>
        public SolidColorBrush ColorPieza
        {
            get { return colorPieza; }
            set
            {
                colorPieza = value;
                RaisePropertyChanged("ColorPieza");
            }
        }

        /// <summary>
        /// Devuelve o asigna el tiempo total acumulado
        /// </summary>
        public int TiempoAcumuladoTotal
        {
            get { return tiempoTotal; }
            set
            {
                tiempoTotal += value;
                RaisePropertyChanged("TiempoAcumuladoTotal");
            }
        }

        /// <summary>
        /// Devuelve o asigna el tiempo acumulado
        /// </summary>
        public int TiempoAcumulado
        {
            get { return tiempo; }
            set
            {
                tiempo += value;
                RaisePropertyChanged("TiempoAcumulado");
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nom">nombre del jugador</param>
        /// <param name="URI">URI de la imagen</param>
        /// <param name="color">color de la pieza</param>
        public Player(String nom, String URI, SolidColorBrush color)
        {
            Nombre = nom;
            foto = null;

            ColorPieza = color;

            tiempo = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nom">Nombre del jugador</param>
        /// <param name="URI">Imagen del jugador</param>
        /// <param name="color">color de la pieza</param>
        public Player(String nom, BitmapImage bmp, SolidColorBrush color)
        {
            Nombre = nom;
            foto = bmp;

            ColorPieza = color;

            tiempo = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="URI"></param>
        public Player(String nom, String URI)
            : this(nom, URI, new SolidColorBrush(Colors.Black))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nom">Nombre del jugador</param>
        /// <param name="URI">Imagen del jugador</param>
        /// <param name="color">color de la pieza</param>
        public Player(String nom, SolidColorBrush color)
            : this(nom, "", color)
        {
        }

        /// <summary>
        /// Constructor solo color
        /// </summary>
        /// <param name="col">color de las piezas</param>
        public Player(SolidColorBrush col)
            : this("", "", col)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Player()
        {
        }

        /// <summary>
        /// Pone a cero el tiempo 
        /// </summary>
        public void ResetTiempo() { tiempo = 0; }

        /// <summary>
        /// Pone a cero el tiempo total (y el parcial)
        /// </summary>
        public void ResetTiempoTotal()
        {
            tiempoTotal = 0;
            tiempo = 0;
        }

        #region Implementación INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
