
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server_CD4.Communication
{
    public class Server
    {
        Socket serverSocket;
        List<ClientHandler> clients = new List<ClientHandler>();
        Action<string> GuiUpdater;

        Thread acceptingThread;

       

        public Server(string ip, int port, Action<string> GuiUpdater)
        {
            this.GuiUpdater = GuiUpdater;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            serverSocket.Listen(5);


        }

        public void AcceptStart()
        {
            acceptingThread = new Thread(new ThreadStart(AcceptClient));
            acceptingThread.IsBackground = true;
            acceptingThread.Start();
        }

        private void AcceptClient()
        {
            while (acceptingThread.IsAlive)
            {
                clients.Add(new ClientHandler(serverSocket.Accept(), new Action<string, Socket>(MessageReceived)));
            }
        }

        private void MessageReceived(string message, Socket socketSender)
        {
            GuiUpdater(message);

            foreach(var item in clients)
            {
                if (item.ClientSocket != socketSender)
                {
                    item.Send(message);
                }
            }
        }

        public void AcceptStop()
        {
            serverSocket.Close();
            acceptingThread.Abort();

            foreach (var item in clients)
            {
                item.Close();
            }
            clients.Clear();
        }

        //Verbindung zu bestimmten User abbrechen
        public void DisconnectUser(string name)
        {
            foreach(var item in clients)
            {
                if (item.Name.Equals(name))
                {
                    item.Close();
                    clients.Remove(item);
                    break;
                }
            }
        }
    }
}
