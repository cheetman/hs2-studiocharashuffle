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
            public int CharaCount { get; set; }
            public bool CharaPathIsOk { get; set; } = true;
            public bool CharaPathIsLoading { get; set; } = false;
            public string CharaPathMessage { get; set; }
            public bool CharaIsSub { get; set; }
            public int CharaSubDepth { get; set; } = 2;
            public bool CharaIsAuto { get; set; }
            public bool CharaIsAutoRunning { get; set; }
            public bool CharaFaceIsAuto { get; set; }
            public bool CharaFaceIsAutoRunning { get; set; }
            public int CharaAutoTime { get; set; } = 20;
            public int CharaFaceAutoTime { get; set; } = 20;


            public string CoordPath { get; set; }
            public int CoordCount { get; set; }

            public bool CoordPathIsOk { get; set; } = true;
            public bool CoordPathIsLoading { get; set; } = false;
            public string CoordPathMessage { get; set; }

            public bool CoordIsSub { get; set; }
            public int CoordSubDepth { get; set; } = 2;
            public bool CoordIsAuto { get; set; }
            public bool CoordIsAutoRunning { get; set; }
            public int CoordAutoTime { get; set; } = 20;
            public bool CoordIsOne { get; set; }
            public bool CoordIsRepeat { get; set; }


        }













    }
}
