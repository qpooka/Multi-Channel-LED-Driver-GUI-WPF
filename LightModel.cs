using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.INPC;

namespace MVVM.model
{
    class LightModel : BaseINPC
    {
        private string _light_text;
        public string Light_text
        {
            get { return _light_text; }
            set
            {
                if (_light_text != value)
                {
                    _light_text = value;
                    RaisePropertyChangedEvent("Light_text");
                }
            }
        }
        private int _light_val;
        public int Light_val
        {
            get { return _light_val; }
            set
            {
                _light_val = value;
                RaisePropertyChangedEvent("Light_val");
            }
        }
    }
}
