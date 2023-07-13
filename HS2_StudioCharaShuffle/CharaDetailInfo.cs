using AIChara;
using CharaCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HS2_StudioCharaSwitch
{
    public class CharaDetailInfo
    {
        public CharaDetailDefine DetailDefine;
        public object RevertValue;

        public CharaDetailInfo(ChaControl chaCtrl, CharaDetailDefine detailDefine)
        {
            DetailDefine = detailDefine;
            RevertValue = detailDefine.Get != null ? detailDefine.Get(chaCtrl) : null;
        }
    }


    public class CharaDetailDefine
    {
        public enum CharaDetailDefineType
        {
            // non-data
            UNKNOWN,
            SEPERATOR,
            BUTTON,
            // continuous
            SLIDER,
            COLOR,
            HAIR_BUNDLE,
            VALUEINPUT,
            ABMXSET1,
            ABMXSET2,
            ABMXSET3,
            // discrete
            SELECTOR,
            TOGGLE,
            INT_STATUS,
            SKIN_OVERLAY,
            CLOTH_OVERLAY,
        };
        public enum CharaDetailDefineCatelog
        {
            VANILLA,
            ABMX,
            BOOBSETTING,
            OVERLAY,
        };
        public delegate object GetFunc(ChaControl chaCtrl);
        public delegate void SetFunc(ChaControl chaCtrl, object value);
        public delegate void UpdFunc(ChaControl chaCtrl);
        public delegate object DefFunc(ChaControl chaCtrl);
        public delegate List<CustomSelectInfo> LstFunc(ChaControl chaCtrl);

        // define
        public string Key = "";
        public CharaDetailDefineType Type = CharaDetailDefineType.UNKNOWN;
        public CharaDetailDefineCatelog Catelog = CharaDetailDefineCatelog.VANILLA;
        public GetFunc Get;
        public SetFunc Set;
        public UpdFunc Upd;
        public LstFunc SelectorList;

        // helper
        public virtual bool IsData
        {
            get
            {
                return Type != CharaDetailDefineType.UNKNOWN &&
                       Type != CharaDetailDefineType.SEPERATOR &&
                       Type != CharaDetailDefineType.BUTTON;
            }
        }

        public virtual bool IsContinuousData
        {
            get
            {
                return Type == CharaDetailDefineType.SLIDER ||
                       Type == CharaDetailDefineType.COLOR ||
                       Type == CharaDetailDefineType.HAIR_BUNDLE ||
                       Type == CharaDetailDefineType.VALUEINPUT ||
                       Type == CharaDetailDefineType.ABMXSET1 ||
                       Type == CharaDetailDefineType.ABMXSET2 ||
                       Type == CharaDetailDefineType.ABMXSET3;
            }
        }

        public virtual bool IsDiscreteData
        {
            get
            {
                return Type == CharaDetailDefineType.SELECTOR ||
                       Type == CharaDetailDefineType.TOGGLE ||
                       Type == CharaDetailDefineType.INT_STATUS ||
                       Type == CharaDetailDefineType.SKIN_OVERLAY ||
                       Type == CharaDetailDefineType.CLOTH_OVERLAY;
            }
        }

        public static bool ParseBool(object v)
        {
            if (v is bool)
            {
                return (bool)v;
            }
            else if (v is int)
            {
                return ((int)v) != 0;
            }
            else
            {
                bool retb;
                int reti;
                float retf;
                if (bool.TryParse(v.ToString(), out retb))
                    return retb;
                else if (int.TryParse(v.ToString(), out reti))
                    return reti != 0;
                else if (float.TryParse(v.ToString(), out retf))
                    return retf != 0;
                else
                    return false;
            }
        }
    }

}
