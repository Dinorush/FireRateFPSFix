using HarmonyLib;
using Gear;
using FireRateFPSFix.Dependencies;
using FireRateFPSFix.FireState;

namespace FireRateFPSFix.Patches
{
    [HarmonyPatch]
    internal static class WeaponFirePatch
    {
        [HarmonyPatch(typeof(BulletWeaponArchetype), nameof(BulletWeaponArchetype.PostFireCheck))]
        [HarmonyWrapSafe]
        [HarmonyPrefix]
        private static void PrePostFireCallback(BulletWeaponArchetype __instance)
        {
            var updater = FireStateManager.GetUpdater(__instance.m_weapon);
            if (__instance.m_firing)
                updater.UpdateFired();
        }

        [HarmonyPatch(typeof(BulletWeaponArchetype), nameof(BulletWeaponArchetype.PostFireCheck))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        [HarmonyAfter(EWCWrapper.PLUGIN_GUID)]
        private static void PostFireCallback_AfterEWC(BulletWeaponArchetype __instance)
        {
            var updater = FireStateManager.GetUpdater(__instance.m_weapon);
            updater.OnPostFireCheck();
        }

        [HarmonyPatch(typeof(BWA_Auto), nameof(BWA_Auto.OnStopFiring))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void PostStopFiringCallback(BulletWeaponArchetype __instance)
        {
            var updater = FireStateManager.GetUpdater(__instance.m_weapon);
            updater.OnStopFiring();
        }
    }
}
