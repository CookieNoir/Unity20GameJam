using System.Collections;
using UnityEngine;

public class GameTaskDisplayDelayedReleaser : MonoBehaviour
{
    [SerializeField] private GameTaskDisplay _display;
    [SerializeField, Min(0f)] private float _delay = 0.5f;
    private IEnumerator _delayCoroutine;

    private void OnEnable()
    {
        if (_display != null)
        {
            _display.OnTaskCompleted?.AddListener(StartDelayedRelease);
            if (_display.IsTaskCompleted)
            {
                StartDelayedRelease();
            }
        }
    }

    private void StartDelayedRelease()
    {
        if (_display == null)
        {
            return;
        }
        if (_delay < 0f ||
            !isActiveAndEnabled)
        {
            _display.Release();
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
        if (_display != null)
        {
            _display.Release();
        }
        _delayCoroutine = null;
    }

    private void OnDisable()
    {
        if (_display != null)
        {
            _display.OnTaskCompleted?.RemoveListener(StartDelayedRelease);
        }
        _delayCoroutine = null;
    }
}
