using BepInEx;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Studio.TreeNodeObject;

namespace HS2_StudioCharaShuffle
{
    public class Utils
    {
        public class TreeCharaInfo
        {
            public int Sex { get; set; }
            public int Index { get; set; }
            public string Name { get; set; }
            public string TreePath { get; set; }
            public TreeState TreeState { get; set; }
            public bool IsVisible { get; set; }
            public bool IsSelected { get; set; } = true;
        }

        public static List<TreeCharaInfo> TreeCharaInfos = new List<TreeCharaInfo>();
        public static Dictionary<string, List<TreeCharaInfo>> TreeCharaInfosDic = new Dictionary<string, List<TreeCharaInfo>>();



        //public static List<TreeCharaInfo> GetTreeCharaInfoList()
        //{
        //    return TreeCharaInfos;
        //}
        public static Dictionary<string, List<TreeCharaInfo>>  GetTreeCharaInfoDic()
        {
            return TreeCharaInfosDic;
        }


        public static void BuildTreeCharaInfoList()
        {
            var list = Studio.Studio.Instance.dicInfo;
            //var tmpTreeCharaInfos = new List<TreeCharaInfo>();
            var tmpTreeCharaInfosDic = new Dictionary<string, List<TreeCharaInfo>>();
            foreach (var item in list)
            {
                var treeInfo = item.Key;
                var objInfo = item.Value;
                // 判断人物
                if (objInfo.kind == 0)
                {
                    var charaInfo = objInfo as OCIChar;
                    if (charaInfo != null)
                    {
                        var treePath = @"\";
                        var parent = treeInfo.parent;
                        while (parent != null)
                        {
                            treePath = $@"\{parent.textName}{treePath}";
                            parent = parent.parent;
                        }

                        if (tmpTreeCharaInfosDic.ContainsKey(treePath))
                        {
                            tmpTreeCharaInfosDic[treePath].Add(new TreeCharaInfo
                            {
                                Name = treeInfo.textName,
                                Sex = charaInfo.sex,
                                Index = charaInfo.objectInfo.dicKey,
                                TreeState = treeInfo.treeState,
                                IsVisible = treeInfo.visible,
                                TreePath = treePath
                            });
                        }
                        else
                        {

                            tmpTreeCharaInfosDic[treePath] = new List<TreeCharaInfo>{ new TreeCharaInfo
                                {
                                    Name = treeInfo.textName,
                                    Sex = charaInfo.sex,
                                    Index = charaInfo.objectInfo.dicKey,
                                    TreeState = treeInfo.treeState,
                                    IsVisible = treeInfo.visible,
                                    TreePath = treePath
                                }
                            };
                        }


                    }
                }
            }

            TreeCharaInfosDic = tmpTreeCharaInfosDic;

        }


        static public string GetCharaPath(byte sex)
        {
            //string exportPath = StudioCharaEditor.CharaExportPath.Value;
            string defPath = Path.Combine(Paths.GameRootPath, sex == 0 ? "UserData\\chara\\male" : "UserData\\chara\\female");
            //if (exportPath.Contains(StudioCharaEditor.DefaultPathMacro))
            //{
            //    exportPath = exportPath.Replace(StudioCharaEditor.DefaultPathMacro, defPath);
            //}
            return defPath;
        }

    
    }
}
