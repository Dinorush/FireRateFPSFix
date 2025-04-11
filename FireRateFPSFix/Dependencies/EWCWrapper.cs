using BepInEx.Unity.IL2CPP;
using EWC.CustomWeapon;
using GameData;
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
        }

        public static float GetBurstDelay(BulletWeapon weapon, float burstDelay)
        {
            if (!hasEWC) return burstDelay;
            return GetBurstDelay_Internal(weapon, burstDelay);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float GetBurstDelay_Internal(BulletWeapon weapon, float burstDelay)
        {
            var cwc = weapon.GetComponent<CustomWeaponComponent>();
            return cwc != null ? cwc.CurrentBurstDelay : burstDelay;
        }

        public static float GetShotDelay(BulletWeapon weapon, float shotDelay)
        {
            if (!hasEWC) return shotDelay;
            return GetShotDelay_Internal(weapon, shotDelay);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static float GetShotDelay_Internal(BulletWeapon weapon, float shotDelay)
        {
            var cwc = weapon.GetComponent<CustomWeaponComponent>();
            return cwc != null ? 1f / cwc.CurrentFireRate : shotDelay;
        }
    }
}
