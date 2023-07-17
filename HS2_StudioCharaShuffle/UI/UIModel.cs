using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HS2_StudioCharaShuffle
{
    public class UIModel
    {

        public static UIModel Instance { get; private set; }
        public static UIModel GetInstance()
        {
            if(Instance == null)
            {
                Instance = new UIModel();
            }
            return Instance;
        }

        public MainModel Main { get; set; } = new MainModel();

        public class MainModel
        {

            public string CharaPath { get; set; }
            public bool CharaIsSub { get; set; }
            public bool CharaIsAuto { get; set; }
            public int CharaAutoTime { get; set; } = 20;


            public string CoordPath { get; set; }
            public bool CoordIsSub { get; set; }
            public bool CoordIsAuto { get; set; }
            public int CoordAutoTime { get; set; } = 20;

        }













    }
}
