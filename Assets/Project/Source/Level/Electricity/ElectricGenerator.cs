using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElectricGenerator : MonoBehaviour
{
    [SerializeField] private TimedBehavior _timedBehavior;
    [SerializeField] private ElectricParticlePool _electricParticlePool;
    [SerializeField] private Transform _electricParticleParent;
    [SerializeField, Min(0.05f)] private float _minExistenceTimeForNewParticle = 3f;
    [SerializeField, Min(1)] private int _particlesLimit = 1;
    [SerializeField] private TimeDome _timeDome;
    [SerializeField] private LineRendererPath _electricPath;
    [SerializeField] private bool _releaseParticleOnEndReached = false;
    [field: SerializeField] public UnityEvent OnEndReached { get; private set; }
    private readonly HashSet<ElectricParticle> _activeParticles = new();
    private ElectricParticle _lastParticle;

    private void FixedUpdate()
    {
        if (_timedBehavior == null ||
            !_timedBehavior.IsUsingTime ||
            _activeParticles.Count >= _particlesLimit ||
            (_lastParticle != null && _lastParticle.ExistenceTime < _minExistenceTimeForNewParticle) ||
            _electricParticlePool == null ||
            !_electricParticlePool.TryGet(out var particle))
        {
            return;
        }
        var particleTransform = particle.transform;
        particleTransform.SetParent(_electricParticleParent);
        particle.SetDomeAndPath(_timeDome, _electricPath);
        particle.OnEndReached += EndReached;
        particle.BeforeReleased += ReleaseParticle;
        _activeParticles.Add(particle);
        _lastParticle = particle;
    }

    private void ReleaseParticle(ElectricParticle particle)
    {
        if (particle == null)
        {
            return;
        }
        UnsubscribeFromParticle(particle);
        _activeParticles.Remove(particle);
        if (_lastParticle == particle)
        {
            _lastParticle = null;
        }
    }

    private void UnsubscribeFromParticle(ElectricParticle particle)
    {
        particle.OnEndReached -= EndReached;
        particle.BeforeReleased -= ReleaseParticle;
    }

    private void EndReached(ElectricParticle particle)
    {
        OnEndReached?.Invoke();
        if (particle != null &&
            _releaseParticleOnEndReached)
        {
            particle.Release();
        }
    }

    private void OnDisable()
    {
        foreach (var particle in _activeParticles)
        {
            if (particle == null)
            {
                continue;
            }
            UnsubscribeFromParticle(particle);
        }
        _activeParticles.Clear();
        _lastParticle = null;
    }
}
