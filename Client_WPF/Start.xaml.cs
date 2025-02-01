using MCB.ServiceReference1;
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
    /// Start.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Start : Window
    {
        ChatClient client = new ChatClient();
        public Start()
        {
            InitializeComponent();
        }

        //로그인
        private void Start_Btn(object sender, RoutedEventArgs e)
        {
            string temp = client.Login(txt_id.Text, txt_pw.Password);

            if(temp != null)
            {                
                this.Hide();
                MainWindow mainWindow = new MainWindow(temp, txt_id.Text);


                bool? b = mainWindow.ShowDialog();
                if (b == true) this.Show();
            }
            else
            {
                //
                RegiNO regino = new RegiNO();

                regino.NO.Title = "로그인 실패";
                regino.TextB.Text = "로그인 실패";
                bool? b = regino.ShowDialog();

                if (b == true) this.Show();
            }



        }

        //회원가입
        private void Register_Btn(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Register register = new Register();
            bool? b = register.ShowDialog();
            if (b == true) this.Show();
        
        }
    }
}
