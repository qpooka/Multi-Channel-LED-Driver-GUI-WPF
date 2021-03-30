using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.INPC;
using SQLite;

namespace MVVM.model
{
    [Table("presets")]
    class SavedLightPresetModel : BaseINPC
    {
        
        private string _preset_text;
        [PrimaryKey]
        public string Preset_text
        {
            get { return _preset_text; }
            set { _preset_text = value; }
            
        }

        private int _light_val0;
        public int Light_Val0
        {
            get { return _light_val0; }
            set { _light_val0 = value; }
        }
        private int _light_val1;
        public int Light_Val1
        {
            get { return _light_val1; }
            set { _light_val1 = value; }
        }
        private int _light_val2;
        public int Light_Val2
        {
            get { return _light_val2; }
            set { _light_val2 = value; }
        }
        private int _light_val3;
        public int Light_Val3
        {
            get { return _light_val3; }
            set { _light_val3 = value; }
        }
        /*
                public SavedLightPresetModel(int inLight0, int inLight1, int inLight2, int inLight3)
                {
                    _light_val0 = inLight0;
                    _light_val1 = inLight1;
                    _light_val2 = inLight2;
                    _light_val3 = inLight3;
                    _preset_text = "" + inLight0 + " | " + inLight1 + " | " + inLight2 + " | " + inLight3;
                }
        */
        //Query<> is uncompatible with a constructor with parameters
    }
    class DatabaseHandler
    {
        private SQLiteConnection _db;
        public DatabaseHandler(String DataSource)
        {
            _db = new SQLiteConnection(DataSource);
            _db.CreateTable<SavedLightPresetModel>();
        }

        public SQLiteConnection Db
        {
            get { return _db; }
            set { _db = value; }
        }
    }
    //is there a way to "put in" a class as a paramter? thought there was a way
}

//keep in mind itemtemplates and datatemplates for more complicated display schemes
/*
 <ListBox.ItemTemplate>
     <DataTemplate>
         <TextBlock Text="{Binding }"></TextBlock>
     </DataTemplate>
</ListBox.ItemTemplate>
*/