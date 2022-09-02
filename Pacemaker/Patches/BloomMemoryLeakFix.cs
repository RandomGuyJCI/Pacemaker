using HarmonyLib;
using UnityEngine;

namespace Pacemaker.Patches
{
    [HarmonyPatch]
    public static class BloomMemoryLeakFix
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(VideoBloom), "BloomBlit")]
        public static bool Prefix(RenderTexture source, RenderTexture blur1, RenderTexture blur2, float radius1, float radius2, VideoBloom __instance)
        {
            float num = __instance.videoBlurGetMaxScaleFor(radius1);
			int num2 = (int)num;
			float value = num - num2;
			num = __instance.videoBlurGetMaxScaleFor(radius2);
			int num3 = (int)num;
			float num4 = 1f;
			int width = source.width;
			int height = source.height;
			float num5 = ((num2 != 0) ? 1f : (-1f));
			if (radius1 == 0f)
			{
				Graphics.Blit(source, blur1);
				return false;
			}
			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBHalf);
			renderTexture.filterMode = FilterMode.Bilinear;
			renderTexture.wrapMode = TextureWrapMode.Clamp;
			Graphics.Blit(source, renderTexture);
			int i;
			Vector4 value2;
			RenderTexture temporary = null;
			for (i = 0; i < num2; i++)
			{
				num5 = ((i % 2 != 0) ? (-1f) : 1f);
				value2 = new Vector4(num5 * num4 * 1.33333337f, num4 * (1f / 3f), num5 * num4 * (1f / 3f), (0f - num4) * 1.33333337f);
				__instance.videoBloomMaterial.SetVector(Shader.PropertyToID("_Param0"), value2);
				temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBHalf);
				temporary.filterMode = FilterMode.Bilinear;
				temporary.wrapMode = TextureWrapMode.Clamp;
				__instance.videoBloomMaterial.SetTexture(Shader.PropertyToID("_MainTex"), renderTexture);
				Graphics.Blit(renderTexture, temporary, __instance.videoBloomMaterial, 0);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				num4 *= 1.41421354f;
			}
			RenderTexture.ReleaseTemporary(temporary);
			value2 = new Vector4((0f - num5) * num4 * 1.33333337f, num4 * (1f / 3f), (0f - num5) * num4 * (1f / 3f), (0f - num4) * 1.33333337f);
			__instance.videoBloomMaterial.SetVector(Shader.PropertyToID("_Param0"), value2);
			__instance.videoBloomMaterial.SetFloat(Shader.PropertyToID("_Param2"), value);
			__instance.videoBloomMaterial.SetTexture(Shader.PropertyToID("_MainTex"), renderTexture);
			Graphics.Blit(renderTexture, blur1, __instance.videoBloomMaterial, 1);
			if (blur2 != null)
			{
				RenderTexture temporary2 = null;
				for (; i < num3; i++)
				{
					num5 = ((i % 2 != 0) ? (-1f) : 1f);
					value2 = new Vector4(num5 * num4 * 1.33333337f, num4 * (1f / 3f), num5 * num4 * (1f / 3f), (0f - num4) * 1.33333337f);
					__instance.videoBloomMaterial.SetVector(Shader.PropertyToID("_Param0"), value2);
					temporary2 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBHalf);
					temporary2.filterMode = FilterMode.Bilinear;
					temporary2.wrapMode = TextureWrapMode.Clamp;
					__instance.videoBloomMaterial.SetTexture(Shader.PropertyToID("_MainTex"), renderTexture);
					Graphics.Blit(renderTexture, temporary2, __instance.videoBloomMaterial, 0);
					RenderTexture.ReleaseTemporary(renderTexture);
					renderTexture = temporary2;
					num4 *= 1.41421354f;
				}
				RenderTexture.ReleaseTemporary(temporary2);
				value2 = new Vector4((0f - num5) * num4 * 1.33333337f, num4 * (1f / 3f), (0f - num5) * num4 * (1f / 3f), (0f - num4) * 1.33333337f);
				__instance.videoBloomMaterial.SetVector(Shader.PropertyToID("_Param0"), value2);
				__instance.videoBloomMaterial.SetFloat(Shader.PropertyToID("_Param2"), num - num3);
				__instance.videoBloomMaterial.SetTexture(Shader.PropertyToID("_MainTex"), renderTexture);
				Graphics.Blit(renderTexture, blur2, __instance.videoBloomMaterial, 1);
			}
			RenderTexture.ReleaseTemporary(renderTexture);
            return false;
        }
    }
}