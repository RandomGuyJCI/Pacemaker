using BepInEx;
using HarmonyLib;

namespace Pacemaker
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInProcess("Rhythm Doctor.exe")]
    public class Pacemaker : BaseUnityPlugin
    {
        private const string modGUID = "com.rhythmdr.pacemaker";
        private const string modName = "Pacemaker";
        private const string modVersion = "1.1.0";
        private readonly Harmony harmony = new("com.rhythmdr.pacemaker");
        
        private void Awake()
        {
            Logger.LogInfo("Pacemaker successfully loaded!");
            harmony.PatchAll();
        }
    }
}