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

namespace MCB
{
    /// <summary>
    /// RegiNO.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RegiNO : Window
    {
        public RegiNO()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            NO.Close();
        }

    }
}
