using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Net.Sockets;
using Client_CD4.Communication;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Client_CD4.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        Client clientVar;
        //Mit Server verbunden?
        public Boolean Connected = false;

        //Username und Nachricht
        public string ChatName { get; set; }
        public string Message { get; set; }

        //empfangene Nachrichten
        public ObservableCollection<string> MessagesReceived { get; set; }

        //Relay Commands
        public RelayCommand ConnectBtnClicked { get; set; }
        public RelayCommand SendBtnClicked { get; set; }
        

        public MainViewModel()
        {
            Message = "";
            MessagesReceived = new ObservableCollection<string>();
            ConnectBtnClicked = new RelayCommand(ShowConnectClicked, IsShowConnectClickEnabled);
            SendBtnClicked = new RelayCommand(ShowSendBtnClicked, IsShowSendBtnClickedEnabled);
        }

        //Send Button 
        private bool IsShowSendBtnClickedEnabled()
        {
            return (Connected && Message.Length >= 1);
        }

        private void ShowSendBtnClicked()
        {
            clientVar.Send(ChatName + ": " + Message);
            MessagesReceived.Add("You: " + Message);
        }

        //Connect Button
        private bool IsShowConnectClickEnabled()
        {
            return !Connected;
        }

        private void ShowConnectClicked()
        {
            if(ChatName != null && !ChatName.Equals(""))
            {
                //Wenn kein Name eingegeben ist, kann er sich nicht verbinden
                Connected = true;
                clientVar = new Client("127.0.0.1", 10100, new Action<string>(NewMessageReceived), ClientDisconnected);
                
            }
        }

        //Wenn Verbindung von Client zu Ende
        private void ClientDisconnected()
        {
            Connected = false;

            CommandManager.InvalidateRequerySuggested();
        }

        //Erhalt einer neuen Nachricht
        private void NewMessageReceived(string message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                MessagesReceived.Add(message);
            });
        }

        //TEST COMMIT GITHUB
    } 
    
}