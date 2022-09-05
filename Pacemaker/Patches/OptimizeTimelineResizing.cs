using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RDLevelEditor;

namespace Pacemaker.Patches
{
    [HarmonyPatch]
    public class OptimizeTimelineResizing
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Timeline), "SetExpanded")]
        [HarmonyPatch(typeof(Timeline), "ZoomVert")]
        [HarmonyPatch(typeof(Timeline), "ZoomIn")]
        [HarmonyPatch(typeof(Timeline), "ZoomOut")]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, 
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Timeline), "UpdateUI", new []{ typeof(bool) })))
                .Advance(-1)
                .SetOpcodeAndAdvance(OpCodes.Ldc_I4_0)
                .InstructionEnumeration();
        }
    }
}