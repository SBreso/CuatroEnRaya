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
        public bool Oponent { get{return oponent;} set{oponent=value;} }
        public bool TimeIsChecked { get{return timeIsChecked;} set{timeIsChecked=value;}}
        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }
        public int Time { get{return time;} set{time=value;} }
        private bool oponent;//true-->maquina & false-->2 players
        private bool timeIsChecked;
        private int x;
        private int y;
        private int time;
        #endregion
        public NewGameWinModal()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.textX.Text = x.ToString();
                this.textY.Text = y.ToString();
                checkOponent(this.oponent);
                this.sliderTime.Value = time;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
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
                Oponent = whatOponent();
                DialogResult = true;
                //si el tiempo es cero, es como si no se chequeara
                if (Time == 0) { TimeIsChecked = false; }
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
    }
}
