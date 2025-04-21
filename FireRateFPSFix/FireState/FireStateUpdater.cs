using FireRateFPSFix.Dependencies;
using Gear;
using System;

namespace FireRateFPSFix.FireState
{
    public sealed class FireStateUpdater
    {
        private readonly BulletWeapon _weapon;
        private BulletWeaponArchetype _archetype;
        private float _shotDelay;
        private float _burstDelay;
        private float _cooldownDelay;
        private IntPtr _archetypePtr;

        private float _timeBuffer = 0f;
        private float _lastFireTime = 0f;

        public FireStateUpdater(BulletWeapon weapon)
        {
            _weapon = weapon;
            _archetype = _weapon.m_archeType;
            _shotDelay = _archetype.ShotDelay();
            _burstDelay = _archetype.BurstDelay();
            _cooldownDelay = _archetype.CooldownDelay();
            _archetypePtr = _archetype.Pointer;
        }

        private void UpdateArchetype()
        {
            var newArch = _weapon.m_archeType;
            if (_archetypePtr == newArch.Pointer) return;

            _archetype = newArch;
            _shotDelay = _archetype.ShotDelay();
            _burstDelay = _archetype.BurstDelay();
            _cooldownDelay = _archetype.CooldownDelay();
            _archetypePtr = _archetype.Pointer;
            _timeBuffer = 0f;
            _lastFireTime = 0f;
        }

        public bool IsValid => _weapon != null;

        private float GetCooldownDelay() => EWCWrapper.GetCooldownDelay(_weapon, _cooldownDelay);
        private float GetBurstDelay() => EWCWrapper.GetBurstDelay(_weapon, _burstDelay);
        private float GetShotDelay() => EWCWrapper.GetShotDelay(_weapon, _shotDelay);

        public void UpdateStartFiring()
        {
            UpdateArchetype();

            float nextTime = _archetype.m_nextBurstTimer;
            float time = Clock.Time;
            if (nextTime + Clock.Delta >= time)
                _timeBuffer += time - nextTime;
            else
                _timeBuffer = 0f;
            _lastFireTime = 0;
        }

        public void UpdateFired()
        {
            UpdateArchetype();

            float time = Clock.Time;
            if (_lastFireTime != 0)
                _timeBuffer += time - _lastFireTime - GetShotDelay();
            _lastFireTime = time;
        }

        public void UpdateFireTime()
        {
            UpdateArchetype();

            if (_archetype.m_firing)
                _archetype.m_nextShotTimer -= _timeBuffer;
            else
            {
                float delay = _archetype.HasCooldown ? GetCooldownDelay() : GetBurstDelay();
                _archetype.m_nextBurstTimer = Clock.Time - _timeBuffer + delay;
                _archetype.m_nextShotTimer = Clock.Time - _timeBuffer + GetShotDelay();
                _timeBuffer -= Math.Min(_timeBuffer, delay); // Avoid double counting buffer when UpdateStartFiring runs
            }
        }
    }
}
