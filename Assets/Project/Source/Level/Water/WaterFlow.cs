using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaterFlow : MonoBehaviour
{
    [SerializeField] private TimeDome _timeDome;
    [SerializeField] private LineRendererPath _waterPath;
    [SerializeField, Min(1)] private int _targetPointsCount = 64;
    [SerializeField, Min(0f)] private float _minPointRadius = 0.01f;
    [SerializeField] private bool _resetPointsOnEnable = true;
    [field: SerializeField] public UnityEvent OnPointsChanged { get; private set; }
    [field: SerializeField] public UnityEvent OnEndReached { get; private set; }
    private Vector3 _origin;
    private Vector3[] _points;
    private bool[] _pointsActivity;
    private bool[] _pointsWater;
    private int _pointsCount = 0;
    private float _pointRadius = 0f;
    private bool _arePointsReceived = false;

    public IReadOnlyList<bool> PointsActivity => _pointsCount > 0 ? _pointsActivity : Array.Empty<bool>();

    public IReadOnlyList<bool> PointsWater => _pointsCount > 0 ? _pointsWater : Array.Empty<bool>();

    private bool ArePointsReceived()
    {
        if (_arePointsReceived)
        {
            return true;
        }
        if (_waterPath == null ||
            !_waterPath.TryGetPositionByDistance(0f, out _origin))
        {
            return false;
        }
        float totalDistance = _waterPath.TotalDistance;
        if (Mathf.Approximately(totalDistance, 0f))
        {
            _pointsCount = 0;
            _pointRadius = _minPointRadius;
            _arePointsReceived = true;
            return true;
        }
        if (_points == null ||
            _points.Length < _targetPointsCount)
        {
            _points = new Vector3[_targetPointsCount];
            _pointsActivity = new bool[_targetPointsCount];
            _pointsWater = new bool[_targetPointsCount];
        }
        _pointsCount = _targetPointsCount;
        float step = totalDistance / _pointsCount;
        float offset = step / 2f;
        for (int i = 0; i < _pointsCount; ++i)
        {
            float distance = offset + i * step;
            _waterPath.TryGetPositionByDistance(distance, out _points[i]);
            _pointsActivity[i] = false;
            _pointsWater[i] = false;
        }
        _pointRadius = Mathf.Max(offset, _minPointRadius);
        _arePointsReceived = true;
        return true;
    }

    private void OnEnable()
    {
        if (_resetPointsOnEnable)
        {
            _arePointsReceived = false;
        }
    }

    private void FixedUpdate()
    {
        bool arePointsReceived = _arePointsReceived;
        if (!ArePointsReceived())
        {
            return;
        }
        bool arePointsChanged = _arePointsReceived != arePointsReceived;
        arePointsChanged |= UpdatePoints();
        if (arePointsChanged)
        {
            OnPointsChanged?.Invoke();
        }
    }

    private bool UpdatePoints()
    {
        if (!_arePointsReceived ||
            _timeDome == null)
        {
            return false;
        }
        bool isAnyPointChanged = false;
        if (_pointsCount > 0)
        {
            for (int i = _pointsCount - 1; i >= 0; --i)
            {
                bool wasActive = _pointsActivity[i];
                bool isActive = IsPointInsideDome(_points[i]);
                _pointsActivity[i] = isActive;
                isAnyPointChanged |= isActive != wasActive;
                if (isActive &&
                    _pointsWater[i])
                {
                    isAnyPointChanged |= TryPour(i);
                }
            }
        }
        if (IsPointInsideDome(_origin))
        {
            if (_pointsCount > 0)
            {
                if (_pointsActivity[0] &&
                    !_pointsWater[0])
                {
                    _pointsWater[0] = true;
                    isAnyPointChanged |= true;
                }
            }
            else
            {
                OnEndReached?.Invoke();
            }
        }
        return isAnyPointChanged;
    }

    private bool TryPour(int index)
    {
        if (index == (_pointsCount - 1))
        {
            _pointsWater[index] = false;
            OnEndReached?.Invoke();
            return true;
        }
        else
        {
            if (_pointsActivity[index + 1] &&
                !_pointsWater[index + 1])
            {
                _pointsWater[index] = false;
                _pointsWater[index + 1] = true;
                return true;
            }
        }
        return false;
    }

    private bool IsPointInsideDome(Vector3 point)
    {
        return _timeDome.ContainsSphere(point, _pointRadius);
    }
}
