using FireRateFPSFix.Dependencies;
using Gear;
using System;

namespace FireRateFPSFix.FireState
{
    public sealed class FireStateUpdater
    {
        private readonly BulletWeapon _weapon;
        private float _shotDelay;
        private float _burstDelay;
        private float _cooldownDelay;
        private bool _hasCooldown;

        private float _timeBuffer = 0f;
        private float _lastFireTime = 0f;
        private float _nextBurstTime = 0f;

        public FireStateUpdater(BulletWeapon weapon)
        {
            _weapon = weapon;
            var archetype = _weapon.m_archeType;
            if (!EWCWrapper.GetDelays(_weapon, out _shotDelay, out _burstDelay, out _cooldownDelay))
            {
                _shotDelay = archetype.ShotDelay();
                _burstDelay = archetype.BurstDelay();
                _cooldownDelay = archetype.CooldownDelay();
            }
            _hasCooldown = archetype.HasCooldown;
        }

        private float GetCooldown() => _hasCooldown ? _cooldownDelay : _burstDelay;

        public void EWCOnCooldownSet(float shotDelay, float burstDelay, float cooldownDelay)
        {
            _shotDelay = shotDelay;
            _burstDelay = burstDelay;
            _cooldownDelay = cooldownDelay;
            _hasCooldown = _weapon.m_archeType.HasCooldown;
            _nextBurstTime = Math.Max(_lastFireTime + GetCooldown(), Clock.Time);
        }

        public bool IsValid => _weapon != null;

        public void UpdateStartFiring()
        {
            float expectedTime = _lastFireTime + GetCooldown();
            float time = Clock.Time;
            if (_nextBurstTime + Clock.Delta >= time)
                _timeBuffer += time - expectedTime;
            else
                _timeBuffer = 0f;
            _lastFireTime = 0;
        }

        public void UpdateFired()
        {
            float time = Clock.Time;
            if (_lastFireTime != 0)
                _timeBuffer += time - _lastFireTime - _shotDelay;
            _lastFireTime = time;
        }

        public void UpdateFireTime()
        {
            var archetype = _weapon.m_archeType;
            if (archetype.m_firing)
                archetype.m_nextShotTimer -= _timeBuffer;
            else
            {
                var time = Clock.Time;
                archetype.m_nextBurstTimer = Math.Max(time, time + GetCooldown() - _timeBuffer);
                archetype.m_nextShotTimer = Math.Max(time, time + _shotDelay - _timeBuffer);
                _nextBurstTime = Math.Max(archetype.m_nextBurstTimer, archetype.m_nextShotTimer);
            }
        }
    }
}
