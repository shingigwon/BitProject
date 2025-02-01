using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MCBChat_Server
{
    [ServiceContract]
    internal interface IChat
    {
        [OperationContract]
        bool IDCheck(string id);

        [OperationContract]
        void SignUp(string id, string pw, string name);

        [OperationContract]
        string Login(string id, string pw);

        [OperationContract]
        dynamic[] Say(string msg);

        [OperationContract]
        void Save(string id, string question, string answer, string code);

        [OperationContract]
        string[] Load(string id);

    }
}
