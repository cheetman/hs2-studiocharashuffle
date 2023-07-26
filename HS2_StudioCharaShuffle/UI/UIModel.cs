using System;

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


        [Serializable]
        public class MainModel
        {

            public string CharaPath;
            public int CharaCount;
            public bool CharaPathIsOk  = true;
            public bool CharaPathIsLoading = false;
            public string CharaPathMessage;
            public bool CharaIsSub;
            public int CharaSubDepth = 2;
            public bool CharaIsAuto;
            public bool CharaIsAutoRunning;
            public bool CharaFaceIsAuto;
            public bool CharaFaceIsAutoRunning;
            public int CharaAutoTime = 20;
            public int CharaFaceAutoTime = 20;


            public string CoordPath;
            public int CoordCount;

            public bool CoordPathIsOk  = true;
            public bool CoordPathIsLoading  = false;
            public string CoordPathMessage;

            public bool CoordIsSub;
            public int CoordSubDepth = 2;
            public bool CoordIsAuto;
            public bool CoordIsAutoRunning;
            public int CoordAutoTime  = 20;
            public bool CoordIsOne;
            public bool CoordIsRepeat;

            public byte[] ClothStatus = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            public byte[] ClothStatusMan = new byte[4] { 0, 0, 0, 0};


        }













    }
}
