using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using WPF_ChatRoom;

namespace WPF_ChatClient
{
    class LoginVM : INotifyPropertyChanged
    {
        private MainWindow _mainWindow;
        public LoginVM(MainWindow mainWindow) { _mainWindow = mainWindow; }

        #region 通知到界面的固定代码
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)//通知到界面
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


        private LoginM _loginM;
        public LoginM LoginM
        {
            get
            {
                if (_loginM == null)
                { _loginM = new LoginM(); }
                return _loginM;
            }
            set
            {
                _loginM = value;
                RaisePropertyChanged("LoginM");
            }
        }


        #region 登录方法，包装成ICommand，以绑定到v中
        private void Loginfunc()
        {
            if (LoginM.Password != "")
            {
                string serverIp = "127.0.0.1";
                int serverPort = 8080;
                TcpClient client = new TcpClient();
                client.Connect(serverIp, serverPort);

                //ChatRoom chatRoom = new ChatRoom(LoginM.Name, LoginM.Password, client);
                //chatRoom.Show();
                MessageBox.Show("11");
            }
            else
            {
                MessageBox.Show("账户名不能为空");
            }

        }

        bool CanLoginExecute()
        {
            return true;
        }

        public ICommand LoginAction
        {
            get
            {
                return new RelayCommand(Loginfunc, CanLoginExecute);
            }
        }
        #endregion
    }
}
