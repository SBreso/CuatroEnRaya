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
        public NewGameWinModal()
        {
            InitializeComponent();
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
                Debug.Print("{0}--{1}", ex.Message, ex.GetType());
            }
        }

        private void checkTime_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.textBlockTime.IsEnabled = false;
                this.sliderTime.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Debug.Print("{0}--{1}", ex.Message,ex.GetType());
            }
            
        }

        private void checkTime_Checked(object sender, RoutedEventArgs e)
        {
            try {
                this.textBlockTime.IsEnabled = true;
                this.sliderTime.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Debug.Print("{0}--{1}", ex.Message, ex.GetType());
            }
        }
    }
}
