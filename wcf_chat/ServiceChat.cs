using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


namespace wcf_chat
{
  
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        List<ServerUser> users = new List<ServerUser>();
        List<String> messages = new List<String>();
        int numOfWords = 0;
        int nextId = 1;

        public int Connect(string name)
        { 
            ServerUser user = new ServerUser() {
                ID = nextId,
                Name = name,
                operationContext = OperationContext.Current
            };
            nextId++;
            SendMsg(" "+user.Name+ $" joined to service!", 0);
            users.Add(user);
           
            return user.ID;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.ID == id);
            if (user!=null)
            {
                users.Remove(user);
                SendMsg(" "+user.Name + $" left service!", 0);
            }
        }

        public void SendMsg(string msg, int id)
        {
            Console.WriteLine($"? {id}");
            int getNumOfWords(string inputLine)
            {
                string[] words;
                words = inputLine.Split(' ');
                return words.Length;
            }

            var msgType = "";
            if (id != 0) {
                messages.Add(msg);
                numOfWords = 0;
                foreach (var message in messages)
                {
                    numOfWords += getNumOfWords(message);
                    Console.WriteLine($"- {message}");
                }
                msgType = "[USER]";
            }
            else
            {
                msgType = "[SYSTEM]";
            }

            foreach (var item in users)
            {
                string answer = DateTime.Now.ToShortTimeString();

                var user = users.FirstOrDefault(i => i.ID == id);
                if (user != null)
                {
                    answer += ": " + user.Name + " ";
                }
                answer += $"\n    {msgType} : {msg} \n    [NUM] : {numOfWords}";
                item.operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer);
            }
        }
    }
}
