using System.Collections;
using UnityEngine;

public class ElectricParticleDelayedReleaser : MonoBehaviour
{
    [SerializeField] private ElectricParticle _electricParticle;
    [SerializeField, Min(0f)] private float _delay = 0.5f;
    private IEnumerator _delayCoroutine;

    private void OnEnable()
    {
        if (_electricParticle != null)
        {
            _electricParticle.OnPathCompleted?.AddListener(StartDelayedRelease);
            if (_electricParticle.IsEndReached) 
            {
                StartDelayedRelease();
            }
        }
    }

    private void StartDelayedRelease()
    {
        if (_electricParticle == null)
        {
            return;
        }
        if (_delay < 0f ||
            !isActiveAndEnabled)
        {
            _electricParticle.Release();
            return;
        }
        if (_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
        }
        _delayCoroutine = DelayedRelease();
        StartCoroutine(_delayCoroutine);
    }

    private IEnumerator DelayedRelease()
    {
        yield return new WaitForSeconds(_delay);
        if (_electricParticle != null)
        {
            _electricParticle.Release();
        }
        _delayCoroutine = null;
    }

    private void OnDisable()
    {
        if (_electricParticle != null)
        {
            _electricParticle.OnPathCompleted?.RemoveListener(StartDelayedRelease);
        }
        _delayCoroutine = null;
    }
}
