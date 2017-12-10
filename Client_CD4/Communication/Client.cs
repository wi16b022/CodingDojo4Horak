using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client_CD4.Communication
{
    class Client
    {
        //Buffer anlegen
        byte[] buffer = new byte[1024];
        Socket clientsocket;


        Action<string> Informer;
        Action CancelInformer;

        public Client(string ip, int port, Action<string> informer, Action cancelInformer)
        {
            try
            { 
            this.Informer = informer;
            this.CancelInformer = cancelInformer;
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse(ip), port);
            clientsocket = client.Client;
            StartReceiving();
            }

            //Wenn Server nicht gestartet oder gestoppt -> Exception
            catch (Exception)
            {
                Informer("Server not ready!");
                CancelInformer();
            }
        }
        //Empfangen Start
        private void StartReceiving()
        {
            Task.Factory.StartNew(Receive);
        }

        //Empfangen
        private void Receive()
        {
            string message = "";
            while (!message.Equals("@quit"))
            {
                int length = clientsocket.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, length);
                Informer(message);
            }
            Close();
        }

        //Empfangen Ende
        private void Close()
        {
            clientsocket.Close();
            CancelInformer();
            
        }

        //Senden
        public void Send(string message)
        {
            if (clientsocket != null)
            {
                clientsocket.Send(Encoding.UTF8.GetBytes(message));
            }
        }
    }
}
