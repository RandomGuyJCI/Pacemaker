using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace Pacemaker.Patches
{
    [HarmonyPatch]
    public static class BloomMemoryLeakFix
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(VideoBloom), "BloomBlit")]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(RenderTexture), "ReleaseTemporary", new []{ typeof(RenderTexture) })))
                .Advance(3)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 11),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RenderTexture), "ReleaseTemporary", new []{ typeof(RenderTexture) })))
                .MatchForward(false,
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(RenderTexture), "ReleaseTemporary", new []{ typeof(RenderTexture) })))
                .Advance(3)
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldloc_S, 12),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RenderTexture), "ReleaseTemporary", new []{ typeof(RenderTexture) })))
                .InstructionEnumeration();
        }
    }
}