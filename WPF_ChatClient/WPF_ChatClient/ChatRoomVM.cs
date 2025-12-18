using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_ChatRoom;

namespace WPF_ChatClient
{
    class ChatRoomVM : INotifyPropertyChanged
    {
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

        private MessageManager messageManager = MessageManager.Instance;

        ChatRoomM _chatRoomM;
        public ChatRoomM ChatRoomM
        {
            get
            {
                if (_chatRoomM == null) { _chatRoomM = new ChatRoomM(); }
                return _chatRoomM;
            }
            set
            {
                _chatRoomM = value;
                RaisePropertyChanged("ChatRoomM");
            }
        }

        private ChatRoom _chatRoom;

        public ChatRoomVM(ChatRoom chatRoom)
        {
            _chatRoom = chatRoom;
            messageManager.OnMessageReceived += OnMessageReceived;
        }

        #region 发送按键
        private void RoomSendMes()//发送信息
        {
            if (ChatRoomM.Write != "")
            {
                messageManager.SendMsg(ChatRoomM.Write, 0);
                ShowOnScroll(ChatRoomM.Write);

                ChatRoomM.Write = "";
                ChatRoomM = ChatRoomM;
            }
            else
            {
                MessageBox.Show("信息不能为空");
            }
        }

        bool CanSendExecute()
        {
            return true;
        }

        public ICommand RoomSendMesAction
        {
            get
            {
                return new RelayCommand(RoomSendMes, CanSendExecute);
            }
        }

        //将修改通知到UI线程(非UI线程修改UI会报错)
        private void ShowOnScroll(string message)
        {

            if (_chatRoom.TextPanel1.Dispatcher.CheckAccess())// 如果当前线程是 UI 线程，直接更新
            {
                var newText = new TextBlock
                {
                    Text = message,
                    Margin = new Thickness(5)
                };

                _chatRoom.TextPanel1.Children.Add(newText);
                _chatRoom.ScrollViewer1.ScrollToEnd();
            }
            else // 如果当前线程不是 UI 线程，通过 Dispatcher 调度到 UI 线程
            {

                _chatRoom.TextPanel1.Dispatcher.Invoke(new Action(() =>
                {
                    var newText = new TextBlock
                    {
                        Text = message,
                        Margin = new Thickness(5)
                    };
                    _chatRoom.TextPanel1.Children.Add(newText);
                }));
            }


        }

        #endregion

        public void OnMessageReceived(string mes)
        {
            ShowOnScroll(mes);
        }

    }
}
