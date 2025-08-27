using BepInEx.Unity.IL2CPP;
using EWC.API;
using EWC.CustomWeapon;
using FireRateFPSFix.FireState;
using Gear;
using System.Runtime.CompilerServices;

namespace FireRateFPSFix.Dependencies
{
    internal static class EWCWrapper
    {
        public const string PLUGIN_GUID = "Dinorush.ExtraWeaponCustomization";

        public static readonly bool hasEWC;

        static EWCWrapper()
        {
            hasEWC = IL2CPPChainloader.Instance.Plugins.ContainsKey(PLUGIN_GUID);
            if (hasEWC)
                AddFireRateChangeCallback();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void AddFireRateChangeCallback()
        {
            FireRateAPI.CooldownSet += (weapon, shotDelay, burstDelay, cooldownDelay) =>
            {
                FireStateManager.GetUpdater(weapon).EWCOnCooldownSet(shotDelay, burstDelay, cooldownDelay);
            };
        }

        public static bool GetDelays(BulletWeapon weapon, out float shotDelay, out float burstDelay, out float cooldownDelay)
        {
            if (!hasEWC)
            {
                shotDelay = 0;
                burstDelay = 0;
                cooldownDelay = 0;
                return false;
            }
            return GetDelays_Internal(weapon, out shotDelay, out burstDelay, out cooldownDelay);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool GetDelays_Internal(BulletWeapon weapon, out float shotDelay, out float burstDelay, out float cooldownDelay)
        {
            var cwc = weapon.GetComponent<CustomWeaponComponent>();
            if (cwc == null)
            {
                shotDelay = 0;
                burstDelay = 0;
                cooldownDelay = 0;
                return false;
            }
            shotDelay = 1f / cwc.CurrentFireRate;
            burstDelay = cwc.CurrentBurstDelay;
            cooldownDelay = cwc.CurrentCooldownDelay;
            return true;
        }
    }
}
