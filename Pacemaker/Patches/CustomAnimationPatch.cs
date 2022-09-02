using System.Runtime.CompilerServices;
using HarmonyLib;
using RDLevelEditor;
using UnityEngine;

namespace Pacemaker.Patches
{
    public class CustomAnimationFields
    {
        public int lastClipFrame;
        public string lastClipName;
        public float lastPivotX;
        public float lastPivotY;
    }
    
    [HarmonyPatch]
    public static class CustomAnimationPatch
    {
        private static readonly ConditionalWeakTable<CustomAnimation, CustomAnimationFields> customAnimationFields = new ();
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LevelEvent_MakeSprite), "CreateSprite")]
        public static bool Prefix(LevelEvent_MakeSprite __instance)
        {
            var spriteContainer = __instance.game.rooms[__instance.room].spriteContainer;
            var gameObject = Object.Instantiate(__instance.gc.customSprite, spriteContainer);
            var component = gameObject.GetComponent<CustomAnimation>();
            component.data = LevelEvent_Base.level.customCharacterData[__instance.filename];
            component.animationCompleted = __instance.OnCustomAnimEnd;
            component.enabled = true;
            __instance.customAnimation = component;
            var component2 = gameObject.GetComponent<CustomSprite>();
            LevelEvent_Base.level.sprites.Add(__instance.spriteId, component2);
            gameObject.transform.LocalMoveXY(RDClass.Vfx.RDWidth / 2f, __instance.vfx.RDHeight / 2f);
            component2.renderer.enabled = __instance.visible;
            component2.customAnimation.renderer.enabled = __instance.visible;
            component2.renderer.sortingOrder = -__instance.depth;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEvent_SetVisible), "<Run>b__5_0")]
        public static void Postfix(LevelEvent_SetVisible __instance)
        {
            if (LevelEvent_Base.level.sprites.ContainsKey(__instance.target))
                LevelEvent_Base.level.sprites[__instance.target].customAnimation.renderer.enabled = __instance.visible;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomAnimation), "Awake")]
        public static void Postfix(CustomAnimation __instance)
        {
            customAnimationFields.Add(__instance, new CustomAnimationFields());
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CustomAnimation), "LateUpdate")]
        public static bool Prefix(CustomAnimation __instance)
        {
            if (__instance.renderMode == CustomAnimation.RenderMode.MeshRenderer && !__instance.renderer.enabled) return false;
            if (!__instance.data.mainTexture || __instance.data.mainTexture.width == 0 || __instance.data.mainTexture.height == 0) return false;

            var material = ((__instance.renderMode == CustomAnimation.RenderMode.MeshRenderer) ? __instance.renderer.material : __instance.rawImage.material);
            if (__instance.renderMode == CustomAnimation.RenderMode.RawImage) __instance.rawImage.texture = __instance.data.mainTexture;
            
            material.mainTexture = __instance.data.mainTexture;
            material.SetTexture(Shader.PropertyToID("_GlowTex"), __instance.data.glowTexture);
            material.SetTexture(Shader.PropertyToID("_OutlineTex"), __instance.data.outlineTexture);
            var mainTexture = material.mainTexture;
            
            if (!__instance.data.mainTexture) return false;
            
            if (Time.timeScale > 0f || __instance.ignoreTimescale) __instance.time += Time.unscaledDeltaTime;
            
            __instance.clipFrame = (int) (__instance.fps * __instance.time);

            if (__instance.currentClip.type == AnimationType.Once || __instance.currentClip.type == AnimationType.LoopOnBeat)
            {
	            if (__instance.clipFrame >= __instance.currentClip.frames.Length)
	            {
		            __instance.clipFrame = __instance.currentClip.frames.Length - 1;
                    __instance.animationCompleted?.Invoke(__instance, __instance.currentClip);
                }
            }
            else if (__instance.currentClip.type == AnimationType.Loop)
            {
	            var num = __instance.currentClip.frames.Length / __instance.fps;
	            if (__instance.time > num)
	            {
		            var num2 = __instance.currentClip.frames.Length - __instance.currentClip.loopStart;
		            __instance.clipFrame = (__instance.clipFrame - __instance.currentClip.frames.Length) % num2 + __instance.currentClip.loopStart;
	            }
            }

            if (__instance.clipFrame >= __instance.currentClip.frames.Length) return false;

            if (__instance.renderMode == CustomAnimation.RenderMode.MeshRenderer)
            {
                var mesh = __instance.filter.mesh;
                if (__instance.pivotX != customAnimationFields.GetOrCreateValue(__instance).lastPivotX || __instance.pivotY != customAnimationFields.GetOrCreateValue(__instance).lastPivotY)
                {
                    customAnimationFields.GetOrCreateValue(__instance).lastPivotX = __instance.pivotX;
                    customAnimationFields.GetOrCreateValue(__instance).lastPivotY = __instance.pivotY;
                    
                    var vector = new Vector3(__instance.pivotX, __instance.pivotY, 0f);
                    mesh.vertices = new[]
                    {
                        new Vector3(0f, 0f, 0f) - vector,
                        new Vector3(1f, 0f, 0f) - vector,
                        new Vector3(0f, 1f, 0f) - vector,
                        new Vector3(1f, 1f, 0f) - vector
                    };
                    var indices = new[] { 0, 3, 1, 0, 2, 3 };
                    mesh.SetIndices(indices, MeshTopology.Triangles, 0);
                    mesh.colors = new[]
                    {
                        Color.white,
                        Color.white,
                        Color.white,
                        Color.white
                    };
                }

                if (__instance.clipFrame != customAnimationFields.GetOrCreateValue(__instance).lastClipFrame || __instance.currentClip.name != customAnimationFields.GetOrCreateValue(__instance).lastClipName)
                {
                    customAnimationFields.GetOrCreateValue(__instance).lastClipFrame = __instance.clipFrame;
                    customAnimationFields.GetOrCreateValue(__instance).lastClipName = __instance.currentClip.name;
                    var sheetFrame = __instance.currentClip.frames[__instance.clipFrame];
                    var uVsForSheetFrame = __instance.data.GetUVsForSheetFrame(sheetFrame);
                    mesh.SetUVs(0, uVsForSheetFrame);
                    material.SetVector(Shader.PropertyToID("_UVBounds"), new Vector4(uVsForSheetFrame[0].x, uVsForSheetFrame[0].y, uVsForSheetFrame[3].x, uVsForSheetFrame[3].y));
                }

                __instance.transform.localScale = new Vector3(__instance.data.spriteSize.x * __instance.scaleX, __instance.data.spriteSize.y * __instance.scaleY, 1f);
            }
            else
            {
                var num3 = __instance.currentClip.portraitOffset.x / (float)mainTexture.width;
                var num4 = __instance.currentClip.portraitOffset.y / (float)mainTexture.height;
                var width = __instance.currentClip.portraitSize.x / (float)mainTexture.width;
                var height = __instance.currentClip.portraitSize.y / (float)mainTexture.height;
                if (__instance.clipFrame != customAnimationFields.GetOrCreateValue(__instance).lastClipFrame || __instance.currentClip.name != customAnimationFields.GetOrCreateValue(__instance).lastClipName)
                {
                    customAnimationFields.GetOrCreateValue(__instance).lastClipFrame = __instance.clipFrame;
                    customAnimationFields.GetOrCreateValue(__instance).lastClipName = __instance.currentClip.name;
                    var sheetFrame = __instance.currentClip.frames[__instance.clipFrame];
                    var uVsForSheetFrame = __instance.data.GetUVsForSheetFrame(sheetFrame);
                    __instance.rawImage.uvRect = new Rect(uVsForSheetFrame[0].x + num3, uVsForSheetFrame[0].y + num4, width, height);
                }
            }
            
            return false;
        }
    }
}