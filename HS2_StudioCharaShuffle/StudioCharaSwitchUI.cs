using BepInEx.Logging;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WearCustom;

namespace HS2_StudioCharaSwitch
{
    public class StudioCharaSwitchUI : MonoBehaviour
    {

        private readonly int windowID = 10564;
        private readonly string windowTitle = "Studio Charactor Switch";
        private bool IsMouseInWindow = false;
        public bool IsVisible { get; set; }
        enum GuiModeType
        {
            MAIN,
            SAVE,
        };
        private GuiModeType guiMode = GuiModeType.MAIN;


        private Rect windowRect = new Rect(0f, 300f, 600f, 400f);
        private GUIStyle largeLabel;
        private GUIStyle btnstyle;






        private OCIChar ociTarget;

        private void Start()
        {
            largeLabel = new GUIStyle("label");
            largeLabel.fontSize = 16;
            btnstyle = new GUIStyle("button");
            btnstyle.fontSize = 16;

        }

        private void OnGUI()
        {
            if (IsVisible)
            {
                try
                {
                    GUIStyle guistyle = new GUIStyle(GUI.skin.window);
                    windowRect = GUI.Window(windowID, windowRect, new GUI.WindowFunction(FunctionWindowGUI), windowTitle, guistyle);

                    IsMouseInWindow = windowRect.Contains(Event.current.mousePosition);
                    if (IsMouseInWindow)
                    {
                        Studio.Studio.Instance.cameraCtrl.noCtrlCondition = (() => IsMouseInWindow && IsVisible);
                        Input.ResetInputAxes();
                    }
                }
                catch (Exception ex)
                {

                    StudioCharaSwitchPlugin.Logger.Log(LogLevel.Error, ex.ToString());
                }
            }
        }

        private void FunctionWindowGUI(int winID)
        {

            if (GUIUtility.hotControl == 0)
            {

            }
            if (Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl("");
                GUI.FocusWindow(winID);

            }
            GUI.enabled = true;

            switch (guiMode)
            {
                case GuiModeType.MAIN:
                    guiEditorMain();
                    break;
                case GuiModeType.SAVE:
                    //guiSave();
                    break;
                default:
                    break;
            }

            GUI.DragWindow();

        }


        private void Update()
        {
            // hotkey check
            if (StudioCharaSwitchPlugin.UIHotKeyShow.Value.IsDown())
            {
                IsVisible = !IsVisible;
                if (IsVisible)
                {
                    //CharaEditorMgr.Instance.ReloadDictionary();
                    //windowRect = new Rect(StudioCharaEditor.UIXPosition.Value, StudioCharaEditor.UIYPosition.Value, Math.Max(600, StudioCharaEditor.UIWidth.Value), Math.Max(400, StudioCharaEditor.UIHeight.Value));
                }
                else
                {
                    //StudioCharaEditor.UIXPosition.Value = (int)windowRect.x;
                    //StudioCharaEditor.UIYPosition.Value = (int)windowRect.y;
                    //StudioCharaEditor.UIWidth.Value = (int)windowRect.width;
                    //StudioCharaEditor.UIHeight.Value = (int)windowRect.height;
                }
            }

            // change select check
            if (IsVisible)
            {
                TreeNodeObject curSel = GetCurrentSelectedNode();
                if (curSel != lastSelectedTreeNode)
                {
                    OnSelectChange(curSel);
                }
            }

            // house keeping
            //StudioCharaSwitchMgr.Instance.HouseKeeping(IsVisible);

            // check todo queue
            //if (ToDoQueue.Count > 0)
            //{
            //    Action p = ToDoQueue.Dequeue();
            //    p();
            //}
        }



        public List<string> CharaPathArr { get; private set; } = new List<string>();

        private void guiEditorMain()
        {
            float fullw = windowRect.width - 20;
            float fullh = windowRect.height - 20;
            float leftw = 150;
            float rightw = fullw - 8 - leftw - 5;
            GUIStyle cat1btnstyle = new GUIStyle("button");



            // 关闭按钮
            var cbRect = new Rect(windowRect.width - 16, 3, 13, 13);
            var oldColor = GUI.color;
            GUI.color = Color.red;
            if (GUI.Button(cbRect, ""))
            {
                IsVisible = false;
            }
            GUI.color = oldColor;

            //var cec = StudioCharaSwitchMgr.Instance.GetEditorController(ociTarget);
            //if (ociTarget == null || cec == null)
            //{
            //    GUILayout.FlexibleSpace();
            //    GUILayout.BeginHorizontal();
            //    GUILayout.FlexibleSpace();
            //    GUILayout.Label("<color=#00ffff>" + LC("Please select a charactor to edit.") + "</color>", largeLabel);
            //    GUILayout.FlexibleSpace();
            //    GUILayout.EndHorizontal();
            //    GUILayout.FlexibleSpace();

            //    return;
            //}

            GUILayout.BeginHorizontal();
            // LEFT area
            GUILayout.BeginVertical(GUILayout.Width(leftw + 8));
            // catelog1 select
            GUILayout.BeginHorizontal();


            //var oldColor = GUI.color;
            if (GUILayout.Button(LC("测试随机换人")))
            {

                //var dirInfo = new DirectoryInfo(@"E:\BaiduSyncdisk\HS2 [人物卡共用]\腋猫子 [5]");

                //List<string> charaPathArr = CharaPathArr;
                //charaPathArr.Clear();
                //foreach (FileInfo item in from x in dirInfo.GetFiles()
                //                          orderby x.LastWriteTime descending
                //                          select x)
                //{
                //    if (item.Extension.ToLower() == ".png")
                //    {
                //        charaPathArr.Add(item.FullName);
                //    }
                //}

                // 测试 获取整个树
                var dicInfo = Studio.Studio.Instance.dicInfo;
                StudioCharaSwitchPlugin.Logger.LogDebug($@"获取整个树：
                    数量：{dicInfo.Count}
                    ");

                var array2 = GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x)).Where(x => x != null).ToList();

                //{ JsonConvert.SerializeObject(item) }
                foreach (var item in array2)
                {
                    StudioCharaSwitchPlugin.Logger.LogDebug($@"获取选中物体：
                     kind { item.kind }
                     kinds { string.Join(",", item.kinds) }
                     ToString() { string.Join(",", item.ToString()) }
                     objectInfo { item.objectInfo.ToString() }
                     objectInfo.dicKey { item.objectInfo.dicKey }
                     objectInfo.treeState { item.objectInfo.treeState }
                     objectInfo.visible { item.objectInfo.visible }
                     treeNodeObject { item.treeNodeObject.ToString()}
                     treeNodeObject.name { item.treeNodeObject.name}
                     treeNodeObject.textName { item.treeNodeObject.textName}
                     treeNodeObject.tag { item.treeNodeObject.tag}
                     treeNodeObject.treeNode { item.treeNodeObject.treeNode}
                     treeNodeObject.childCount { item.treeNodeObject.childCount }
                     treeNodeObject.enableAddChild { item.treeNodeObject.enableAddChild }

                    ");
                }



                // 获取选中人物
                var array = GuideObjectManager.Instance.selectObjectKey.Select(x => Studio.Studio.GetCtrlInfo(x) as OCIChar).Where(x => x != null).ToList();

                foreach (var item in array)
                {

                    StudioCharaSwitchPlugin.Logger.LogDebug($@"获取选中人物物体：
                     sex { item.sex }
                     charFileStatus { item.charFileStatus }
                     charInfo { item.charInfo }
                     oiCharInfo { item.oiCharInfo }
                    ");
                    item.ChangeChara(@"E:\BaiduSyncdisk\HS2 [人物卡共用]\new [3]\AISChaF_20200101123928407.png");
                }



                //OCIChar[] array = (from v in Singleton<GuideObjectManager>.Instance.selectObjectKey.Select(delegate (int v)
                //{
                //    ObjectCtrlInfo ctrlInfo = Studio.GetCtrlInfo(v);
                //    return (OCIChar)(object)((ctrlInfo is OCIChar) ? ctrlInfo : null);
                //})
                //                   where v != null
                //                   where v.get_oiCharInfo().get_sex() == _charaData.Sex
                //                   select v).ToArray();

                //int num = array.Count;
                //for (int i = 0; i < num; i++)
                //{
                //    array[i].ChangeChara("pngpath");
                //}
                //if (num > 0)
                //{
                //    //ItemComa<string>.Loader.HideOrShowPanel();
                //}

            }
            //GUI.color = oldColor;



        }


        // Localize
        public Dictionary<string, string> curLocalizationDict;
        private string LC(string org)
        {
            if (curLocalizationDict != null && curLocalizationDict.ContainsKey(org) && !string.IsNullOrWhiteSpace(curLocalizationDict[org]))
                return curLocalizationDict[org];
            else
                return org;
        }



        protected TreeNodeObject GetCurrentSelectedNode()
        {
            return Studio.Studio.Instance.treeNodeCtrl.selectNode;
        }

        private void OnSelectChange(TreeNodeObject newSel)
        {
            lastSelectedTreeNode = newSel;
            ociTarget = GetOCICharFromNode(newSel);
            Console.WriteLine("Select change to {0}", ociTarget);
        }

        private TreeNodeObject lastSelectedTreeNode;


        protected OCIChar GetOCICharFromNode(TreeNodeObject node)
        {
            if (node == null) return null;

            var dic = Studio.Studio.Instance.dicInfo;
            if (dic.ContainsKey(node))
            {
                ObjectCtrlInfo oci = dic[node];
                if (oci is OCIChar)
                {
                    return oci as OCIChar;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        // 替换外形
        public static void LoadAnatomy(string filePath)
        {
            CallWearCustom(filePath, anatomy);
        }

        public static void LoadOutfit(string filePath)
        {
            CallWearCustom(filePath, outfit);
        }
        private static readonly bool[] outfit = new bool[5] { false, false, false, true, true };




        private static Type _studioCharaListUtilType;
        public static Type StudioCharaListUtilType => _studioCharaListUtilType ?? (_studioCharaListUtilType = Type.GetType("WearCustom.StudioCharaListUtil, HS2WearCustom, Version=0.4.0.0, Culture=neutral, PublicKeyToken=null"));
        private static readonly bool[] anatomy = new bool[5] { true, true, true, false, false };
        public static void CallWearCustom(string fileFullName, bool[] loadState)
        {
            if (!string.IsNullOrEmpty(fileFullName))
            {
                GameObject val = GameObject.Find("StudioScene/Canvas Main Menu/02_Manipulate/00_Chara");
                StudioCharaListUtil component = val.GetComponent<StudioCharaListUtil>();
                if (component == null)
                {
                    StudioCharaListUtil.Install();
                    component = val.GetComponent<StudioCharaListUtil>();
                }
                FieldInfo field = StudioCharaListUtilType.GetField("replaceCharaHairOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo field2 = StudioCharaListUtilType.GetField("replaceCharaHeadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo field3 = StudioCharaListUtilType.GetField("replaceCharaBodyOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo field4 = StudioCharaListUtilType.GetField("replaceCharaClothesOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo field5 = StudioCharaListUtilType.GetField("replaceCharaAccOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo[] array = new FieldInfo[5] { field3, field2, field, field4, field5 };
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].SetValue(component, loadState[i]);
                }
                FieldInfo field6 = StudioCharaListUtilType.GetField("charaFileSort", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo method = StudioCharaListUtilType.GetMethod("ChangeChara", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);
                object value = field6.GetValue(component);
                object obj = ((value is CharaFileSort) ? value : null);
                ((CharaFileSort)obj).cfiList.Clear();
                CharaFileInfo val2 = new CharaFileInfo(fileFullName, "Bobby");
                val2.node = new ListNode();
                val2.select = true;
                CharaFileInfo item = val2;
                ((CharaFileSort)obj).cfiList.Add(item);
                ((CharaFileSort)obj).select = 0;
                method.Invoke(component, new object[0]);
            }


        }

    }
}
