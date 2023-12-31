﻿using AIChara;
using BepInEx.Logging;
using KKAPI.Studio;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WearCustom;

namespace HS2_StudioCharaShuffle
{
    public class UI : MonoBehaviour
    {

        private readonly int windowID = 10564;
        private readonly string windowTitle = "Studio Charactor Shuffle";
        private bool IsMouseInWindow = false;
        public bool IsVisible { get; set; }
        enum GuiModeType
        {
            MAIN,
        };
        private GuiModeType guiMode = GuiModeType.MAIN;


        private Rect windowRect = new Rect(0f, 300f, 600f, 400f);

        private UIModel Model;

        private int selectedTreeIndex = 0;
        private HashSet<int> mySelectedTreeIndexs = new HashSet<int>();


        private void Start()
        {

            Model = UIModel.GetInstance();
            Model.Main.CharaPath = Utils.GetCharaPath(1).FullName;
            Model.Main.CharaPathIsOk = true;
            Model.Main.CoordPath = Utils.GetCoordPath(1).FullName;
            Model.Main.CoordPathIsOk = true;
            LoadLastConfig();


            StudioSaveLoadApi.SceneLoad += (object sender, SceneLoadEventArgs e) =>
            {
                mySelectedTreeIndexs.Clear();
                if (IsVisible)
                {
                    Utils.BuildTreeCharaInfoList();
                }
            };


            Studio.Studio.Instance.treeNodeCtrl.onSelect += (TreeNodeObject treeNodeObject) =>
            {
                if (IsVisible)
                {
                    if (Studio.Studio.Instance.dicInfo.TryGetValue(treeNodeObject, out ObjectCtrlInfo obj))
                    {
                        selectedTreeIndex = obj.objectInfo.dicKey;
                    }
                    Utils.BuildTreeCharaInfoList();
                }
            };

            Studio.Studio.Instance.treeNodeCtrl.onDelete += (treeNodeObject) =>
            {

                if (IsVisible)
                {
                    Utils.BuildTreeCharaInfoList();
                }
            };
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

                    StudioCharaShufflePlugin.Logger.Log(LogLevel.Error, ex.ToString());
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
                default:
                    break;
            }

            GUI.DragWindow();
        }


        private void Update()
        {
            if (StudioCharaShufflePlugin.UIHotKeyShow.Value.IsDown())
            {
                IsVisible = !IsVisible;
                if (IsVisible)
                {
                    Utils.BuildTreeCharaInfoList();
                }
            }

            // 随机人物卡
            else if (StudioCharaShufflePlugin.UIHotKey1.Value.IsDown())
            {
                UI.ToDoQueue.Enqueue(() => RandomChara());
            }
            // 随机人物卡外观
            else if (StudioCharaShufflePlugin.UIHotKey2.Value.IsDown())
            {
                UI.ToDoQueue.Enqueue(() => RandomCharaWithoutCloth());
            }
            // 随机服装卡
            else if (StudioCharaShufflePlugin.UIHotKey3.Value.IsDown())
            {
                UI.ToDoQueue.Enqueue(() => RandomCloth());
            }


            //check todo queue
            if (ToDoQueue.Count > 0)
            {
                Action p = ToDoQueue.Dequeue();
                p();
            }
        }

        public static Queue<Action> ToDoQueue = new Queue<Action>();


        private Vector2 leftScroll = Vector2.zero;
        private Vector2 rightScroll = Vector2.zero;
        public List<string> CharaPathArr { get; private set; } = new List<string>();

        private void guiEditorMain()
        {
            float fullw = windowRect.width - 20;
            float fullh = windowRect.height - 20;
            float leftw = 150;
            float rightw = fullw - 8 - leftw - 5;

            var oldColor = GUI.color;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(leftw + 8));


            if (GUILayout.Button(LC("刷新")))
            {
                Utils.BuildTreeCharaInfoList();
            }


            leftScroll = GUILayout.BeginScrollView(leftScroll, GUI.skin.box);

            foreach (var item in Utils.GetTreeCharaInfoDic())
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.gray; ;
                GUILayout.Label($"{item.Key}");
                GUI.color = oldColor;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                foreach (var info in item.Value)
                {
                    //if (selectedTreeIndex.Contains(info.Index))
                    if (selectedTreeIndex == info.Index)
                    {
                        GUI.color = Color.green;
                    }
                    else if (!info.IsVisible)
                    {
                        GUI.color = Color.yellow; ;
                    }

                    bool OldIsSelected = mySelectedTreeIndexs.Contains(info.Index);
                    info.IsSelected = OldIsSelected; // 这行只给判断使用
                    bool IsSelected = GUILayout.Toggle(OldIsSelected, $" {(info.Sex == 1 ? "[女]" : "[男]")} {info.Name}");
                    if (IsSelected != OldIsSelected)
                    {
                        if (IsSelected)
                        {
                            mySelectedTreeIndexs.Add(info.Index);
                        }
                        else
                        {
                            mySelectedTreeIndexs.Remove(info.Index);
                        }
                    }
                    GUI.color = oldColor;
                }
            }

            GUILayout.EndScrollView();


            if (GUILayout.Button(LC("随机人物卡")))
            {
                UI.ToDoQueue.Enqueue(() => RandomChara());
            }

            if (GUILayout.Button(LC("随机人物外形")))
            {
                UI.ToDoQueue.Enqueue(() => RandomCharaWithoutCloth());
            }

            if (GUILayout.Button(LC("随机服装卡")))
            {
                UI.ToDoQueue.Enqueue(() => RandomCloth());

            }


            GUILayout.EndVertical();


            GUILayout.BeginVertical();



            rightScroll = GUILayout.BeginScrollView(rightScroll, GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label(LC("人物卡随机目录"), GUI.skin.box);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label(LC("路径："), GUILayout.Width(250));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();


            if (Model.Main.CharaPathIsLoading)
            {
                GUI.enabled = false;
            }

            string newTxtV = string.Empty;
            if (!Model.Main.CharaPathIsOk)
            {
                GUI.color = Color.red;
                newTxtV = GUILayout.TextField(Model.Main.CharaPath, GUILayout.Width(250));
                GUI.color = oldColor;
            }
            else
            {
                newTxtV = GUILayout.TextField(Model.Main.CharaPath, GUILayout.Width(250));
            }



            if (GUILayout.Button(LC("选择目录"), GUILayout.Width(100)))
            {
                OpenFileDialog.Show((files) =>
                {
                    if (files != null && files.Length > 0)
                    {
                        string pathname = files[0];
                        Model.Main.CharaPath = Path.GetDirectoryName(pathname);
                        UI.ToDoQueue.Enqueue(() => RefreshCharaCards());
                    }
                }, "Save Charactor", Model.Main.CharaPath, "Images (*.png)|*.png|All files|*.*", ".png", OpenFileDialog.OpenSaveFileDialgueFlags.OFN_EXPLORER | OpenFileDialog.OpenSaveFileDialgueFlags.OFN_LONGNAMES);

            }

            GUILayout.FlexibleSpace();



            GUILayout.EndHorizontal();


            if (!Model.Main.CharaPathIsOk)
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.red;
                GUILayout.Label(Model.Main.CharaPathMessage);
                GUI.color = oldColor;
                GUILayout.EndHorizontal();
            }


            GUILayout.BeginHorizontal();
            GUILayout.Label(LC("识别人物卡数量："), GUILayout.Width(110));
            GUILayout.Label(Model.Main.CharaCount.ToString(), GUILayout.Width(30));
            if (GUILayout.Button(LC("刷新"), GUILayout.Width(50)))
            {
                UI.ToDoQueue.Enqueue(() => RefreshCharaCards());
            }


            GUILayout.EndHorizontal();




            bool IsChecked = GUILayout.Toggle(Model.Main.CharaIsSub, $"是否包含子目录");
            if (IsChecked != Model.Main.CharaIsSub)
            {
                Model.Main.CharaIsSub = IsChecked;
            }
            if (IsChecked)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LC("子目录深度："), GUILayout.Width(80));
                float newValue = GUILayout.HorizontalSlider(Model.Main.CharaSubDepth, 1f, 5f, GUILayout.Width(80));
                int newValueI = (int)newValue;
                if (newValueI != Model.Main.CharaSubDepth)
                {
                    Model.Main.CharaSubDepth = newValueI;
                }
                GUILayout.Label($"{newValueI}层");
                GUILayout.EndHorizontal();
            }

            if (!Model.Main.CharaPathIsOk)
            {
                GUI.enabled = false;
            }

            var oldEnabled = GUI.enabled;
            if (Model.Main.CharaIsAutoRunning)
            {
                GUI.enabled = false;
            }

            if (Model.Main.CharaIsAuto)
            {
                GUI.color = Color.green;
            }
            IsChecked = GUILayout.Toggle(Model.Main.CharaIsAuto, "是否自动替换");
            GUI.color = oldColor;
            if (IsChecked != Model.Main.CharaIsAuto)
            {
                Model.Main.CharaIsAuto = IsChecked;
            }
            if (IsChecked)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LC("替换频率："), GUILayout.Width(70));
                float newValue = GUILayout.HorizontalSlider(Model.Main.CharaAutoTime, 10f, 300f, GUILayout.Width(80));
                int newValueI = (int)newValue;
                if (newValueI != Model.Main.CharaAutoTime)
                {
                    Model.Main.CharaAutoTime = newValueI;
                }
                GUILayout.Label($"{newValueI}秒", GUILayout.Width(50));


                if (Model.Main.CharaIsAutoRunning)
                {
                    GUI.enabled = oldEnabled;
                }

                if (GUILayout.Button(Model.Main.CharaIsAutoRunning ? LC("停止") : LC("启动"), GUILayout.Width(50)))
                {
                    if (Model.Main.CharaIsAutoRunning)
                    {
                        CancelInvoke("RandomChara");
                        Model.Main.CharaIsAutoRunning = false;
                    }
                    else
                    {
                        InvokeRepeating("RandomChara", Model.Main.CharaAutoTime, Model.Main.CharaAutoTime);
                        Model.Main.CharaIsAutoRunning = true;
                    }
                }


                GUILayout.EndHorizontal();
            }

            oldEnabled = GUI.enabled;
            if (Model.Main.CharaFaceIsAutoRunning)
            {
                GUI.enabled = false;
            }
            if (Model.Main.CharaFaceIsAuto)
            {
                GUI.color = Color.green;
            }
            IsChecked = GUILayout.Toggle(Model.Main.CharaFaceIsAuto, "是否自动替换外观");
            GUI.color = oldColor;
            if (IsChecked != Model.Main.CharaFaceIsAuto)
            {
                Model.Main.CharaFaceIsAuto = IsChecked;
            }
            if (IsChecked)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LC("替换频率："), GUILayout.Width(70));
                float newValue = GUILayout.HorizontalSlider(Model.Main.CharaFaceAutoTime, 10f, 300f, GUILayout.Width(80));
                int newValueI = (int)newValue;
                if (newValueI != Model.Main.CharaFaceAutoTime)
                {
                    Model.Main.CharaFaceAutoTime = newValueI;
                }
                GUILayout.Label($"{newValueI}秒", GUILayout.Width(50));

                if (Model.Main.CharaFaceIsAutoRunning)
                {
                    GUI.enabled = oldEnabled;
                }

                if (GUILayout.Button(Model.Main.CharaFaceIsAutoRunning ? LC("停止") : LC("启动"), GUILayout.Width(50)))
                {
                    if (Model.Main.CharaFaceIsAutoRunning)
                    {
                        CancelInvoke("RandomCharaWithoutCloth");
                        Model.Main.CharaFaceIsAutoRunning = false;
                    }
                    else
                    {
                        InvokeRepeating("RandomCharaWithoutCloth", Model.Main.CharaFaceAutoTime, Model.Main.CharaFaceAutoTime);
                        Model.Main.CharaFaceIsAutoRunning = true;
                    }
                }


                GUILayout.EndHorizontal();
            }

            GUI.enabled = true;



            GUILayout.BeginHorizontal();
            GUILayout.Label(LC("服装卡随机目录"), GUI.skin.box);
            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal();
            GUILayout.Label(LC("路径："), GUILayout.Width(250));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();


            if (Model.Main.CoordPathIsLoading)
            {
                GUI.enabled = false;
            }

            newTxtV = GUILayout.TextField(Model.Main.CoordPath, GUILayout.Width(250));

            if (GUILayout.Button(LC("选择目录"), GUILayout.Width(100)))
            {

                OpenFileDialog.Show((files) =>
                {
                    if (files != null && files.Length > 0)
                    {
                        string pathname = files[0];
                        Model.Main.CoordPath = Path.GetDirectoryName(pathname);

                        UI.ToDoQueue.Enqueue(() => RefreshCoordCards());


                    }
                }, "Save Charactor", Model.Main.CoordPath, "Images (*.png)|*.png|All files|*.*", ".png", OpenFileDialog.OpenSaveFileDialgueFlags.OFN_EXPLORER | OpenFileDialog.OpenSaveFileDialgueFlags.OFN_LONGNAMES);

            }

            GUILayout.FlexibleSpace();


            GUILayout.EndHorizontal();




            if (!Model.Main.CoordPathIsOk)
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.red;
                GUILayout.Label(Model.Main.CoordPathMessage);
                GUI.color = oldColor;
                GUILayout.EndHorizontal();
            }



            GUILayout.BeginHorizontal();
            GUILayout.Label(LC("识别服装卡数量："), GUILayout.Width(110));
            GUILayout.Label(Model.Main.CoordCount.ToString(), GUILayout.Width(30));
            if (GUILayout.Button(LC("刷新"), GUILayout.Width(50)))
            {
                UI.ToDoQueue.Enqueue(() => RefreshCoordCards());
            }


            GUILayout.EndHorizontal();


            IsChecked = GUILayout.Toggle(Model.Main.CoordIsSub, $"是否包含子目录");
            if (IsChecked != Model.Main.CoordIsSub)
            {
                Model.Main.CoordIsSub = IsChecked;
            }
            if (IsChecked)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LC("子目录深度："), GUILayout.Width(80));
                float newValue = GUILayout.HorizontalSlider(Model.Main.CoordSubDepth, 1f, 5f, GUILayout.Width(80));
                int newValueI = (int)newValue;
                if (newValueI != Model.Main.CoordSubDepth)
                {
                    Model.Main.CoordSubDepth = newValueI;
                }
                GUILayout.Label($"{newValueI}层");
                GUILayout.EndHorizontal();
            }

            if (!Model.Main.CoordPathIsOk)
            {
                GUI.enabled = false;
            }

            IsChecked = GUILayout.Toggle(Model.Main.CoordIsOne, $"每次随机一件服装");
            if (IsChecked != Model.Main.CoordIsOne)
            {
                Model.Main.CoordIsOne = IsChecked;
            }

            IsChecked = GUILayout.Toggle(Model.Main.CoordIsRepeat, $"是否允许相同服装");
            if (IsChecked != Model.Main.CoordIsRepeat)
            {
                Model.Main.CoordIsRepeat = IsChecked;
            }

            if (Model.Main.CoordIsAuto)
            {
                GUI.color = Color.green;
            }
            oldEnabled = GUI.enabled;
            if (Model.Main.CoordIsAutoRunning)
            {
                GUI.enabled = false;
            }
            IsChecked = GUILayout.Toggle(Model.Main.CoordIsAuto, "是否自动更换");
            GUI.color = oldColor;

            if (IsChecked != Model.Main.CoordIsAuto)
            {
                Model.Main.CoordIsAuto = IsChecked;
            }
            if (IsChecked)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LC("替换频率："), GUILayout.Width(80));
                float newValue = GUILayout.HorizontalSlider(Model.Main.CoordAutoTime, 10f, 300f, GUILayout.Width(80));
                int newValueI = (int)newValue;
                if (newValueI != Model.Main.CoordAutoTime)
                {
                    Model.Main.CoordAutoTime = newValueI;
                }
                GUILayout.Label($"{newValueI}秒", GUILayout.Width(50));

                if (Model.Main.CoordIsAutoRunning)
                {
                    GUI.enabled = oldEnabled;
                }

                if (GUILayout.Button(Model.Main.CoordIsAutoRunning ? LC("停止") : LC("启动"), GUILayout.Width(50)))
                {
                    if (Model.Main.CoordIsAutoRunning)
                    {
                        CancelInvoke("RandomCloth");
                        Model.Main.CoordIsAutoRunning = false;
                    }
                    else
                    {
                        InvokeRepeating("RandomCloth", Model.Main.CoordAutoTime, Model.Main.CoordAutoTime);
                        Model.Main.CoordIsAutoRunning = true;
                    }
                }
                GUILayout.EndHorizontal();
            }




            GUI.enabled = true;





            GUILayout.BeginHorizontal();
            GUILayout.Label(LC("其他"), GUI.skin.box);
            GUILayout.EndHorizontal();


            GUILayout.Label(LC("默认服装[女]"), GUILayout.Width(80));
            var rowIndex = 0;
            foreach (var item in categoryName)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(item.Name, GUILayout.Width(80));

                if (item.Value == 3)
                {
                    foreach (var index in Enumerable.Range(0, item.Value))
                    {
                        Color oldColor2 = GUI.color;
                        if (Convert.ToByte(index) == Model.Main.ClothStatus[rowIndex])
                            GUI.color = Color.green;
                        if (GUILayout.Button(ClothStatus3Name[index], GUILayout.Width(50)))
                        {
                            // 更新服装状态
                            var charas = Utils.GetTreeCharaInfoDic().SelectMany(x => x.Value).Where(x => x.IsSelected).ToList();
                            foreach (var chara in charas)
                            {

                                var obj = Studio.Studio.GetCtrlInfo(chara.Index) as OCIChar;
                                if (obj != null)
                                {
                                    var control = obj.GetChaControl();
                                    control.SetClothesState(rowIndex, Convert.ToByte(index), true);
                                }
                            }

                            Model.Main.ClothStatus[rowIndex] = Convert.ToByte(index);
                        }
                        GUI.color = oldColor2;
                    }
                }
                else
                {
                    foreach (var index in Enumerable.Range(0, item.Value))
                    {
                        Color oldColor2 = GUI.color;
                        if (Convert.ToByte(index) == Model.Main.ClothStatus[rowIndex])
                            GUI.color = Color.green;
                        if (GUILayout.Button(ClothStatus2Name[index], GUILayout.Width(50)))
                        {
                            // 更新服装状态
                            var charas = Utils.GetTreeCharaInfoDic().SelectMany(x => x.Value).Where(x => x.IsSelected).ToList();
                            foreach (var chara in charas)
                            {

                                var obj = Studio.Studio.GetCtrlInfo(chara.Index) as OCIChar;
                                if (obj != null)
                                {
                                    var control = obj.GetChaControl();
                                    control.SetClothesState(rowIndex, Convert.ToByte(index), true);
                                }
                            }
                            Model.Main.ClothStatus[rowIndex] = Convert.ToByte(index);
                        }
                        GUI.color = oldColor2;
                    }
                }




                GUILayout.EndHorizontal();
                rowIndex++;
            }



            if (GUILayout.Button("保存配置", GUILayout.Width(80)))
            {
                SaveLastConfig();
            }



            GUILayout.EndScrollView();


            GUILayout.EndVertical();


            GUILayout.EndHorizontal();

            // 关闭按钮
            var cbRect = new Rect(windowRect.width - 16, 3, 13, 13);
            GUI.color = Color.red;
            if (GUI.Button(cbRect, ""))
            {
                IsVisible = false;
            }
            GUI.color = oldColor;
        }

        string[] ClothStatus3Name = { "穿", "脱", "裸" };
        string[] ClothStatus2Name = { "穿", "裸" };
        NameValue[] categoryName = { new NameValue("上装", 3), new NameValue("裤子", 3), new NameValue("内衣", 3), new NameValue("内裤", 3), new NameValue("手套", 2), new NameValue("连裤袜", 3), new NameValue("短袜", 2), new NameValue("鞋子", 2) };
        string[] categoryMName = { "上衣", "下装", "手套", "鞋子" };

        private ChaListDefine.CategoryNo[] MALE_CLOTH_CATEGORYNO = new ChaListDefine.CategoryNo[] {
                ChaListDefine.CategoryNo.mo_top,
                ChaListDefine.CategoryNo.mo_bot,
                ChaListDefine.CategoryNo.mo_gloves,
                ChaListDefine.CategoryNo.mo_shoes
            };
        private ChaListDefine.CategoryNo[] FEMALE_CLOTH_CATEGORYNO = new ChaListDefine.CategoryNo[] {
                ChaListDefine.CategoryNo.fo_top,
                ChaListDefine.CategoryNo.fo_bot,
                ChaListDefine.CategoryNo.fo_inner_t,
                ChaListDefine.CategoryNo.fo_inner_b,
                ChaListDefine.CategoryNo.fo_gloves,
                ChaListDefine.CategoryNo.fo_panst,
                ChaListDefine.CategoryNo.fo_socks,
                ChaListDefine.CategoryNo.fo_shoes
            };

        private class NameValue
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public NameValue(string Name, int Value)
            {
                this.Name = Name;
                this.Value = Value;
            }
        }

        private void RandomChara()
        {

            IEnumerator MyCoroutine()
            {

                // 找出随机的人物
                var charas = Utils.GetTreeCharaInfoDic().SelectMany(x => x.Value).Where(x => x.IsSelected).ToList();
                var count = charas.Count();
                if (count == 0)
                {
                    yield break;
                }
                if (Utils.GetCharaCards().Count == 0)
                {
                    yield break;
                }


                var selectedCards = Utils.GetCharaCards().OrderBy(p => Guid.NewGuid()).Take(count);
                var queueSelectedCards = new Queue<string>(selectedCards);

                foreach (var chara in charas)
                {

                    var obj = Studio.Studio.GetCtrlInfo(chara.Index) as OCIChar;
                    if (obj != null)
                    {

                        if (queueSelectedCards.Count > 0)
                        {
                            var card = queueSelectedCards.Dequeue();
                            obj.ChangeChara(card);
                            //LoadAll(card, obj);
                            // 更新衣服状态
                            if (obj.sex == 1)
                            {
                                var control = obj.GetChaControl();
                                var index = 0;
                                foreach (var state in Model.Main.ClothStatus)
                                {
                                    control.SetClothesState(index++, state, true);
                                }
                            }
                            yield return null;
                        }
                        else
                        {
                            StudioCharaShufflePlugin.Logger.LogWarning($"人物卡不够");
                        }
                        //StudioCharaShufflePlugin.Logger.LogWarning($"刷新");
                        if (queueSelectedCards.Count == 0)
                        {
                            //StudioCharaShufflePlugin.Logger.LogWarning($"刷新2");
                            Utils.BuildTreeCharaInfoList();
                        }

                    }


                }

                yield return null;

            }
            StartCoroutine(MyCoroutine());

        }
        private void RandomCharaWithoutCloth()
        {
            IEnumerator MyCoroutine()
            {
                // 找出随机的人物
                var charas = Utils.GetTreeCharaInfoDic().SelectMany(x => x.Value).Where(x => x.IsSelected).ToList();
                var count = charas.Count();
                if (count == 0)
                {
                    yield break;
                }
                if (Utils.GetCharaCards().Count == 0)
                {
                    yield break;
                }
                var selectedCards = Utils.GetCharaCards().OrderBy(p => Guid.NewGuid()).Take(count);
                var queueSelectedCards = new Queue<string>(selectedCards);

                foreach (var chara in charas)
                {

                    var obj = Studio.Studio.GetCtrlInfo(chara.Index) as OCIChar;
                    if (obj != null)
                    {

                        if (queueSelectedCards.Count > 0)
                        {
                            //obj.charInfo;
                            //obj.GetChaControl;

                            var card = queueSelectedCards.Dequeue();
                            LoadAnatomy(card, obj);
                            yield return null;
                        }
                        else
                        {
                            StudioCharaShufflePlugin.Logger.LogWarning($"人物卡不够");
                        }
                        if (queueSelectedCards.Count == 0)
                        {
                            Utils.BuildTreeCharaInfoList();
                        }

                    }

                }

                yield return null;

            }
            StartCoroutine(MyCoroutine());


        }
        private void RandomCloth()
        {

            IEnumerator MyCoroutine()
            {
                // 找出随机的人物
                var charas = Utils.GetTreeCharaInfoDic().SelectMany(x => x.Value).Where(x => x.IsSelected).ToList();
                var count = charas.Count();
                if (count == 0)
                {
                    yield break;
                }
                if (Utils.GetCoordCards().Count == 0)
                {
                    yield break;
                }

                if (!Model.Main.CoordIsOne)
                {
                    // 判断是否允许重复
                    if (Model.Main.CoordIsRepeat)
                    {
                        var random = new System.Random();
                        // 随机取n个
                        var cards = Utils.GetCoordCards();

                        var cardsNew = new List<string>();
                        foreach (var i in Enumerable.Range(0, count))
                        {
                            cardsNew.Add(cards[random.Next(cards.Count)]);
                        }
                        var cardsIndex = 0;
                        foreach (var chara in charas)
                        {
                            var obj = Studio.Studio.GetCtrlInfo(chara.Index) as OCIChar;
                            if (obj != null)
                            {
                                //LoadOutfit(card, obj);
                                obj.LoadClothesFile(cardsNew[cardsIndex++]);
                                // 更新衣服状态
                                if (obj.sex == 1)
                                {
                                    var control = obj.GetChaControl();
                                    var index = 0;
                                    foreach (var state in Model.Main.ClothStatus)
                                    {
                                        control.SetClothesState(index++, state, true);
                                    }
                                }
                                yield return null;
                            }
                        }
                    }
                    else
                    {
                        var selectedCards = Utils.GetCoordCards().OrderBy(p => Guid.NewGuid()).Take(count);
                        var queueSelectedCards = new Queue<string>(selectedCards);
                        foreach (var chara in charas)
                        {

                            var obj = Studio.Studio.GetCtrlInfo(chara.Index) as OCIChar;
                            if (obj != null)
                            {

                                if (queueSelectedCards.Count > 0)
                                {
                                    var card = queueSelectedCards.Dequeue();
                                    //LoadOutfit(card, obj);
                                    obj.LoadClothesFile(card);
                                    // 更新衣服状态
                                    if (obj.sex == 1)
                                    {
                                        var control = obj.GetChaControl();
                                        var index = 0;
                                        foreach (var state in Model.Main.ClothStatus)
                                        {
                                            control.SetClothesState(index++, state, true);
                                        }
                                    }
                                    yield return null;
                                }
                                else
                                {
                                    StudioCharaShufflePlugin.Logger.LogWarning($"服装卡不够");
                                }

                            }
                        }



                    }

                }
                else
                {
                    var selectedCard = Utils.GetCoordCards().OrderBy(p => Guid.NewGuid()).FirstOrDefault();
                    if (selectedCard != null)
                    {
                        foreach (var chara in charas)
                        {
                            var obj = Studio.Studio.GetCtrlInfo(chara.Index) as OCIChar;
                            if (obj != null)
                            {

                                obj.LoadClothesFile(selectedCard);
                                yield return null;

                            }

                        }
                    }
                }


                yield return null;

            }
            StartCoroutine(MyCoroutine());


        }

        private void RefreshCharaCards()
        {

            IEnumerator MyCoroutine()
            {
                Model.Main.CharaPathMessage = "人物卡读取中...";
                Model.Main.CharaPathIsLoading = true;

                if (!Directory.Exists(Model.Main.CharaPath))
                {
                    Model.Main.CharaPathMessage = "目录不存在!";
                    Model.Main.CharaCount = 0;
                    Model.Main.CharaPathIsOk = false;
                    Model.Main.CharaPathIsLoading = false;
                    yield break;
                }

                string rootPath = Path.GetPathRoot(Model.Main.CharaPath);
                // 检查所选目录是否为盘符根目录
                if (Model.Main.CharaPath == rootPath)
                {
                    Model.Main.CharaPathMessage = "不允许设置为盘符根目录!";
                    Model.Main.CharaCount = 0;
                    Model.Main.CharaPathIsOk = false;
                    Model.Main.CharaPathIsLoading = false;
                    yield break;
                }

                var count = 0;
                void DoBackgroundTask()
                {
                    count = Utils.BuildCharaCardPaths(Model.Main.CharaPath, Model.Main.CharaIsSub ? Model.Main.CharaSubDepth : 0);
                }

                Thread backgroundThread = new Thread(DoBackgroundTask);
                backgroundThread.Start();

                // 等待后台线程完成任务
                while (backgroundThread.IsAlive)
                {
                    yield return null;
                }
                Model.Main.CharaPathIsLoading = false;


                Model.Main.CharaCount = count;
                if (count == 0)
                {
                    Model.Main.CharaPathMessage = "未找到人物卡!";
                    Model.Main.CharaPathIsOk = false;
                }
                else
                {
                    Model.Main.CharaPathIsOk = true;
                }
                StudioCharaShufflePlugin.Logger.LogInfo($"识别人物卡数量：{count}");

            }

            StartCoroutine(MyCoroutine());
        }
        private void RefreshCoordCards()
        {

            IEnumerator MyCoroutine()
            {
                Model.Main.CoordPathIsLoading = true;
                Model.Main.CoordPathMessage = "服装卡读取中...";

                if (!Directory.Exists(Model.Main.CoordPath))
                {
                    Model.Main.CoordPathMessage = "目录不存在!";
                    Model.Main.CoordCount = 0;
                    Model.Main.CoordPathIsOk = false;
                    Model.Main.CoordPathIsLoading = false;
                    yield break;
                }

                string rootPath = Path.GetPathRoot(Model.Main.CoordPath);
                // 检查所选目录是否为盘符根目录
                if (Model.Main.CoordPath == rootPath)
                {
                    Model.Main.CoordPathMessage = "不允许设置为盘符根目录!";
                    Model.Main.CoordCount = 0;
                    Model.Main.CoordPathIsOk = false;
                    Model.Main.CoordPathIsLoading = false;
                    yield break;
                }

                var count = 0;
                void DoBackgroundTask()
                {
                    count = Utils.BuildCoordCardPaths(Model.Main.CoordPath, Model.Main.CoordIsSub ? Model.Main.CoordSubDepth : 0);
                }

                Thread backgroundThread = new Thread(DoBackgroundTask);
                backgroundThread.Start();

                // 等待后台线程完成任务
                while (backgroundThread.IsAlive)
                {
                    yield return null;
                }
                Model.Main.CoordPathIsLoading = false;
                Model.Main.CoordCount = count;
                if (count == 0)
                {
                    Model.Main.CoordPathMessage = "未找到服装卡!";
                    Model.Main.CoordPathIsOk = false;
                }
                else
                {
                    Model.Main.CoordPathIsOk = true;
                }
                StudioCharaShufflePlugin.Logger.LogInfo($"识别服装卡数量：{count}");

            }

            StartCoroutine(MyCoroutine());
        }

        private void SaveLastConfig()
        {
            var path = Path.Combine(Utils.GetDllPath(), "HS2_StudioCharaShuffle.json");
            var content = JsonUtility.ToJson(Model.Main);
            if (Directory.Exists(Path.GetDirectoryName(path)))
            {
                File.WriteAllText(path, content);
            }
        }

        private void LoadLastConfig()
        {

            var path = Path.Combine(Utils.GetDllPath(), "HS2_StudioCharaShuffle.json");
            if (File.Exists(path))
            {
                var content = File.ReadAllText(path);
                var obj = JsonUtility.FromJson<UIModel.MainModel>(content);
                if (obj != null)
                {
                    if (obj.CharaPathIsOk && !string.IsNullOrWhiteSpace(obj.CharaPath))
                    {
                        Model.Main.CharaPath = obj.CharaPath;
                        Model.Main.CharaCount = obj.CharaCount;
                    }

                    if (obj.CoordPathIsOk && !string.IsNullOrWhiteSpace(obj.CoordPath))
                    {
                        Model.Main.CoordPath = obj.CoordPath;
                        Model.Main.CoordCount = obj.CoordCount;
                    }
                    Model.Main.CharaIsSub = obj.CharaIsSub;
                    Model.Main.CoordIsSub = obj.CoordIsSub;
                    Model.Main.CharaSubDepth = obj.CharaSubDepth;
                    Model.Main.CoordSubDepth = obj.CoordSubDepth;
                    Model.Main.CoordIsOne = obj.CoordIsOne;
                    Model.Main.CoordIsRepeat = obj.CoordIsRepeat;

                    Model.Main.CharaFaceIsAuto = obj.CharaFaceIsAuto;
                    Model.Main.CharaIsAuto = obj.CharaIsAuto;
                    Model.Main.CoordIsAuto = obj.CoordIsAuto;
                    Model.Main.CharaAutoTime = obj.CharaAutoTime;
                    Model.Main.CharaFaceAutoTime = obj.CharaFaceAutoTime;
                    Model.Main.CoordAutoTime = obj.CoordAutoTime;
                }
            }

        }

        // Localize
        //public Dictionary<string, string> curLocalizationDict;
        private string LC(string org)
        {
            return org;
            //if (curLocalizationDict != null && curLocalizationDict.ContainsKey(org) && !string.IsNullOrWhiteSpace(curLocalizationDict[org]))
            //    return curLocalizationDict[org];
            //else
            //    return org;
        }

        // 替换外形
        public static void LoadAnatomy(string filePath, OCIChar chara)
        {
            CallWearCustom(filePath, anatomy, chara);
        }

        public static void LoadAll(string filePath, OCIChar chara)
        {
            CallWearCustom(filePath, anatomy2, chara);
        }

        public static void LoadOutfit(string filePath, OCIChar chara)
        {
            CallWearCustom(filePath, outfit, chara);
        }

        private static readonly bool[] outfit = new bool[5] { false, false, false, true, true };


        //private string greenText(string text)
        //{
        //    return colorText(text, "00ff00");
        //}
        //private string colorText(string text, string color = "ffffff")
        //{
        //    return "<color=#" + color + ">" + text + "</color>";
        //}


        private static Type _studioCharaListUtilType;
        public static Type StudioCharaListUtilType => _studioCharaListUtilType ?? (_studioCharaListUtilType = Type.GetType("WearCustom.StudioCharaListUtil, HS2WearCustom, Version=0.4.0.0, Culture=neutral, PublicKeyToken=null"));
        private static readonly bool[] anatomy = new bool[5] { true, true, true, false, false };
        private static readonly bool[] anatomy2 = new bool[5] { true, true, true, true, true };
        public static void CallWearCustom(string fileFullName, bool[] loadState, OCIChar chara)
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
                //MethodInfo method = StudioCharaListUtilType.GetMethod("ChangeChara", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);
                MethodInfo method = StudioCharaListUtilType.GetMethod("ChangeChara", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(OCIChar) }, null);


                object value = field6.GetValue(component);
                object obj = ((value is CharaFileSort) ? value : null);
                ((CharaFileSort)obj).cfiList.Clear();
                CharaFileInfo val2 = new CharaFileInfo(fileFullName, "Bobby");
                val2.node = new ListNode();
                val2.select = true;
                CharaFileInfo item = val2;
                ((CharaFileSort)obj).cfiList.Add(item);
                ((CharaFileSort)obj).select = 0;
                if (chara == null)
                {
                    method.Invoke(component, new object[0]);
                }
                else
                {
                    method.Invoke(component, new object[] { chara });
                }
            }


        }

    }
}
