using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Server_CD4.Communication;
using System.Collections.ObjectModel;


using System.Linq;

namespace Server_CD4.ViewModel
{
       public class MainViewModel : ViewModelBase
    {

        Server server; //Server vorbereiten, Serverklasse importieren

        //Port und IP-Adresse
        private int port = 10100;
        private string ip = "127.0.0.1";

        //Verbindung hergestellt?
        private bool isConnected = false;

        private DataHandler dataHandler;

        //Observable Collections
        public ObservableCollection<string> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public ObservableCollection<string> LogMessages { get; private set; }
        public ObservableCollection<string> LogFiles
        {
            get
            {
                return new ObservableCollection<string>(dataHandler.QueryFilesFromFolder());
            }
        }

        //Relay Commands
        public RelayCommand StartBtnClicked { get; set; }
        public RelayCommand StopBtnClicked { get; set; }
        public RelayCommand DropUserBtnClicked { get; set; }
        public RelayCommand SaveLogFileBtnClicked { get; set; }
        public RelayCommand OpenLogFileBtnClicked { get; set; }
        public RelayCommand DropLogFileBtnClicked { get; set; }


        public int NomberOfMessagesReceived { get { return Messages.Count; } }
        public string SelectedUser { get; set; }
        public string SelectedLogFile { get; set; }

        public MainViewModel()
        {

            dataHandler = new DataHandler();
            LogMessages = new ObservableCollection<string>();

            Messages = new ObservableCollection<string>();
            Users = new ObservableCollection<string>();

            StartBtnClicked = new RelayCommand(ShowStartBtnClicked, IsShowStartBtnClickedEnabled);
            StopBtnClicked = new RelayCommand(ShowStopBtnClicked, IsShowStopBtnClickedEnabled);
            DropUserBtnClicked = new RelayCommand(ShowDropUserBtnClicked, IsShowDropUserBtnClickedEnabled);
            SaveLogFileBtnClicked = new RelayCommand(ShowSaveLogFileBtnClicked, IsShowSaveLogFileBtnClickedEnabled);
            DropLogFileBtnClicked = new RelayCommand(ShowDropLogFileBtnClicked, IsShowDropLogFileBtnClickedEnabled);
            OpenLogFileBtnClicked = new RelayCommand(ShowOpenLogFileBtnClicked, IsShowOpenLogFileBtnClickedEnabled);
        }

        //Open Log File Button
        private bool IsShowOpenLogFileBtnClickedEnabled()
        {
            return SelectedLogFile != null;
        }

        private void ShowOpenLogFileBtnClicked()
        {
            LogMessages = new ObservableCollection<string>(dataHandler.Load(SelectedLogFile));
            RaisePropertyChanged("LogMessages");
        }

        //Drop Log File Button
        private bool IsShowDropLogFileBtnClickedEnabled()
        {
            return SelectedLogFile != null;
        }

        private void ShowDropLogFileBtnClicked()
        {
            dataHandler.Delete(SelectedLogFile);
            RaisePropertyChanged("LogFiles");
        }

        //Save Log File Button
        private bool IsShowSaveLogFileBtnClickedEnabled()
        {
            return Messages.Count >= 1;
        }

        private void ShowSaveLogFileBtnClicked()
        {
            dataHandler.Save(Messages.ToList());
            RaisePropertyChanged("LogFiles");
        }

        //Drop User Button
        private bool IsShowDropUserBtnClickedEnabled()
        {
            return SelectedUser != null;
        }

        private void ShowDropUserBtnClicked()
        {
            
            server.DisconnectUser(SelectedUser);
            Users.Remove(SelectedUser);
           
        }

        //Stop Button
        private bool IsShowStopBtnClickedEnabled()
        {
            return isConnected;
        }

        private void ShowStopBtnClicked()
        {
            server.AcceptStop();
            isConnected = false;
        }

        //Start Button
        private bool IsShowStartBtnClickedEnabled()
        {
            return !isConnected;
        }

        private void ShowStartBtnClicked()
        {
            server = new Server(ip, port, UpdateGuiWithNewMessage);
            server.AcceptStart();
            isConnected = true;

        }

        //Gui Update
        private void UpdateGuiWithNewMessage(string message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                string name = message.Split(':')[0];
                if (!Users.Contains(name))
                {
                    Users.Add(name);
                }
                Messages.Add(message);
                RaisePropertyChanged("NomberOfMessagesReceived");
            });
            }
    }
}