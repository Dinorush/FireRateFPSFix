using Gear;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FireRateFPSFix.FireState
{
    public static class FireStateManager
    {
        private static readonly Dictionary<IntPtr, FireStateUpdater> _updaters = new();

        public static FireStateUpdater GetUpdater(BulletWeapon weapon)
        {
            if (_updaters.TryGetValue(weapon.Pointer, out var updater))
                return updater;

            return _updaters[weapon.Pointer] = new(weapon);
        }

        public static void Cleanup()
        {
            foreach (var kv in _updaters.Where(kv => !kv.Value.IsValid).ToList().ToArray())
                _updaters.Remove(kv.Key);
        }
    }
}
