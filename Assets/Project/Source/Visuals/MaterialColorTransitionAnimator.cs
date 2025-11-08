using System.Collections;
using UnityEngine;

public class MaterialColorTransitionAnimator : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField, Min(0)] private int _materialIndex = 0;
    [SerializeField] private string _propertyName = string.Empty;
    [SerializeField] private Color _firstColor = Color.clear;
    [SerializeField] private Color _secondColor = Color.white;
    [SerializeField] private AnimationCurve _firstToSecondCurve;
    [SerializeField, Min(0f)] private float _firstToSecondDuration = 1f;
    [SerializeField] private AnimationCurve _secondToFirstCurve;
    [SerializeField, Min(0f)] private float _secondToFirstDuration = 1f;
    private MaterialPropertyBlock _propertyBlock;
    private IEnumerator _transitionCoroutine;

    public void StartFirstToSecondTransition()
    {
        StartTransition(_secondColor, _firstToSecondCurve, _firstToSecondDuration);
    }

    public void StartSecondToFirstTransition()
    {
        StartTransition(_firstColor, _secondToFirstCurve, _secondToFirstDuration);
    }

    public void StartTransition(bool firstToSecond)
    {
        if (firstToSecond)
        {
            StartFirstToSecondTransition();
        }
        else
        {
            StartSecondToFirstTransition();
        }
    }

    private void StartTransition(Color toColor, AnimationCurve curve, float duration)
    {
        if (!isActiveAndEnabled ||
            _renderer == null ||
            !_renderer.IsHavingMaterial(_materialIndex))
        {
            return;
        }
        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }
        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = null;
        }
        _renderer.GetPropertyBlock(_propertyBlock, _materialIndex);
        Color fromColor = _propertyBlock.GetColor(_propertyName);
        if (Mathf.Approximately(duration, 0f))
        {
            Color fromToColor = GetFromToColor(fromColor, toColor, curve, 1f);
            ApplyColor(fromToColor);
            return;
        }
        Color startColor = _propertyBlock.GetColor(_propertyName);
        _transitionCoroutine = Transition(fromColor, toColor, curve, duration);
        StartCoroutine(_transitionCoroutine);
    }

    private IEnumerator Transition(Color fromColor, Color toColor, AnimationCurve curve, float duration)
    {
        float timeSpent = 0f;
        while (timeSpent < duration)
        {
            if (_renderer == null)
            {
                _transitionCoroutine = null;
                yield break;
            }
            float timeFactor = timeSpent / duration;
            Color fromToColor = GetFromToColor(fromColor, toColor, curve, timeFactor);
            ApplyColor(fromToColor);
            yield return null;
            timeSpent += Time.deltaTime;
        }
        if (_renderer == null)
        {
            _transitionCoroutine = null;
            yield break;
        }
        ApplyColor(GetFromToColor(fromColor, toColor, curve, 1f));
    }

    private void ApplyColor(Color color)
    {
        _propertyBlock.SetColor(_propertyName, color);
        _renderer.SetPropertyBlock(_propertyBlock, _materialIndex);
    }

    private Color GetFromToColor(Color fromColor, Color toColor, AnimationCurve curve, float timeFactor)
    {
        float fromToFactor = (curve != null) ? curve.Evaluate(timeFactor) : timeFactor;
        return Color.Lerp(fromColor, toColor, fromToFactor);
    }

    private void OnDisable()
    {
        _transitionCoroutine = null;
    }
}
