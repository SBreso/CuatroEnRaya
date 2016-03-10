using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para AvatarWin.xaml
    /// </summary>
    public partial class AvatarWin : Window
    {
        public List<BitmapImage> iconList { get; set; }
        public BitmapImage iconChoosed { get; set; }
        public AvatarWin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double prop = 0;
            foreach (BitmapImage bi in iconList)
            {
                prop = bi.Width/ bi.Height;
                Image img = new Image();
                img.MouseLeftButtonUp += new MouseButtonEventHandler(img_Click);
                img.Height = 75;
                img.Width = 75*prop;
                img.Source = bi;
                img.Margin = new Thickness(2,2,2,2);
                wrap.Children.Add(img);
            }
            this.Width = iconList.Count * (75 * prop+6);
            this.Height = 75 +4;
        }
        private void img_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Image img = (Image)sender;
                iconChoosed = (BitmapImage)img.Source;

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Debugger.WriteException(ex, this);
            }
        }
    }
}
