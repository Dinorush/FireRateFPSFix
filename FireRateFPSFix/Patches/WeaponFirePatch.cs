using HarmonyLib;
using Gear;
using FireRateFPSFix.Dependencies;
using FireRateFPSFix.FireState;

namespace FireRateFPSFix.Patches
{
    [HarmonyPatch]
    internal static class WeaponFirePatch
    {
        [HarmonyPatch(typeof(BWA_Burst), nameof(BWA_Burst.OnStartFiring))]
        [HarmonyPatch(typeof(BWA_Auto), nameof(BWA_Auto.OnStartFiring))]
        [HarmonyPatch(typeof(BulletWeaponArchetype), nameof(BulletWeaponArchetype.OnStartFiring))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        [HarmonyAfter(EWCWrapper.PLUGIN_GUID)]
        private static void StartFiringCallback(BulletWeaponArchetype __instance)
        {
            FireStateManager.GetUpdater(__instance.m_weapon).UpdateStartFiring();
        }

        [HarmonyPatch(typeof(BulletWeaponArchetype), nameof(BulletWeaponArchetype.PostFireCheck))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        [HarmonyAfter(EWCWrapper.PLUGIN_GUID)]
        private static void PostFireCallback(BulletWeaponArchetype __instance)
        {
            var updater = FireStateManager.GetUpdater(__instance.m_weapon);
            if (__instance.m_firing)
                updater.UpdateNextFireTime();
            else
                updater.UpdateEndFiring();
        }
    }
}
