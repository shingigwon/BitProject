using MCB.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MCB.ServiceReference1;
using Python.Runtime;
using System.Windows.Interop;
using System.Runtime.DesignerServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Microsoft.Win32;
using System.Xml;
using System.IO;
using System.Threading;

namespace MCB
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 프로퍼티 및 변수
        // 출력 코드 선언
        List<string> include = new List<string>();
        List<string> main = new List<string>();
        private string define;

        // 서버 클래스 선언
        ChatClient client = new ChatClient();

        // 챗 바인딩 클래스 선언
        ChatData chatData = null;
        public string ID { get; private set; }
        #endregion


        public MainWindow(string userName, string id)
        {
            InitializeComponent();
            chatData = new ChatData(userName);
            ChattingBox.ItemsSource = chatData;     // 바인딩
            ID = id;
        }

        #region 이벤트 메서드
        private void Window_Closing(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        //Enter로 Send
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(tbUserMsg.Text))
                    return;

                string message = tbUserMsg.Text;
                dynamic[] text = client.Say(message);

                chatData.UserChat(message);

                ChatFillter(ID, message, text[0], text[1]);

                tbUserMsg.Text = "";

                ScrollDown();
            }
        }

        //전송버튼
        private void btnSendMsg_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbUserMsg.Text))
                return;

            string message = tbUserMsg.Text;
            dynamic[] text = client.Say(message);

            chatData.UserChat(message);

            ChatFillter(ID, message, text[0], text[1]);

            tbUserMsg.Text = "";
            ScrollDown();
            
        }
        #endregion

        #region 기능 함수 
        private void ScrollDown()
        {
            // 채팅창 아래 포커싱
            if (VisualTreeHelper.GetChildrenCount(ChattingBox) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(ChattingBox, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }

            // 코드창 아래 포커싱
            if (VisualTreeHelper.GetChildrenCount(CodeText) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(CodeText, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }
        private void ChatFillter(string id, string question, string answer, string code)
        {
            if (code == "." || code == null)
            {
                chatData.MicoBotChat(answer);
                return;
            }
            
            else if (code.Contains("☞") == true)
            {
                string codes = CodeSetting(code);
                CodeText.Text = codes;

                if (!SaveFile())
                    return;

                int count = ChattingBox.Items.Count;

                Message user = (Message)ChattingBox.Items[count - 3];
                Message bot = (Message)ChattingBox.Items[count - 2];

                client.Save(id, user.Content, bot.Content, CodeText.Text);

                chatData.MicoBotChat(answer);
            }
            else if (code == "☜")
            {

                string[] temp = client.Load(id);

                if (temp == null)
                {
                    chatData.MicoBotChat("저장된 코드가 없습니다...");
                    return;
                }

                chatData.MicoBotChat(answer);

                // 이전 대화 및 코드 불러오기
                chatData.UserChat(temp[0]);
                chatData.MicoBotChat(temp[1]);
                CodeText.Text = temp[2];
            }
            else
            {
                chatData.MicoBotChat(answer);

                string codes = CodeSetting(code);
                CodeText.Text = codes;
            }
        }

        // 파일로 저장
        private bool SaveFile()
        {
            bool result = true;
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "C 파일 (*.c)|*.c|Text 파일 (*.txt)|*.txt";
            dlg.Title = "C 파일로 저장";
            if (dlg.ShowDialog() == true)
                File.WriteAllText(dlg.FileName, CodeText.Text);

            RegiOK regiok = new RegiOK();

            if (dlg.FileName != "")
            {
                regiok.OK.Title = "파일 저장";
                regiok.TextB.Text = "저장 성공!";
            }

            else
            {
                regiok.OK.Title = "파일 저장";
                regiok.okicon.Source = new BitmapImage(new Uri("Resources/NoIcon.png", UriKind.Relative));
                regiok.Btn_OK.Background = new SolidColorBrush(Color.FromArgb(255, 233, 150, 122));
                regiok.TextB.Text = "저장 실패";
                result = false;
            }

            regiok.ShowDialog();

            return result;
        }

        private string CodeSetting(string code)
        {
            string[] words = { "" };
            int codeIdx = 0;

            if (include.Count == 0)
                include.Add("#include <stdio.h>\n");


            // define이 있는 경우
            if (code.Split('№').Length > 1)
            {
                words = code.Split('№');       // Define 나누기

                define = words[0];
                codeIdx++;
            }
            // include가 있는 경우
            else if (code.Split('™').Length > 1)
            {
                words = code.Split('™');       // include 나누기

                include.Add(words[0]);
                codeIdx++;
            }
            else if (code.Split('☞').Length > 1)
            {
                words = code.Split('☞');
                codeIdx++;
            }
            else
                words[codeIdx] = code;

            if (main.Count == 0)
                main.Add(words[codeIdx]);
            else
            {
                double value = FindSimilarity.findSimilarity(main[main.Count - 1], words[codeIdx]);
                // 값이 일부만 변경 (덮어쓰기 해야함)
                if (value > .6) main[main.Count - 1] = words[codeIdx];

                // 값이 완전 변경 (새롭게 저장)
                else main.Add(words[codeIdx]);
            }

            // 출력문 초기화
            string codes = string.Empty;

            // include문 저장
            foreach (string word in include)
                codes += word;

            codes += define;

            // 코드 저장
            foreach (string word in main)
                codes += word;

            return codes;

        }
        #endregion
    }

    #region 데이터 클래스
    public class ChatData : ObservableCollection<Message>
    {
        public ChatData(string name)
        {
            string temp = string.Format("{0}님! 무엇을 도와드릴까요?", name);
            this.Add(new Message() { Sender = "미코봇", Content = temp, FlowDir = "LeftToRight", Resource = @"Resources\mcb.png", Color = "#FFDFF1E6" });
        }

        public void MicoBotChat(string msg)
        {
            this.Add(new Message() { Sender = "미코봇", Content = msg, FlowDir = "LeftToRight", Resource = @"Resources\mcb.png", Color = "#FFDFF1E6" });
        }
        public void UserChat(string msg)
        {
            this.Add(new Message() { Sender = "나", Content = msg, FlowDir = "RightToLeft", Resource = @"Resources\user.png", Color = "#FFF3D8CE" });
        }
    }
    public class Message
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public string FlowDir { get; set; }
        public string Resource { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region 문자열 유사도 판별 클래스
    public class FindSimilarity
    {
        public static int getEditDistance(string X, string Y)
        {
            int m = X.Length;
            int n = Y.Length;

            int[][] T = new int[m + 1][];
            for (int i = 0; i < m + 1; ++i) { T[i] = new int[n + 1]; }
            for (int i = 1; i <= m; i++) { T[i][0] = i; }
            for (int j = 1; j <= n; j++) { T[0][j] = j; }

            int cost;
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    cost = X[i - 1] == Y[j - 1] ? 0 : 1;
                    T[i][j] = Math.Min(Math.Min(T[i - 1][j] + 1, T[i][j - 1] + 1),
                            T[i - 1][j - 1] + cost);
                }
            }

            return T[m][n];
        }

        public static double findSimilarity(string x, string y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentException("Strings must not be null");
            }

            double maxLength = Math.Max(x.Length, y.Length);
            if (maxLength > 0)
            {
                // 필요한 경우 선택적으로 대소문자를 무시합니다.
                return (maxLength - getEditDistance(x, y)) / maxLength;
            }
            return 1.0;
        }
    }
    #endregion
}