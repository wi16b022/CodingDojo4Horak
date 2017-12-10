
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server_CD4.Communication
{
    public class ClientHandler
    {

        Action<string, Socket> action;

        //Buffer
        byte[] buffer = new byte[1024];

        //Thread
        private Thread clientReceiveThread;

        //Nachrichtenende definieren, Abbruchbedingung
        string messageEnd = "@quit";

        public Socket ClientSocket { get; private set; }
        public string Name { get; private set; }

        public ClientHandler(Socket socket, Action<string, Socket> action) 
        {         
            this.ClientSocket = socket;
            this.action = action;
            clientReceiveThread = new Thread(Receive);
            clientReceiveThread.Start();
        }

        //Solange nicht disconnected wurde, wird empfangen
        private void Receive()
        {
            string message = "";

            while (!message.Equals(messageEnd))
            {
                int length = ClientSocket.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, length);
                if (Name == null && message.Contains(":"))
                {
                    Name = message.Split(':')[0];
                }
                action(message, ClientSocket);
            }

            Close();
        }

        public void Send(string message)
        {
            ClientSocket.Send(Encoding.UTF8.GetBytes(message));
        }

        //Verbindung schließen
        internal void Close()
        {
            Send(messageEnd);
            ClientSocket.Close(1);
            clientReceiveThread.Abort();
        }
    }
}
