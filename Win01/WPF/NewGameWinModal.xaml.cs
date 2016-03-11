using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Win01
{
    /// <summary>
    /// Lógica de interacción para NewGameWinModal.xaml
    /// </summary>
    public partial class NewGameWinModal : Window
    {
        #region
        public bool PcOponent { get; set ; }
        public bool TimeIsChecked { get; set;}
        public int X { get ; set ; }
        public int Y { get ;set ; }
        public int Time { get; set; }
        public int Objective { get ; set; }
        public int Level { get ; set; }
        public Player Machine { get; set; }

        public List<Player> pcOpponents { get; set; }
        #endregion
        public NewGameWinModal()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.textX.Text = X.ToString();
                this.textY.Text = Y.ToString();
                this.textObjective.Text = Objective.ToString();
                checkOponent(PcOponent);
                this.sliderTime.Value = Time;
                this.textBlockTime.Text = ((int)sliderTime.Value).ToString();                
                loadTime();
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        private void loadTime()
        {
            if (Level > 3)
            {
                TimeIsChecked = true;
                checkTime.IsEnabled = false;
                Time = 20/Level;
            }
        }
        /// <summary>
        /// Para saber que opcion de contrincante esta chequeada
        /// </summary>
        /// <param name="b"></param>
        private void checkOponent(bool b)
        {
            try
            {
                if (b)//maquina
                {
                    this.pcOption.IsChecked = true;
                }
                else
                {
                    this.twoPlayersOption.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Cuando cambia el valor del slider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                this.textBlockTime.Text = ((int)(e.NewValue)).ToString();
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Aceptar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //establecer los valores. Controlamos que no se pase poniendo filas y columnas
                X = Int32.Parse(this.textX.Text);
                if (X > 8)
                {
                    X = 8;
                }
                Y = Int32.Parse(this.textY.Text);
                if (Y > 10)
                {
                    Y = 10;
                }
                Time = (int)this.sliderTime.Value;
                TimeIsChecked = (bool)this.checkTime.IsChecked;
                PcOponent = whatOponent();
                DialogResult = true;
                //si el tiempo es cero, es como si no se chequeara
                if (Time == 0) { TimeIsChecked = false; }
                int min = Math.Min(X, Y);
                Objective = Int32.Parse(this.textObjective.Text);
                if (Objective > min)
                {
                    Objective = min;
                }
                loadTime();
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Saber que opcion de contrincante esta seleccionada
        /// </summary>
        /// <returns></returns>
        private bool whatOponent()
        {
            try
            {
                return (bool)this.pcOption.IsChecked;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
                return false;
            }
        }
        /// <summary>
        /// Cancelar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
        /// <summary>
        /// Asi solo aceptamos nuemros en los textBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                TextBox obj = (TextBox)sender;
                if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }

            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        /// <summary>
        /// Chequear el tiempo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)checkTime.IsChecked)
                {
                    sliderTime.IsEnabled = true;
                    textBlockTime.IsEnabled = true;
                }
                else
                {
                    sliderTime.IsEnabled = false;
                    textBlockTime.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }

        private void pcOption_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            chooseOpponent.IsEnabled = (bool)rb.IsChecked;
        }

        private void twoPlayersOption_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            chooseOpponent.IsEnabled = !(bool)rb.IsChecked;
        }

        private void chooseOpponent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage img = new BitmapImage();
                List<BitmapImage> iconMachine = new List<BitmapImage>();
                foreach (Player p in pcOpponents)
                {
                    iconMachine.Add(p.Foto);
                }
                AvatarWin a = new AvatarWin();
                a.Owner = this;
                a.iconList = iconMachine;
                a.ShowDialog();
                if (a.DialogResult == true)
                {
                    img = a.iconChoosed;
                }
                searchOpponent(img);
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
        private void searchOpponent(BitmapImage img)
        {
            int i = 0;
            while (i < pcOpponents.Count && !pcOpponents[i].Foto.Equals(img))
            {
                i++;
            }
            Level = i+1;
            Machine= pcOpponents[i];
        }
    }
}
