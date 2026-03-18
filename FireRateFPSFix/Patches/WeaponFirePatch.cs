using HarmonyLib;
using Gear;
using FireRateFPSFix.Dependencies;
using FireRateFPSFix.FireState;

namespace FireRateFPSFix.Patches
{
    [HarmonyPatch]
    internal static class WeaponFirePatch
    {
        [HarmonyPatch(typeof(BWA_Auto), nameof(BWA_Auto.OnFireShot))]
        [HarmonyPatch(typeof(BWA_Burst), nameof(BWA_Burst.OnFireShot))]
        [HarmonyPatch(typeof(BWA_Semi), nameof(BWA_Semi.OnFireShot))]
        [HarmonyWrapSafe]
        [HarmonyPriority(Priority.High)]
        [HarmonyPrefix]
        private static void PreFireCallback(BulletWeaponArchetype __instance, ref FireStateUpdater __state)
        {
            __state = FireStateManager.GetUpdater(__instance.m_weapon);
            __state.UpdatePreFired();
        }

        [HarmonyPatch(typeof(BWA_Auto), nameof(BWA_Auto.OnFireShot))]
        [HarmonyPatch(typeof(BWA_Burst), nameof(BWA_Burst.OnFireShot))]
        [HarmonyPatch(typeof(BWA_Semi), nameof(BWA_Semi.OnFireShot))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void PostFireCallback(bool __runOriginal, FireStateUpdater __state)
        {
            if (__runOriginal)
                __state.UpdatePostFired();
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
