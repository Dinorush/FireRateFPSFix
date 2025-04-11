using FireRateFPSFix.Dependencies;
using Gear;
using System;

namespace FireRateFPSFix.FireState
{
    public sealed class FireStateUpdater
    {
        private float _timeBuffer = 0f;
        private float _lastFireTime = 0f;
        private readonly BulletWeapon _weapon;
        private BulletWeaponArchetype _archetype;
        private float _shotDelay;
        private float _burstDelay;
        private float _cooldownDelay;
        private IntPtr _archetypePtr;

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
            _lastFireTime = Clock.Time;
        }

        public bool IsValid => _weapon != null;

        private float GetCooldownDelay() => _cooldownDelay;
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

        public void UpdateEndFiring()
        {
            UpdateArchetype();

            float delay = _archetype.HasCooldown ? GetCooldownDelay() : GetBurstDelay();
            _archetype.m_nextBurstTimer = Clock.Time - _timeBuffer + delay;
            _archetype.m_nextShotTimer = Clock.Time - _timeBuffer + GetShotDelay();
            _timeBuffer -= Math.Min(_timeBuffer, delay); // Avoid double counting buffer when UpdateStartFiring runs
        }

        public void UpdateNextFireTime()
        {
            UpdateArchetype();

            float time = Clock.Time;
            if (_lastFireTime != 0)
                _timeBuffer += time - _lastFireTime - GetShotDelay();
            _archetype.m_nextShotTimer -= _timeBuffer;
            _lastFireTime = time;
        }
    }
}
