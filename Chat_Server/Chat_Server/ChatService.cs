using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;

namespace MCBChat_Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    internal class ChatService : IChat
    {

        public SqlConnection Scon { get; private set; }

        #region DB연결 및 해제, 쿼리문실행

        public void DBOpen()
        {                   //@"Data Source=[서버 이름];Initial Catalog=[DB 명]; User ID=[ID];Password=[PW]";
            string constr = @"Data Source=DESKTOP-0I86BTV\SQLEXPRESS;Initial Catalog=MCM; User ID=mcm; Password=5340";
            Scon = new SqlConnection(constr);
            Scon.Open(); //연결 열기
        }

        public void DBClose()
        {
            Scon.Close(); //연결 닫기
        }

        //쿼리문실행
        private void ExecuteNonQuery(string sql)
        {
            using (SqlCommand scom = new SqlCommand(sql, Scon))
            {
                int ret = scom.ExecuteNonQuery();  //insert, update, delete     //단방향
                if (ret != 1)
                    throw new Exception("쿼리문 실패");
            }
        }

        #endregion

        #region 중복체크
        //select id from Member where ID = 1;
        public bool IDCheck(string id)
        {
            DBOpen();
            
            string sql = string.Format("select ID from Member where ID = '{0}';", id);

            SqlCommand scom = new SqlCommand(sql, Scon);
            SqlDataReader reader = scom.ExecuteReader();
            while (reader.Read())
            {

                if (id == reader[0].ToString())
                {
                    return true;
                }
            }
            reader.Close();
            DBClose();

            return false;
        }
        #endregion


        #region 회원가입
        //insert into member values('1', '1', '1');
        public void SignUp(string id, string pw, string name)
        {
            DBOpen();
            string sql = string.Format("insert into Member values('{0}','{1}','{2}');", id, pw, name);
            ExecuteNonQuery(sql);
            DBClose();
        }
        #endregion


        #region 로그인하기
        //select id, pw, name from Member where ID = '1' and pw = '1';
        public string Login(string id, string pw)
        {
            DBOpen();
            string sql = string.Format("select id, pw, name from Member where ID = '{0}' and pw = '{1}';", id, pw);

            SqlCommand scom = new SqlCommand(sql, Scon);
            SqlDataReader reader = scom.ExecuteReader();

            while (reader.Read())
            {
                
                if (id == reader[0].ToString() && pw == reader[1].ToString())
                {
                    return reader[2].ToString();
                }
            }
            reader.Close();
            DBClose();

            return null;
        }
        #endregion


        #region 메시지 보내기
        public dynamic[] Say(string msg)
        {
            // python path 위치(python.exe 파일 위치)
            string szPythonPath = "C:\\Program Files\\Python39";

            Environment.SetEnvironmentVariable("PYTHONHOME", szPythonPath);
            Environment.SetEnvironmentVariable("PYTHONPATH", $"{szPythonPath}\\Lib;" +
                                                //python 실행 파일 위치
                                                "D:\\mcmproject\\Micobot");

            // python 엔진 초기화
            PythonEngine.Initialize();

            using (Py.GIL())
            {
                dynamic Model = Py.Import("model");         // import할 py 파일명
                dynamic[] text = Model.return_answer(msg);  // 함수명(인자)

                if(text == null)
                    return null;

                else
                    return text;
            }
            
        }
        #endregion

        #region 코드 저장
        public void Save(string id, string question, string answer, string code)
        {
            DBOpen();

            //CREATE PROCEDURE SAVECODE(@ID varchar(50), @QUESTION varchar(MAX), @ANSWER varchar(MAX), @CODE varchar(MAX))
            #region 프로시저
            SqlCommand command = new SqlCommand("SAVECODE", Scon);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter id_param = new SqlParameter("@ID", id);
            command.Parameters.Add(id_param);

            SqlParameter question_param = new SqlParameter("@QUESTION", question);
            command.Parameters.Add(question_param);

            SqlParameter answer_param = new SqlParameter("@ANSWER", answer);
            command.Parameters.Add(answer_param);

            SqlParameter code_param = new SqlParameter("@CODE", code);
            command.Parameters.Add(code_param);

            command.ExecuteNonQuery();
            #endregion

            DBClose();
        }
        #endregion

        #region 코드 불러오기
        //Select Q, A, Code from Member_info where id = '1';
        public string[] Load(string id)
        {
            DBOpen();
            string sql = string.Format("Select Q, A, Code from Member_info where id = '{0}';", id);

            SqlCommand scom = new SqlCommand(sql, Scon);
            SqlDataReader reader = scom.ExecuteReader();


            while (reader.Read())
            {
                if (reader.IsDBNull(0))
                    return null;

                else
                {
                    string[] temp = { reader[0].ToString(), reader[1].ToString(), reader[2].ToString() };
                    return temp;
                }

            }
            reader.Close();
            DBClose();

            return null;
        }
        #endregion
    }
}