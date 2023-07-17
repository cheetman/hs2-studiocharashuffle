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
    public class StudioCharaSwitchPlugin : BaseUnityPlugin
    {

        public const string GUID = "Cheatman.StudioCharaShuffle.HS2";
        public const string Name = "Studio Chara Shuffle";
        public const string Version = "0.1.0";

        public static StudioCharaSwitchPlugin Instance { get; private set; }

        internal static new ManualLogSource Logger;



        public static ConfigEntry<int> UIHeight { get; private set; }

        public static ConfigEntry<string> UIPath { get; private set; }
        public static ConfigEntry<KeyboardShortcut> UIHotKeyShow { get; private set; }


        public StudioCharaSwitchPlugin()
        {
            Instance = this;
            Logger = base.Logger;


            UIPath = Config.Bind<string>("路径", "人物路径", "默认值", "人物路径");

            UIHotKeyShow = Config.Bind("General", "快捷键", new KeyboardShortcut(KeyCode.R, KeyCode.LeftShift), "Toggles the main UI on and off.");

     

        }

        private void Awake()
        {


            var gameObject = new GameObject(Name);
            DontDestroyOnLoad(gameObject);
            StudioCharaSwitchMgr.Install(gameObject);



        }





    }
}
