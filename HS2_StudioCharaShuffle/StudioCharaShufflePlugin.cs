using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HS2_StudioCharaShuffle
{



    [BepInPlugin(GUID, Name, Version)]
    [HelpURL("https://github.com/cheetman/hs2-studiocharashuffle")]
    public class StudioCharaShufflePlugin : BaseUnityPlugin
    {
        // 测试用
        //static void Main(string[] args)
        //{

        //}



        public const string GUID = "Cheatman.StudioCharaShuffle.HS2";
        public const string Name = "Studio Chara Shuffle";
        public const string Version = "0.1.0";

        public static StudioCharaShufflePlugin Instance { get; private set; }

        internal static new ManualLogSource Logger;




        //public static ConfigEntry<string> UIPath { get; private set; }
        public static ConfigEntry<KeyboardShortcut> UIHotKeyShow { get; private set; }
        public static ConfigEntry<KeyboardShortcut> UIHotKey1 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> UIHotKey2 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> UIHotKey3 { get; private set; }


        public StudioCharaShufflePlugin()
        {
            Instance = this;
            Logger = base.Logger;



            UIHotKeyShow = Config.Bind("General", "快捷键", new KeyboardShortcut(KeyCode.S, KeyCode.LeftShift), "Toggles the main UI on and off.");
            UIHotKey1 = Config.Bind("General", "随机人物快捷键", new KeyboardShortcut(KeyCode.KeypadDivide), "");
            UIHotKey2 = Config.Bind("General", "随机外观快捷键", new KeyboardShortcut(KeyCode.KeypadMultiply), "");
            UIHotKey3 = Config.Bind("General", "随机服装快捷键", new KeyboardShortcut(KeyCode.KeypadMinus), "");

        }

        private void Awake()
        {
            var gameObject = new GameObject(Name);
            DontDestroyOnLoad(gameObject);
            StudioCharaShuffleMgr.Install(gameObject);
        }

    }
}
