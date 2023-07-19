using AIChara;
using BepInEx;
using MessagePack;
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

        public static List<string> CharaCardPaths = new List<string>();
        public static List<string> CoordCardPaths = new List<string>();





        public static List<string> GetCharaCards()
        {
            return CharaCardPaths;
        }
        public static List<string> GetCoordCards()
        {
            return CoordCardPaths;
        }

        public static int BuildCharaCardPaths(string path, int maxDepth = 1)
        {
            CharaCardPaths.Clear();
            TraverseCharaDirectory(path, 0, maxDepth);
            return CharaCardPaths.Count;
        }

        public static int BuildCoordCardPaths(string path, int maxDepth = 1)
        {
            CoordCardPaths.Clear();
            TraverseCoordDirectory(path, 0, maxDepth);
            return CoordCardPaths.Count;
        }

        static void TraverseCharaDirectory(string directory, int depth, int maxDepth)
        {
            try
            {
                // 设置最大深度为 3
                //int maxDepth = maxDepth;

                // 获取当前目录下的所有 .png 文件
                string[] pngFiles = Directory.GetFiles(directory, "*.png");

                // 处理当前目录下的 .png 文件
                foreach (string file in pngFiles)
                {
                    var isPng = GetPng(file);
                    if (isPng)
                    {
                        CharaCardPaths.Add(file);
                        StudioCharaSwitchPlugin.Logger.LogInfo("人物卡: " + file);
                    }
                }

                // 获取当前目录下的所有子目录
                string[] subDirectories = Directory.GetDirectories(directory);

                // 递归遍历子目录，但限制深度
                if (depth < maxDepth)
                {
                    foreach (string subDirectory in subDirectories)
                    {
                        TraverseCharaDirectory(subDirectory, depth + 1, maxDepth);
                    }
                }
            }
            catch (Exception e)
            {
                StudioCharaSwitchPlugin.Logger.LogWarning("发生错误: " + e.Message);
            }
        }


        static void TraverseCoordDirectory(string directory, int depth, int maxDepth)
        {
            try
            {

                // 获取当前目录下的所有 .png 文件
                string[] pngFiles = Directory.GetFiles(directory, "*.png");

                // 处理当前目录下的 .png 文件
                foreach (string file in pngFiles)
                {
                    var isPng = GetPngCoord(file);
                    if (isPng)
                    {
                        CoordCardPaths.Add(file);
                        StudioCharaSwitchPlugin.Logger.LogInfo("服装卡: " + file);
                    }
                }

                // 获取当前目录下的所有子目录
                string[] subDirectories = Directory.GetDirectories(directory);

                // 递归遍历子目录，但限制深度
                if (depth < maxDepth)
                {
                    foreach (string subDirectory in subDirectories)
                    {
                        TraverseCoordDirectory(subDirectory, depth + 1, maxDepth);
                    }
                }
            }
            catch (Exception e)
            {
                StudioCharaSwitchPlugin.Logger.LogWarning("发生错误: " + e.Message);
            }
        }






        public static Dictionary<string, List<TreeCharaInfo>> GetTreeCharaInfoDic()
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

        //static public Version loadVersion = new Version(ChaFileDefine.ChaFileCoordinateVersion.ToString());
        static public bool GetPngCoord(string filepath, bool noLoadPNG = true)
        {
            byte[] pngData;

            using (var st = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(st))
                {
                    int num = (int)GetPngSize(br);
                    if (num != 0)
                    {
                        //item.GetPngData(br, num);

                        if (noLoadPNG)
                        {
                            br.BaseStream.Seek(num, SeekOrigin.Current);
                        }
                        else
                        {
                            pngData = br.ReadBytes((int)num);
                        }
                        if (br.BaseStream.Length - br.BaseStream.Position == 0L)
                        {
                            //lastLoadErrorCode = -5;
                            return false;
                        }
                    }
                    try
                    {
                        if (br.ReadInt32() > 100)
                        {
                            return false;
                        }
                        if (br.ReadString() != "【AIS_Clothes】")
                        {
                            return false;
                        }
                        if (new Version(br.ReadString()) > ChaFileDefine.ChaFileClothesVersion)
                        {
                            return false;
                        }
                        var language = br.ReadInt32();
                        var coordinateName = br.ReadString();
                        int count = br.ReadInt32();
                        byte[] data = br.ReadBytes(count);
                        if (LoadBytes(data))
                        {
                            return true;
                        }
                        return false;

                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }



        static public bool GetPng(string filepath, bool noLoadPNG = true)
        {
            byte[] pngData;

            using (var st = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(st))
                {
                    int num = (int)GetPngSize(br);
                    if (num != 0)
                    {
                        //item.GetPngData(br, num);

                        if (noLoadPNG)
                        {
                            br.BaseStream.Seek(num, SeekOrigin.Current);
                        }
                        else
                        {
                            pngData = br.ReadBytes((int)num);
                        }
                        if (br.BaseStream.Length - br.BaseStream.Position == 0L)
                        {
                            //lastLoadErrorCode = -5;
                            return false;
                        }
                    }
                    try
                    {
                        if (br.ReadInt32() > 100)
                        {
                            return false;
                        }
                        if (br.ReadString() != "【AIS_Chara】")
                        {
                            return false;
                        }
                        if (new Version(br.ReadString()) > ChaFileDefine.ChaFileVersion)
                        {
                            return false;
                        }
                        var language = br.ReadInt32();
                        var userID = br.ReadString();
                        var dataID = br.ReadString();
                        int count = br.ReadInt32();
                        BlockHeader blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(br.ReadBytes(count));
                        br.ReadInt64();
                        long position = br.BaseStream.Position;
                        BlockHeader.Info info = blockHeader.SearchInfo(ChaFileParameter.BlockName);
                        if (new Version(info.version) > ChaFileDefine.ChaFileParameterVersion)
                        {
                            //item.EvoVersion = true;
                            return true;
                        }
                        br.BaseStream.Seek(position + info.pos, SeekOrigin.Begin);
                        ChaFileParameter chaFileParameter = MessagePackSerializer.Deserialize<ChaFileParameter>(br.ReadBytes((int)info.size));
                        //item.Sex = chaFileParameter.sex;
                        //item.CharaName = chaFileParameter.fullname;
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }


        public static int Big2LittleInt32(int big)
        {
            return ((big >> 24) & 0xFF) | ((big >> 8) & 0xFF00) | ((big << 8) & 0xFF0000) | (big << 24);
        }


        static public bool LoadBytes(byte[] data, Version ver = null)
        {
            ChaFileClothes clothes;
            ChaFileAccessory accessory;
            using (MemoryStream input = new MemoryStream(data))
            {
                using (BinaryReader binaryReader = new BinaryReader(input))
                {
                    try
                    {
                        int count = binaryReader.ReadInt32();
                        byte[] bytes = binaryReader.ReadBytes(count);
                        clothes = MessagePackSerializer.Deserialize<ChaFileClothes>(bytes);
                        count = binaryReader.ReadInt32();
                        bytes = binaryReader.ReadBytes(count);
                        accessory = MessagePackSerializer.Deserialize<ChaFileAccessory>(bytes);
                    }
                    catch (EndOfStreamException)
                    {
                        return false;
                    }
                    clothes.ComplementWithVersion();
                    accessory.ComplementWithVersion();
                    return true;
                }

            }
        }


        public static long GetPngSize(BinaryReader br)
        {
            long result = 0L;
            Stream baseStream = br.BaseStream;
            long position = baseStream.Position;
            try
            {
                if (br.ReadInt64() == 727905341920923785L)
                {
                    int num2;
                    do
                    {
                        int num = Big2LittleInt32(br.ReadInt32());
                        num2 = br.ReadInt32();
                        baseStream.Position += num + 4;
                    }
                    while (num2 != 1145980233);
                    return baseStream.Position - position;
                }
                return result;
            }
            finally
            {
                baseStream.Position = position;
            }
        }


    }
}
