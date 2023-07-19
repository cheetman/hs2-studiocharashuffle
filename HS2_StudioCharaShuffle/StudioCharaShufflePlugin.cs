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
    public class StudioCharaShufflePlugin : BaseUnityPlugin
    {

        //static void Main(string[] args)
        //{
        //    Utils.GetPng();
        //}



        public const string GUID = "Cheatman.StudioCharaShuffle.HS2";
        public const string Name = "Studio Chara Shuffle";
        public const string Version = "0.1.0";

        public static StudioCharaShufflePlugin Instance { get; private set; }

        internal static new ManualLogSource Logger;



        public static ConfigEntry<int> UIHeight { get; private set; }

        public static ConfigEntry<string> UIPath { get; private set; }
        public static ConfigEntry<KeyboardShortcut> UIHotKeyShow { get; private set; }
        public static ConfigEntry<KeyboardShortcut> UIHotKey1 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> UIHotKey2 { get; private set; }
        public static ConfigEntry<KeyboardShortcut> UIHotKey3 { get; private set; }


        public StudioCharaShufflePlugin()
        {
            Instance = this;
            Logger = base.Logger;


            UIPath = Config.Bind<string>("路径", "人物路径", "默认值", "人物路径");

            UIHotKeyShow = Config.Bind("General", "快捷键", new KeyboardShortcut(KeyCode.S, KeyCode.LeftShift), "Toggles the main UI on and off.");
            //UIHotKey1 = Config.Bind("General", "随机人物快捷键", new KeyboardShortcut(KeyCode.KeypadDivide, KeyCode.LeftShift), "Toggles the main UI on and off.");
            //UIHotKey2 = Config.Bind("General", "随机外观快捷键", new KeyboardShortcut(KeyCode.KeypadMultiply, KeyCode.LeftShift), "Toggles the main UI on and off.");
            //UIHotKey3 = Config.Bind("General", "随机服装快捷键", new KeyboardShortcut(KeyCode.KeypadMinus, KeyCode.LeftShift), "Toggles the main UI on and off.");
            UIHotKey1 = Config.Bind("General", "随机人物快捷键", new KeyboardShortcut(KeyCode.KeypadDivide), "Toggles the main UI on and off.");
            UIHotKey2 = Config.Bind("General", "随机外观快捷键", new KeyboardShortcut(KeyCode.KeypadMultiply), "Toggles the main UI on and off.");
            UIHotKey3 = Config.Bind("General", "随机服装快捷键", new KeyboardShortcut(KeyCode.KeypadMinus), "Toggles the main UI on and off.");



        }

        private void Awake()
        {


            var gameObject = new GameObject(Name);
            DontDestroyOnLoad(gameObject);
            StudioCharaShuffleMgr.Install(gameObject);



        }





    }
}
