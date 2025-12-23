using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_ChatClient.MVVM.M;
using WPF_ChatClient.MVVM.VM;

namespace WPF_ChatClient
{
    /// <summary>
    /// ChatRoom.xaml 的交互逻辑
    /// </summary>
    public partial class ChatRoom : Window
    {


        public ChatRoom()
        {
            InitializeComponent();
            this.DataContext = new ChatRoomVM(this);
        }


        #region 界面按钮事件
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.IsActive)
                {
                    window.WindowState = WindowState.Minimized;
                }
            }
        }

        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {

            foreach (Window window in Application.Current.Windows)
            {
                if (window.IsActive)
                {
                    if (window.WindowState != WindowState.Maximized)
                    {
                        window.WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        window.WindowState = WindowState.Normal;
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion
    }
}
