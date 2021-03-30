using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using MVVM.INPC;

namespace MVVM.model
{
    class OtherModel : BaseINPC
    {
        private string _portStatus;
        public string PortStatus
        {
            get { return _portStatus; }
            set
            {
                _portStatus = value;
                RaisePropertyChangedEvent("PortStatus");
            }
        }

        //not really sure if you need this here, but it doesn't matter I guess
        //this isn't binded to anything
        private string _mode;
        public string Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        //again, do I need this here? or would in ViewModel be better? Which would follow MVVM?
        private SerialPort _currPort;
        public SerialPort CurrPort
        {
            get { return _currPort; }
            set { _currPort = value; }
        }

        //should the DispatchTier even be here?
        //Kinda struggling to see the point, but not that it mattters if it is in the viewmodel either
        //feel like this is being too arbitrary
        //if there is an MVVM format for timer, that would be neat
        //private Timer _auto_Mode_Timer;
        public Timer Auto_Mode_Timer
        {
            get; 
            set;
        }

        //again, does this matter ->can someone look this up?
        //private Dispatcher _dispatcher_Timer;
        public Dispatcher Dispatcher_Timer
        {
            get;
            set;
        }
    }
}
