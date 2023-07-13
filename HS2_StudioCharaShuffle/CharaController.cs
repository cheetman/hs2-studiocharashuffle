using AIChara;
using CharaCustom;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HS2_StudioCharaSwitch
{
    public class CharaController
    {

        public OCIChar ociTarget;
        public Dictionary<string, List<string>> myCategorySet;
        public Dictionary<string, CharaDetailInfo> myDetailDict;
        public Dictionary<string, List<CharaDetailInfo>> myDetailSet;
        public List<string> myUpdateSequence;
        public bool hairSameColor;
        public bool hairAutoColor;
        public bool textureInited;
        public bool accSortByParent;

        static public readonly char[] KEY_SEP_CHAR = new char[] { '#' };
        static public readonly string CT1_BODY = "Body";
        static public readonly string CT1_FACE = "Face";
        static public readonly string CT1_HAIR = "Hair";
        static public readonly string CT1_CTHS = "Clothes";
        static public readonly string CT1_ACCS = "Accessories";
        static public readonly string[] MALE_CLOTHES_NAME = { "Top", "Bot", "Gloves", "Shoes" };
        static public readonly string[] FEMALE_CLOTHES_NAME = { "Top", "Bot", "Inner_t", "Inner_b", "Gloves", "Panst", "Socks", "Shoes" };
        static public readonly string[] CATEGORY1 = { CT1_BODY, CT1_FACE, CT1_HAIR, CT1_CTHS, CT1_ACCS };
        static private readonly Dictionary<string, string[]> CATEGORY2_BASE_MALE = new Dictionary<string, string[]>
        {
            {CT1_BODY, new string[] {
                    "==SHAPE==",
                    "ShapeWhole",
                    "ShapeBreast",
                    "ShapeUpper",
                    "ShapeLower",
                    "ShapeArm",
                    "ShapeLeg",
                    "==SKIN==",
                    "Skin",
                    "Sunburn",
                    "Nip",
                    "Underhair",
                    "Nail",
                    "Paint1",
                    "Paint2",
                }
            },
            {CT1_FACE, new string[] {
                    "==FACE==",
                    "FaceType",
                    "ShapeWhole",
                    "ShapeChin",
                    "ShapeCheek",
                    "ShapeEyebrow",
                    "ShapeEyes",
                    "ShapeNose",
                    "ShapeMouth",
                    "ShapeEar",
                    "Mole",
                    "Bread",
                    "==EYES==",
                    "++EyesSameSetting",
                    "EyeL",
                    "EyeR",
                    "EyeEtc",
                    "EyeHL",
                    "Eyebrow",
                    "Eyelashes",
                    "==MAKEUP==",
                    "MakeupEyeshadow",
                    "MakeupCheek",
                    "MakeupLip",
                    "MakeupPaint1",
                    "MakeupPaint2",
                }
            },
            {CT1_HAIR, new string[] {
                    "++ColorAutoSetting",
                    "++ColorSameSetting",
                    "BackHair",
                    "FrontHair",
                    "SideHair",
                    "ExtensionHair",
                }
            },
            {CT1_CTHS, MALE_CLOTHES_NAME
            },
            {CT1_ACCS, new string[] { }
            },
        };
        static private readonly Dictionary<string, string[]> CATEGORY2_BASE_FEMALE = new Dictionary<string, string[]>
        {
            {CT1_BODY, new string[] {
                    "==SHAPE==",
                    "ShapeWhole",
                    "ShapeBreast",
                    "ShapeUpper",
                    "ShapeLower",
                    "ShapeArm",
                    "ShapeLeg",
                    "==SKIN==",
                    "Skin",
                    "Sunburn",
                    "Nip",
                    "Underhair",
                    "Nail",
                    "Paint1",
                    "Paint2",
                }
            },
            {CT1_FACE, new string[] {
                    "==FACE==",
                    "FaceType",
                    "ShapeWhole",
                    "ShapeChin",
                    "ShapeCheek",
                    "ShapeEyebrow",
                    "ShapeEyes",
                    "ShapeNose",
                    "ShapeMouth",
                    "ShapeEar",
                    "Mole",
                    "==EYES==",
                    "++EyesSameSetting",
                    "EyeL",
                    "EyeR",
                    "EyeEtc",
                    "EyeHL",
                    "Eyebrow",
                    "Eyelashes",
                    "==MAKEUP==",
                    "MakeupEyeshadow",
                    "MakeupCheek",
                    "MakeupLip",
                    "MakeupPaint1",
                    "MakeupPaint2",
                }
            },
            {CT1_HAIR, new string[] {
                    "++ColorAutoSetting",
                    "++ColorSameSetting",
                    "BackHair",
                    "FrontHair",
                    "SideHair",
                    "ExtensionHair",
                }
            },
            {CT1_CTHS, FEMALE_CLOTHES_NAME
            },
            {CT1_ACCS, new string[] { }
            },
        };

        // extend plugins
        public object PushUpController { get; private set; }
        public bool HasPushUpPlugin
        {
            get
            {
                return PushUpController != null;
            }
        }
        public object BoobController { get; private set; }
        public bool HasBoobSettingPlugin
        {
            get
            {
                return BoobController != null;
            }
        }
        public object BoneController { get; private set; }
        public bool HasABMXPlugin
        {
            get
            {
                return BoneController != null;
            }
        }
        public object SkinOverlayContrller { get; private set; }
        public object ClothOverlayContrller { get; private set; }
        public bool HasOverlayPlugin
        {
            get
            {
                return SkinOverlayContrller != null && ClothOverlayContrller != null;
            }
        }


        public CharaController(OCIChar target)
        {
            ociTarget = target;
        }


        public void Initialize()
        {
            // check plugins
            //try
            //{
            //    InitPushUpCtrl();
            //}
            //catch (Exception)
            //{
            //    PushUpController = null;
            //}
            //try
            //{
            //    InitBoobCtrl();
            //}
            //catch (Exception)
            //{
            //    BoobController = null;
            //}
            //try
            //{
            //    InitABMXCtrl();
            //}
            //catch (Exception)
            //{
            //    BoneController = null;
            //}
            //try
            //{
            //    InitOverlayCtrl();
            //}
            //catch (Exception)
            //{
            //    SkinOverlayContrller = null;
            //    ClothOverlayContrller = null;
            //}


            try
            {
                InitData();
            }
            catch (Exception ex)
            {
                StudioCharaSwitchPlugin.Logger.LogError(ex.ToString());
            }


            //CheckHairColor();
        }

        public void InitData()
        {
            ChaControl chaCtrl = ociTarget.charInfo;
            myCategorySet = new Dictionary<string, List<string>>();
            myDetailDict = new Dictionary<string, CharaDetailInfo>();
            myDetailSet = new Dictionary<string, List<CharaDetailInfo>>();
            myUpdateSequence = new List<string>();


            bool isDetailInCategory(string cdiKey)
            {
                string[] segs = cdiKey.Split(KEY_SEP_CHAR);
                if (segs.Length != 3)
                    return false;
                if (!myCategorySet.ContainsKey(segs[0]))
                    return false;
                return myCategorySet[segs[0]].Contains(segs[1]);
            }

            void addToDetailSet(CharaDetailInfo cdi)
            {
                string key = cdi.DetailDefine.Key;
                string[] segs = key.Split(KEY_SEP_CHAR);
                string setName = segs[0] + "#" + segs[1];
                if (!myDetailSet.ContainsKey(setName))
                {
                    myDetailSet[setName] = new List<CharaDetailInfo>();
                }
                myDetailSet[setName].Add(cdi);
                myDetailDict[key] = cdi;
            }

            void addToUpdateSequence(CharaDetailInfo cdi)
            {
                if (!cdi.DetailDefine.IsData)
                {
                    return;
                }
                if (!myUpdateSequence.Contains(cdi.DetailDefine.Key))
                {
                    myUpdateSequence.Add(cdi.DetailDefine.Key);
                }
            }

            void addToUpdateSequenceList(string[] seqKeys)
            {
                foreach (string key in seqKeys)
                {
                    if (!myUpdateSequence.Contains(key))
                    {
                        myUpdateSequence.Add(key);
                    }
                }
            }

            // category set
            foreach (string category in CATEGORY1)
            {
                // base
                List<string> cset = new List<string>();
                cset.AddRange(chaCtrl.sex == 0 ? CATEGORY2_BASE_MALE[category] : CATEGORY2_BASE_FEMALE[category]);

                // body overlay
                if (category == CT1_BODY && HasOverlayPlugin)
                {
                    cset.Add("==OVERLAY==");
                    cset.Add("Overlay");
                }

                // face overlay
                if (category == CT1_FACE && HasOverlayPlugin)
                {
                    cset.Add("==OVERLAY==");
                    cset.Add("Overlay");
                }

                // clothes
                if (category == CT1_CTHS)
                {
                    // Nothing here
                }

                // accessories
                if (category == CT1_ACCS)
                {
                    //cset = BuildAccessoriesList();
                }

                myCategorySet[category] = cset;
            }

            // vanilla chara detail set
            //foreach (CharaDetailDefine cdd in CharaDetailSet.Details)
            //{
            //    if (!isDetailInCategory(cdd.Key))
            //    {
            //        continue;
            //    }

            //    CharaDetailInfo cdi = new CharaDetailInfo(chaCtrl, cdd);
            //    addToDetailSet(cdi);
            //    addToUpdateSequence(cdi);
            //}

            //// clothes
            //foreach (string clothName in myCategorySet[CT1_CTHS])
            //{
            //    //string setName = CT1_CTHS + "#" + clothName;
            //    int clothIndex = myCategorySet[CT1_CTHS].IndexOf(clothName);
            //    foreach (CharaDetailDefine cdd in CharaDetailSet.ClothDetailBuilder(chaCtrl, clothIndex))
            //    {
            //        CharaDetailInfo cdi = new CharaDetailInfo(chaCtrl, cdd);
            //        addToDetailSet(cdi);
            //    }
            //    // update sequence key
            //    addToUpdateSequenceList(CharaDetailSet.ClothUpdateSequenceKeyBuilder(chaCtrl, clothIndex));
            //}

            //// accessories
            //foreach (string accKey in myCategorySet[CT1_ACCS])
            //{
            //    foreach (CharaDetailDefine cdd in CharaDetailSet.AccessoryDetailBuilder(chaCtrl, accKey))
            //    {
            //        CharaDetailInfo cdi = new CharaDetailInfo(chaCtrl, cdd);
            //        addToDetailSet(cdi);
            //    }
            //    // update sequence key
            //    addToUpdateSequenceList(CharaDetailSet.AccessoryUpdateSequenceKeyBuilder(chaCtrl, accKey));
            //}
        }


        /// <summary>
        /// build myAccessoriesInfo data, which is always slot number sorted,
        /// and return a string list of valid acc keys, which affected by sort mode
        /// </summary>
        /// <returns>list of valid acc keys</returns>
        //public List<string> BuildAccessoriesList()
        //{
        //    ChaControl chaCtrl = ociTarget.charInfo;
        //    myAccessoriesInfo = new List<AccessoryInfo>();
        //    List<string> accKeys = new List<string>();
        //    int accCount = PluginMoreAccessories.GetAccessoryCount(chaCtrl);
        //    // build myAccessoriesInfo
        //    for (int slotNo = 0; slotNo < accCount; slotNo++)
        //    {
        //        var ai = new AccessoryInfo(chaCtrl, slotNo);
        //        myAccessoriesInfo.Add(ai);
        //        if (!accSortByParent) accKeys.Add(ai.AccKey);
        //    }
        //    // sort by parent
        //    if (accSortByParent)
        //    {
        //        List<CustomSelectInfo> parentSelectList = CharaDetailSet.GetAccessoryParentSelectList();
        //        foreach (CustomSelectInfo parentInfo in parentSelectList)
        //        {
        //            bool hadChild = false;
        //            for (int slotNo = 0; slotNo < accCount; slotNo++)
        //            {
        //                AccessoryInfo ai = myAccessoriesInfo[slotNo];
        //                if (!ai.IsEmptySlot && ChaAccessoryDefine.GetAccessoryParentInt(ai.partsInfo.parentKey) == parentInfo.id)
        //                {
        //                    if (!hadChild)
        //                    {
        //                        accKeys.Add("==" + parentInfo.name + "==");
        //                        hadChild = true;
        //                    }
        //                    accKeys.Add(ai.AccKey);
        //                }
        //            }
        //        }
        //    }

        //    return accKeys;
        //}

    }
}
