using BepInEx;
using BepInEx.Unity.IL2CPP;
using FireRateFPSFix.Dependencies;
using FireRateFPSFix.FireState;
using GTFO.API;
using HarmonyLib;

namespace FireRateFPSFix
{
    [BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.0.2")]
    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(EWCWrapper.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    internal sealed class EntryPoint : BasePlugin
    {
        public const string MODNAME = "FireRateFPSFix";

        public override void Load()
        {
            new Harmony(MODNAME).PatchAll();
            LevelAPI.OnEnterLevel += LevelAPI_OnEnterLevel;
            Log.LogMessage("Loaded " + MODNAME);
        }

        private void LevelAPI_OnEnterLevel()
        {
            FireStateManager.Cleanup();
        }
    }
}