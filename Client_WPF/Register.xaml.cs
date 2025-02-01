using MCB.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Register.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Register : Window
    {
        ChatClient client = new ChatClient();
        public Register()
        {
            InitializeComponent();
        }
        private void Window_Closing(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        //가입버튼
        private void btn_register_Click(object sender, EventArgs e)
        {
            RegiOK regiok = new RegiOK();
            RegiNO regino = new RegiNO();


            if (text_id.Text == string.Empty)
            {
                regino.NO.Title = "가입 실패";
                regino.TextB.Text = "아이디를 입력해주세요.";
                regino.ShowDialog();

            }
            else if (text_pw.Password == string.Empty || text_Checkpw.Password == string.Empty)
            {
                regino.NO.Title = "가입 실패";
                regino.TextB.Text = "비밀번호를 입력해주세요.";
                regino.ShowDialog();

            }

            else if (text_name.Text == string.Empty)
            {
                regino.NO.Title = "가입 실패";
                regino.TextB.Text = "이름을 입력해주세요";
                regino.ShowDialog();

            }

            else if (text_id.IsEnabled == true)
            {
                regino.NO.Title = "가입 실패";
                regino.TextB.Text = "아이디 중복체크를 확인해주세요.";
                regino.ShowDialog();

            }

            else if (text_pw.Password != text_Checkpw.Password)
            {
                regino.NO.Title = "가입 실패";
                regino.TextB.Text = "비밀번호가 다릅니다.";
                regino.ShowDialog();
            }

            else
            {
                client.SignUp(text_id.Text, text_Checkpw.Password, text_name.Text);
                regiok.ShowDialog();
                this.Close();
            }
        }

        //취소버튼
        private void btn_register_No_Click(object sender, RoutedEventArgs e)
        {
            RegiNO regino = new RegiNO();
            regino.ShowDialog();

            if (regino.DialogResult == true)
            {
                this.Close();
            }
        }

        //중복확인버튼
        private void Btn_IdCheck_Click_1(object sender, RoutedEventArgs e)
        {
            RegiNO regino = new RegiNO();
            RegiOK regiok = new RegiOK();

            if (client.IDCheck(text_id.Text) == false)
            {

                regiok.OK.Title = "사용 가능한 아이디";
                regiok.TextB.Text = "사용 가능한 아이디입니다.";
                text_id.IsEnabled = false;
                regiok.ShowDialog();
            }
            else
            {
                regino.TextB.Text = "이미 등록된 아이디 입니다.";
                regino.ShowDialog();
            }
            
        }
    }
}
