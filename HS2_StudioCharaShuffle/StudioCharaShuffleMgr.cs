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
            StudioCharaShufflePlugin.Logger.LogInfo("StudioCharaSwitchMgr Started.");
        }
    }
}
