using System.Collections;
using UnityEngine;

public class ScaleTransitionAnimator : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Vector3 _minScale = Vector3.zero;
    [SerializeField] private Vector3 _maxScale = Vector3.one;
    [SerializeField, Min(0f)] private float _toMaxScaleDuration = 1f;
    [SerializeField] private AnimationCurve _toMaxScaleCurve;
    [SerializeField, Min(0f)] private float _toMinScaleDuration = 1f;
    [SerializeField] private AnimationCurve _toMinScaleCurve;
    private IEnumerator _transitionCoroutine;

    public void StartToMaxScaleTransition()
    {
        StartTransition(_maxScale, _toMaxScaleCurve, _toMaxScaleDuration);
    }

    public void StartToMinScaleTransition()
    {
        StartTransition(_minScale, _toMinScaleCurve, _toMinScaleDuration);
    }

    public void StartTransition(bool toMaxScale)
    {
        if (toMaxScale)
        {
            StartToMaxScaleTransition();
        }
        else
        {
            StartToMinScaleTransition();
        }
    }

    private void StartTransition(Vector3 toScale, AnimationCurve curve, float duration)
    {
        if (!isActiveAndEnabled ||
            _targetTransform == null)
        {
            return;
        }
        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = null;
        }
        Vector3 fromScale = _targetTransform.localScale;
        if (Mathf.Approximately(duration, 0f))
        {
            Vector3 scale = GetFromToScale(fromScale, toScale, curve, 1f);
            _targetTransform.localScale = scale;
            return;
        }
        _transitionCoroutine = AnimateTransition(fromScale, toScale, curve, duration);
        StartCoroutine(_transitionCoroutine);
    }

    private IEnumerator AnimateTransition(Vector3 fromScale, Vector3 toScale, AnimationCurve curve, float duration)
    {
        float timeSpent = 0f;
        while (timeSpent < duration)
        {
            float timeFactor = timeSpent / duration;
            Vector3 scale = GetFromToScale(fromScale, toScale, curve, timeFactor);
            _targetTransform.localScale = scale;
            yield return null;
            if (_targetTransform == null)
            {
                _transitionCoroutine = null;
                yield break;
            }
            timeSpent += Time.deltaTime;
        }
        _targetTransform.localScale = GetFromToScale(fromScale, toScale, curve, 1f);
        _transitionCoroutine = null;
    }

    private Vector3 GetFromToScale(Vector3 fromScale, Vector3 toScale, AnimationCurve curve, float timeFactor)
    {
        float fromToFactor = (curve != null) ? curve.Evaluate(timeFactor) : timeFactor;
        return Vector3.Lerp(fromScale, toScale, fromToFactor);
    }

    private void OnDisable()
    {
        _transitionCoroutine = null;
    }
}
