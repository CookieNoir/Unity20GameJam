using UnityEngine;

public class ElectricPath : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineSource;
    [SerializeField] private bool _resetPointsOnEnable = true;
    private Vector3[] _points;
    private float[] _distances;
    private int _pointsCount;
    private bool _arePointsReceived = false;
    private float _totalDistance;

    public float TotalDistance => _totalDistance;

    private bool ArePointsReceived()
    {
        if (_arePointsReceived)
        {
            return true;
        }
        if (_lineSource == null)
        {
            return false;
        }
        int positionCount = _lineSource.positionCount;
        _totalDistance = 0f;
        _arePointsReceived = true;
        if (positionCount == 0)
        {
            _pointsCount = 0;
            return true;
        }
        if (_points == null ||
            _points.Length < positionCount)
        {
            _points = new Vector3[positionCount];
            _distances = new float[positionCount];
        }
        _pointsCount = _lineSource.GetPositions(_points);
        _distances[0] = _totalDistance;
        for (int i = 1; i < _pointsCount; ++i)
        {
            _totalDistance += Vector3.Distance(_points[i - 1], _points[i]);
            _distances[i] = _totalDistance;
        }
        return true;
    }

    private void OnEnable()
    {
        if (_resetPointsOnEnable)
        {
            _arePointsReceived = false;
        }
    }

    public bool TryGetPositionByDistance(float distance, out Vector3 position)
    {
        position = Vector3.zero;
        if (distance < 0f ||
            !ArePointsReceived() ||
            _pointsCount <= 0)
        {
            return false;
        }
        if (distance >= _totalDistance)
        {
            position = _points[_pointsCount - 1];
            return true;
        }
        int index = FindLowerNearest(_distances, distance);
        float factor = Mathf.InverseLerp(_distances[index], _distances[index + 1], distance);
        position = Vector3.Lerp(_points[index], _points[index + 1], factor);
        return true;
    }

    public static int FindLowerNearest(float[] arr, float target)
    {
        int left = 0;
        int right = arr.Length - 1;
        int result = -1;
        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            if (arr[mid] == target)
            {
                return mid;
            }
            else if (arr[mid] < target)
            {
                result = mid;
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }
        return result;
    }
}
