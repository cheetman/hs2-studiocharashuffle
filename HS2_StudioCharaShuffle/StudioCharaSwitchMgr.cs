using AIChara;
using KKAPI.Studio;
using Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HS2_StudioCharaShuffle
{
   public  class StudioCharaShuffleMgr : MonoBehaviour
    {

        public UI gui;
        public static StudioCharaShuffleMgr Instance { get; private set; }

        public Dictionary<OCIChar, CharaController> charaEditorCtrlDict = new Dictionary<OCIChar, CharaController>();

        public static StudioCharaShuffleMgr Install(GameObject container)
        {
            if (Instance == null)
            {
                Instance = container.AddComponent<StudioCharaShuffleMgr>();
            }
            return Instance;
        }

        private void Awake()
        {
        }

        private void Start()
        {
            StartCoroutine(LoadingCo());
        }


        private IEnumerator LoadingCo()
        {
            yield return new WaitUntil(() => StudioAPI.StudioLoaded);
            // Wait until fully loaded
            yield return null;

            // start ui
            gui = new GameObject("GUI").AddComponent<UI>();
            gui.transform.parent = base.transform;
            gui.IsVisible = false;
            //Console.WriteLine("StudioCharaEditor CharaEditorMgr Started.");
            StudioCharaShufflePlugin.Logger.LogInfo("StudioCharaSwitchMgr Started.");
            // check extra plugins
        }


        //public CharaController GetEditorController(OCIChar ociTarget)
        //{
        //    if (ociTarget == null)
        //    {
        //        return null;
        //    }
        //    if (!charaEditorCtrlDict.ContainsKey(ociTarget))
        //    {
        //        charaEditorCtrlDict[ociTarget] = new CharaController(ociTarget);
        //        charaEditorCtrlDict[ociTarget].Initialize();
        //    }
        //    return charaEditorCtrlDict[ociTarget];
        //}

        //public CharaController GetEditorController(ChaControl chaCtrl)
        //{
        //    foreach (OCIChar ociChar in charaEditorCtrlDict.Keys)
        //    {
        //        if (ociChar.charInfo == chaCtrl)
        //        {
        //            return charaEditorCtrlDict[ociChar];
        //        }
        //    }
        //    return null;
        //}

    }
}
