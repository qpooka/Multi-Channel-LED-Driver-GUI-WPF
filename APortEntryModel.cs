using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.INPC;

namespace MVVM.model
{
    class APortEntryModel : BaseINPC
    {
        private string _portentry;
        public string PortEntry
        {
            get { return _portentry; }
            set
            {
                _portentry = value;
                RaisePropertyChangedEvent("PortEntry");
            }
        }


    }
}
