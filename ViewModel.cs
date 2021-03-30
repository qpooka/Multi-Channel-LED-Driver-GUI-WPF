using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using MVVM.INPC;
using MVVM.model;

namespace MVVM.viewmodel
{
    class ViewModel : BaseINPC
    {
        const string mode_ManOn = "x"; //really just here to improve readability
        const string mode_ManOff = "y";
        const string JustConnected = "~";
        const string GetAutoLight = "z";
        public ObservableCollection<LightModel> Lights { get; set; }
        public ObservableCollection<APortEntryModel> Ports { get; set; }
        //public ObservableCollection<SavedLightPresetModel> Presets{ get; set;}
        public DatabaseHandler PresetLoc = new DatabaseHandler("PresetData");
        public ObservableCollection<SavedLightPresetModel> Local_Presets { get; set; }
        private SavedLightPresetModel _sel_Preset;
        public SavedLightPresetModel Sel_Preset
        {
            get { return _sel_Preset; }
            set 
            {
                _sel_Preset = value;
                RaisePropertyChangedEvent("Sel_Preset");
            }
        }

        private APortEntryModel _sel_Port;
        public APortEntryModel Sel_Port 
        {
            get { return _sel_Port; }
            set 
            { 
                _sel_Port = value;
                RaisePropertyChangedEvent("Sel_Port");
            } 
        } //a regular APortEntryModel Sel_Port didn't work because the RaisePropertyChangedEvent wasn't alerting the object itself changed, 

        //public SerialPort CurrSPort = null;
        byte[] auto_light_bytes = { 0, 0, 0, 0 }; //put in a view or viewmodel?
        public OtherModel other_stuff { get; set; }
        public DelegateCommand Manual_On_Command { get; set; }
        public DelegateCommand Manual_Off_Command { get; set; }
        public DelegateCommand Drop_Down_Opened_Command { get; set; }
        public DelegateCommand Drop_Down_Closed_Command { get; set; }
        public DelegateCommand Move_Slider_Command { get; set; } 
        public DelegateCommand Close_Main_Window_Command { get; set; }
        public DelegateCommand Add_Preset_Command { get; set; }
        public DelegateCommand Set_Preset_Command { get; set; }
        public DelegateCommand Remove_Preset_Command { get; set; }

        public ViewModel()
        {
            Init();
            Manual_Off_Command = new DelegateCommand(ManualOff, Auto_Sel);
            Manual_On_Command = new DelegateCommand(ManualOn, Manual_Sel);
            Drop_Down_Opened_Command = new DelegateCommand(UpdatePortsList);
            Drop_Down_Closed_Command = new DelegateCommand(ConnectPort);
            Move_Slider_Command = new DelegateCommand(LightChange, Auto_Sel);
            Close_Main_Window_Command = new DelegateCommand(CloseOff);
            Add_Preset_Command = new DelegateCommand(AddPreset, Auto_Sel);
            Set_Preset_Command = new DelegateCommand(SetPreset, Auto_Sel);
            Remove_Preset_Command = new DelegateCommand(RemovePreset);
        }

        private void Init()
        {
            ObservableCollection<LightModel> lights = new ObservableCollection<LightModel>();
            ObservableCollection<APortEntryModel> ports = new ObservableCollection<APortEntryModel>();
            ObservableCollection<SavedLightPresetModel> local_presets = new ObservableCollection<SavedLightPresetModel>();

            OtherModel init_other = new OtherModel
            {
                PortStatus = "PLACEHOLDER", //placeholder
                Mode = "",  //placeholder ->init based on data recieved from Arduino
                CurrPort = null,
                //Auto_Mode_Timer = new Timer(Auto_Mode_Timer_Tick, null, 2000, 300);
            };
            init_other.Auto_Mode_Timer = new Timer(Auto_Mode_Timer_Tick, null,Timeout.Infinite, 250);
            init_other.Dispatcher_Timer = Dispatcher.CurrentDispatcher;
            

            if (SerialPort.GetPortNames().Length > 0)
                init_other.PortStatus = "Status: Port(s) available";
            else
                init_other.PortStatus = "Status: No ports available";

            //fucking hell, this small assumption made me spend a whole day debugging
            //if you add same LightModel object to lights and point Lights to it, it is all the exact same forever
            LightModel InitLight0 = new LightModel { Light_text = "Luminosity Percentage: ", Light_val = 0 };
            LightModel InitLight1 = new LightModel { Light_text = "Luminosity Percentage: ", Light_val = 0 };
            LightModel InitLight2 = new LightModel { Light_text = "Luminosity Percentage: ", Light_val = 0 };
            LightModel InitLight3 = new LightModel { Light_text = "Luminosity Percentage: ", Light_val = 0 };

            //initializing is done in viewmodel
            lights.Add(InitLight0);
            lights.Add(InitLight1);
            lights.Add(InitLight2);
            lights.Add(InitLight3);

            var temp = PresetLoc.Db.Query<SavedLightPresetModel>("SELECT * FROM presets");
            foreach(SavedLightPresetModel aSet in temp)
            {
                local_presets.Add(aSet);
            }//copy list in database to "local" list ->is therea way to do this directly?
            //finish binding local list, work on listbox data

            Lights = lights; //why make this distinction? AH, might be b/c it gives it an instance to start with
            other_stuff = init_other;
            Ports = ports;
            Local_Presets = local_presets;
        }


        private void ManualOn(object commandParameter)
        {
            other_stuff.Mode = (string)commandParameter;
            other_stuff.CurrPort.DiscardInBuffer();
            other_stuff.CurrPort.Write(mode_ManOn);
            other_stuff.Auto_Mode_Timer.Change(Timeout.Infinite, 250);
            UpdateControl();
        }
/*      private bool Mode_Sel(string _mode)
         {
                    if (other_stuff.Mode == _mode)
                        return true;
                    else
                        return false;
         }*/

        private bool _false()
        {
            return false;
        }
        private bool Auto_Sel()
        {
            if (other_stuff.Mode == "M")
                return true;
            else
                return false;
        }
        private bool Manual_Sel()
        {
            if (other_stuff.Mode == "A")
                return true;
            else
                return false;
        }

        private void ManualOff(object commandParameter)
        {
            other_stuff.Mode = "A";
            other_stuff.CurrPort.Write(mode_ManOff);
            other_stuff.Auto_Mode_Timer.Change(100, 250);
            UpdateControl();
        }

        private void UpdateControl()
        {
            Manual_Off_Command.InvokeCanExecuteChanged();
            Manual_On_Command.InvokeCanExecuteChanged();
            //Move_Slider_Command.InvokeCanExecuteChanged();
            Add_Preset_Command.InvokeCanExecuteChanged();
            Set_Preset_Command.InvokeCanExecuteChanged();
        }
        //OK, this is too long for just getting an int ->any shorter way to "cast" it?
        private void LightChange(object commandParameter)
        {
            int index = int.Parse((string)commandParameter); 
            Lights[index].Light_text = "Luminosity Percentage: " + Lights[index].Light_val;

            byte[] writebuffer = { 0, 0 };
            writebuffer[0] = (byte)index;
            writebuffer[1] = (byte)Lights[index].Light_val;
            other_stuff.CurrPort.Write(writebuffer, 0, 2);

            byte[]light_buffer_bytes = { 0};
            other_stuff.CurrPort.Read(light_buffer_bytes, 0, 1);
            other_stuff.PortStatus = "P> " + light_buffer_bytes[0];
            //Console.WriteLine(Lights[index].Light_val);
        }

        private void UpdatePortsList(object commandParameter)//(ObservableCollection<APortEntryModel> portz)
        {
            Ports.Clear();
            foreach (string aPort in SerialPort.GetPortNames())
            {
                Ports.Add(new APortEntryModel { PortEntry = aPort });
            }
            //if (Ports.Count > 0)
            //    Sel_Port = Ports[0];
        }

        private void CloseOff(object commandParameter)
        {
            if (other_stuff.CurrPort != null)
                other_stuff.CurrPort.Close();
            other_stuff.Auto_Mode_Timer.Dispose();
            PresetLoc.Db.Close();
        }
        
        private void ConnectPort(object commandParameter) //should map this to a different event? ->if user doesn't click a PORT, it trys to connect anyway b/c menu closed
        {
            if (Sel_Port != null)
            {
                other_stuff.CurrPort = new SerialPort(Sel_Port.PortEntry)
                {
                    BaudRate = 9600,
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One
                };

                try
                {
                    other_stuff.CurrPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    other_stuff.PortStatus = "Status: " + other_stuff.CurrPort.PortName + " already in use";
                    return;
                }
                other_stuff.CurrPort.WriteTimeout = 250;
                other_stuff.CurrPort.ReadTimeout = 250;

                other_stuff.CurrPort.Write(JustConnected);
                try
                {
                    other_stuff.Mode = other_stuff.CurrPort.ReadLine();

                    if (other_stuff.Mode == "M")
                    {
                        other_stuff.PortStatus = "Status: " + other_stuff.CurrPort.PortName + " successful";
                        UpdateControl();
                    }
                    else if (other_stuff.Mode == "A")
                    {
                        other_stuff.PortStatus = "Status: " + other_stuff.CurrPort.PortName + " successful";
                        other_stuff.Auto_Mode_Timer.Change(100, 250);
                        UpdateControl();
                    }
                    else
                    {
                        other_stuff.PortStatus = "Status: " + other_stuff.CurrPort.PortName + " not valid";
                        other_stuff.CurrPort.Close();
                    }
                }
                catch (TimeoutException)
                {
                    other_stuff.PortStatus = "Status: " + other_stuff.CurrPort.PortName + " not valid";
                    other_stuff.CurrPort.Close();
                }
            }
        }

        private void LoopAndConnect()
        {

        }

        private void Auto_Mode_Timer_Tick(object sender)
        {
            if ((other_stuff.Mode == "A") && (other_stuff.CurrPort.IsOpen))
            {
                other_stuff.CurrPort.Write(GetAutoLight);
                
                other_stuff.CurrPort.Read(auto_light_bytes, 0, auto_light_bytes.Length);

                other_stuff.Dispatcher_Timer.BeginInvoke(DispatcherPriority.Normal, (Action)auto_text_change);
            }
        }

        private void auto_text_change()
        {
            for (int ind = 0; ind < 4; ind++)
            {
                Lights[ind].Light_text = "AUTO: " + auto_light_bytes[ind];
            }
        }

        private void AddPreset(object commandParameter)
        {
            SavedLightPresetModel aNewPreset = new SavedLightPresetModel();
            aNewPreset.Light_Val0 = Lights[0].Light_val;
            aNewPreset.Light_Val1 = Lights[1].Light_val;
            aNewPreset.Light_Val2 = Lights[2].Light_val;
            aNewPreset.Light_Val3 = Lights[3].Light_val;
            aNewPreset.Preset_text = Lights[0].Light_val + " | " + Lights[1].Light_val + " | " + Lights[2].Light_val + " | " + Lights[3].Light_val;
            PresetLoc.Db.Insert(aNewPreset);
            Local_Presets.Add(aNewPreset);
        }
        private void SetPreset(object commandParameter)
        {
            if (Sel_Preset != null)
            {
                Lights[0].Light_val = Sel_Preset.Light_Val0;
                Lights[1].Light_val = Sel_Preset.Light_Val1;
                Lights[2].Light_val = Sel_Preset.Light_Val2;
                Lights[3].Light_val = Sel_Preset.Light_Val3;
                Console.WriteLine(">" + Lights[0].Light_val + " | " + Lights[1].Light_val + " | " + Lights[2].Light_val + "<");
                //TODO: still have to fix label transition when going from Auto to Manual
                //Need to correctly change labels when going from Auto->Manual->Preset
                //thinks there's more that should chaneg when changing from Auto->Manual
                //possibly add that Try all Ports button
                //possibly use threads when sending and waiting for bytes
            }
            else
                Console.WriteLine("NO SEL_PRESET SELECTED");
            //if Light_val changes -> slider moves -> command triggers -> bytes sent
            
        }
        private void RemovePreset(object commandParameter)
        {
            if(Sel_Preset != null)
            {
                PresetLoc.Db.Delete<SavedLightPresetModel>(Sel_Preset.Preset_text); //hope there aren't any complications with this
                Local_Presets.Remove(Sel_Preset);
            }
        }
    }
}


//OH, a way to simplify application PORT selection would be: press a button -> run through all available ports, sending a byte
//if it times out (no correct response) -> show message that no available ports work
//if it doesn't, show message of COM that connected
//problem: if 2 or more compatible devices connected
//could offer both options

//might need to be on a seperate thread to improve system responsiveness

//think for presets, it might be better to use a listbox maybe ->to give "preview" of settings