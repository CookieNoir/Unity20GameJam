using UnityEngine;

public class TimeDome : MonoBehaviour
{
    [SerializeField] private SphereCollider _domeCollider;

    private Vector3 DomeCenter => _domeCollider != null ? _domeCollider.transform.TransformPoint(_domeCollider.center) : transform.position;

    private float DomeRadius => GetMaxScale(_domeCollider.transform) * _domeCollider.radius;

    public bool ContainsSphere(SphereCollider other)
    {
        if (_domeCollider == null ||
            other == null)
        {
            return false;
        }
        Vector3 otherCenter = other.transform.TransformPoint(other.center);
        float otherRadius = GetMaxScale(other.transform) * other.radius;
        return ContainsSphere(otherCenter, otherRadius);
    }

    public bool ContainsSphere(Vector3 center, float radius)
    {
        if (_domeCollider == null)
        {
            return false;
        }
        float centerDistance = Vector3.Distance(DomeCenter, center);
        return centerDistance + radius <= DomeRadius;
    }

    public bool ContainsBox(BoxCollider other)
    {
        if (_domeCollider == null ||
            other == null)
        {
            return false;
        }

        Vector3 center = other.center;
        Vector3 extents = other.size * 0.5f;

        Vector3 domeCenter = DomeCenter;
        float domeRadius = DomeRadius;
        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    var localCorner = new Vector3(extents.x * x, extents.y * y, extents.z * z);
                    Vector3 corner = other.transform.TransformPoint(center + localCorner);
                    float distance = Vector3.Distance(domeCenter, corner);
                    if (distance > domeRadius)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private float GetMaxScale(Transform transform)
    {
        Vector3 lossyScale = transform.lossyScale;
        return Mathf.Max(lossyScale.x, lossyScale.y, lossyScale.z);
    }

    private void OnDrawGizmos()
    {
        if (_domeCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(DomeCenter, DomeRadius);
        }
    }
}
