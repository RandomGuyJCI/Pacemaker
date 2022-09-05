using HarmonyLib;
using RDLevelEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pacemaker.Patches
{
    [HarmonyPatch]
    public static class SmarterLevelEventCulling
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEventControl_Base), "Awake")]
        public static void Postfix(GameObject ___go)
        {
            ___go.SetActive(value: false);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Timeline), "CullMaskedObjects")]
        public static bool Prefix(Timeline __instance)
        {
            float scrollViewLeftPos = -__instance.scrollviewContent.anchoredPosition.x;
            float scrollViewRightPos = scrollViewLeftPos + __instance.scrollview.rect.width;
            float scrollViewBottomPos = __instance.scrollViewVertContent.anchoredPosition.y;
            float scrollViewTopPos = scrollViewBottomPos + __instance.scrollViewVert.rect.height;

            foreach (RectTransform cachedBarNumber in __instance.cachedBarNumbers)
            {
                float x = cachedBarNumber.localPosition.x;
                bool active = x >= scrollViewLeftPos - 10f && x <= scrollViewRightPos + 10f;
                cachedBarNumber.gameObject.SetActive(active);
            }

            if (__instance.editor.currentTab == Tab.None || __instance.barColumns == null) return false;

            List<LevelEventControl_Base> eventControls = null;

            switch (__instance.editor.currentTab)
            {
                case Tab.Song:
                    eventControls = __instance.editor.eventControls_song;
                    break;
                case Tab.Rows:
                    if (!__instance.editor.eventControls_rows.Any()) break;
                    var rowsRoom = __instance.editor.selectedRowsTabPageIndex;
                    var lastRowOfRoom = __instance.editor.LastRowOfRoom(rowsRoom);
                    var firstRowOfRoom = rowsRoom == 0 ? 0 : __instance.editor.LastRowOfRoom(rowsRoom - 1) + 1;
                    eventControls = __instance.editor.eventControls_rows
                        .Where((_, index) => index <= lastRowOfRoom && index >= firstRowOfRoom)
                        .SelectMany(list => list)
                        .ToList();
                    break;
                case Tab.Actions:
                    eventControls = __instance.editor.eventControls_actions;
                    break;
                case Tab.Sprites:
                    if (!__instance.editor.eventControls_sprites.Any()) break;
                    var spritesRoom = __instance.editor.selectedSpritesTabPageIndex;
                    var lastSpriteOfRoom = __instance.editor.LastSpriteOfRoom(spritesRoom);
                    var firstSpriteOfRoom = spritesRoom == 0 ? 0 : __instance.editor.LastSpriteOfRoom(spritesRoom - 1) + 1;
                    eventControls = __instance.editor.eventControls_sprites
                        .Where((_, index) => index <= lastSpriteOfRoom && index >= firstSpriteOfRoom)
                        .SelectMany(list => list)
                        .ToList();
                    break;
                case Tab.Rooms:
                    eventControls = __instance.editor.eventControls_rooms;
                    break;
            }
            
            if (eventControls == null) return false;

            foreach (var levelEventControl_Base in eventControls)
            {
                LevelEvent_Base levelEvent = levelEventControl_Base.levelEvent;

                Vector2 anchoredPosition = levelEventControl_Base.rt.anchoredPosition;
                float leftPos = anchoredPosition.x;
                float rightPos = levelEventControl_Base.rightPosition;
                float topPos = scrollViewTopPos + anchoredPosition.y;
                float bottomPos = scrollViewTopPos + levelEventControl_Base.bottomPosition;

                if (levelEvent is LevelEvent_AddOneshotBeat addOneshotEvent)
                    rightPos += addOneshotEvent.skipshot ? ((LevelEventControl_AddClassicBeat) levelEventControl_Base).skipshotBorder.rectTransform.sizeDelta.x : 0;
                else if (levelEvent is LevelEvent_AddClassicBeat addClassicEvent)
                    rightPos += addClassicEvent.hold > 0 ? ((LevelEventControl_AddClassicBeat) levelEventControl_Base).heldbeatBorder.rectTransform.sizeDelta.x : 0;
                
                bool flag = rightPos + 5 >= scrollViewLeftPos &&
                            leftPos - 5 <= scrollViewRightPos &&
                            topPos + 5 >= scrollViewBottomPos &&
                            bottomPos - 5 <= scrollViewTopPos &&
                            levelEventControl_Base.visible    &&
                            __instance.editor.ActionTypeIsVisible(levelEvent.type);
                
                GameObject go = levelEventControl_Base.go;

                if (!flag && go.activeInHierarchy)
                {
                    go.SetActive(value: false);
                }
                else if (flag && !go.activeInHierarchy)
                {
                    go.SetActive(value: true);
                }
            }

            return false;
        }
    }
}