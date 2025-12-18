using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ChatClient
{
    class ChatRoomM
    {
        private string _Write;

        public string Write
        {
            get { return _Write; }
            set { _Write = value; }
        }

        private string _show;

        public string show
        {
            get { return _show; }
            set { _show = value; }
        }
    }
}
