using HarmonyLib;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pacemaker.Patches
{
    [HarmonyPatch]
    public class EditorFullscreenPatch
    {
        private static bool shouldUpdateTimeline;
        
        [HarmonyPatch(typeof(scnEditor), "FullscreenButtonClick")]
        [HarmonyPrefix]
        public static bool Prefix(bool ___fullscreen)
        {
            if (___fullscreen) shouldUpdateTimeline = true;
            return true;
        }

        [HarmonyPatch(typeof(scnEditor), "FullscreenButtonClick")]
        [HarmonyPostfix]
        public static void Postfix(bool ___fullscreen)
        {
            if (___fullscreen) shouldUpdateTimeline = false;
        }
        
        [HarmonyPatch(typeof(scnEditor), "Awake")]
        [HarmonyPostfix]
        public static void Postfix()
        {
            shouldUpdateTimeline = true;
        }

        [HarmonyPatch(typeof(Timeline), "Update")]
        [HarmonyPatch(typeof(Timeline), "LateUpdate")]
        [HarmonyPrefix]
        public static bool Prefix(ScrollRect ___scrollRect, tlVerticalScrollRect ___verticalScrollRect)
        {
            ___scrollRect.enabled = shouldUpdateTimeline;
            ___verticalScrollRect.enabled = shouldUpdateTimeline;
            return shouldUpdateTimeline;
        }

        [HarmonyPatch(typeof(scnEditor), "LateUpdate")]
        [HarmonyPostfix]
        public static void Postfix(EventSystem ___eventSystem, GameObject ___cameraButton)
        {
            ___eventSystem.enabled = shouldUpdateTimeline;
            if (!shouldUpdateTimeline) ___cameraButton.SetActive(value: false);
        }
    }
}