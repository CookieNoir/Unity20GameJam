using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameTaskDisplayAnimator : MonoBehaviour
{
    [SerializeField] private GameTaskDisplay _display;
    [SerializeField] private RectTransform _layoutElementTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Vector3 _minScale = new(1f, 0f, 1f);
    [SerializeField] private Vector3 _maxScale = new(1f, 1f, 1f);
    [Header("On Enable")]
    [SerializeField, Min(0f)] private float _onEnableDuration = 1f;
    [SerializeField] private AnimationCurve _onEnableScalingCurve;
    [SerializeField] private AnimationCurve _onEnableAlphaCurve;
    [Header("On Completed")]
    [SerializeField, Min(0f)] private float _onCompletedDuration = 1f;
    [SerializeField] private AnimationCurve _onCompletedScalingCurve;
    [SerializeField] private AnimationCurve _onCompletedAlphaCurve;
    private IEnumerator _animationCoroutine;

    private void OnEnable()
    {
        bool isAnimationStarted = false;
        if (_display != null)
        {
            _display.OnTaskCompleted?.AddListener(StartOnTaskCompletedAnimation);
            if (_display.IsTaskCompleted)
            {
                StartOnTaskCompletedAnimation();
                isAnimationStarted = true;
            }
        }
        if (!isAnimationStarted)
        {
            StartOnEnableAnimation();
        }
    }

    private void StartOnEnableAnimation()
    {
        StartAnimation(_onEnableDuration, _onEnableScalingCurve, _onEnableAlphaCurve);
    }

    private void StartOnTaskCompletedAnimation()
    {
        StartAnimation(_onCompletedDuration, _onCompletedScalingCurve, _onCompletedAlphaCurve);
    }

    private void StartAnimation(float duration, AnimationCurve scalingCurve, AnimationCurve alphaCurve)
    {
        if (!isActiveAndEnabled ||
            (scalingCurve == null && alphaCurve == null) ||
            (_layoutElementTransform == null && _canvasGroup == null))
        {
            return;
        }
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
        if (duration <= 0f)
        {
            SetScale(scalingCurve, 1f);
            SetAlpha(alphaCurve, 1f);
            return;
        }
        _animationCoroutine = Animate(duration, scalingCurve, alphaCurve);
        StartCoroutine(_animationCoroutine);
    }

    private IEnumerator Animate(float duration, AnimationCurve scalingCurve, AnimationCurve alphaCurve)
    {
        float timeSpent = 0f;
        while (timeSpent < duration)
        {
            float timeFactor = timeSpent / duration;
            SetScale(scalingCurve, timeFactor);
            SetAlpha(alphaCurve, timeFactor);
            yield return null;
            timeSpent += Time.deltaTime;
        }
        SetScale(scalingCurve, 1f);
        SetAlpha(alphaCurve, 1f);
        _animationCoroutine = null;
    }

    private void SetScale(AnimationCurve curve, float timeFactor)
    {
        if (_layoutElementTransform == null)
        {
            return;
        }
        float factor = curve != null ? curve.Evaluate(timeFactor) : 0f;
        _layoutElementTransform.localScale = Vector3.Lerp(_minScale, _maxScale, factor);
        LayoutRebuilder.MarkLayoutForRebuild(_layoutElementTransform);
    }

    private void SetAlpha(AnimationCurve curve, float timeFactor)
    {
        if (_canvasGroup == null)
        {
            return;
        }
        float value = curve != null ? curve.Evaluate(timeFactor) : 0f;
        _canvasGroup.alpha = value;
    }

    private void OnDisable()
    {
        _animationCoroutine = null;
    }
}
