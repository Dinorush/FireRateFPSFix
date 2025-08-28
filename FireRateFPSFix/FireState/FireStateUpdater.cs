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
        private float _nextShotTimeBase = 0f;
        private float _nextShotTime = 0f;
        private bool _inCooldown = true;

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

        private void SetNextShotTime(float time)
        {
            _nextShotTimeBase = time;
            _nextShotTime = Math.Max(time - _timeBuffer, Clock.Time);
        }

        private float GetCooldown(bool includeShotDelay = false)
        {
            float burstDelay = _hasCooldown ? _cooldownDelay : _burstDelay;
            if (includeShotDelay)
                return Math.Max(_shotDelay, burstDelay);
            return burstDelay;
        }

        public void EWCOnCooldownSet(float shotDelay, float burstDelay, float cooldownDelay)
        {
            _shotDelay = shotDelay;
            _burstDelay = burstDelay;
            _cooldownDelay = cooldownDelay;
            _hasCooldown = _weapon.m_archeType.HasCooldown;

            SetNextShotTime(Math.Max(Clock.Time, _lastFireTime + (_inCooldown ? _shotDelay : GetCooldown(true))));
        }

        public bool IsValid => _weapon != null;

        public void UpdateFired()
        {
            float time = Clock.Time;
            if (_nextShotTime + Clock.Delta >= time)
                _timeBuffer += time - _nextShotTimeBase;
            else
                _timeBuffer = 0;
            _inCooldown = false;
            _lastFireTime = time;
        }

        public void OnPostFireCheck()
        {
            var archetype = _weapon.m_archeType;
            var time = Clock.Time;
            if (archetype.m_firing)
            {
                SetNextShotTime(archetype.m_nextShotTimer);
                archetype.m_nextShotTimer = _nextShotTime;
            }
            else
            {
                _inCooldown = true;
                SetNextShotTime(time + Math.Max(_shotDelay, GetCooldown()));
                archetype.m_nextBurstTimer = Math.Max(time, time + GetCooldown() - _timeBuffer);
                archetype.m_nextShotTimer = Math.Max(time, time + _shotDelay - _timeBuffer);
            }
        }

        public void OnStopFiring()
        {
            // Usually overridden by UpdateFireTime, but Auto weapons can call
            // OnStopFiring without entering PostFireCheck
            _inCooldown = true;
            SetNextShotTime(_lastFireTime + _shotDelay);
        }
    }
}
