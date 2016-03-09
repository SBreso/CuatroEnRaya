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
    }
}
