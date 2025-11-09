using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField, Min(0f)] private float _smoothTime = 0.3f;
    public Transform TargetTransform;
    private Vector3 _velocity;

    private void LateUpdate()
    {
        if (_cameraTransform == null ||
            TargetTransform == null)
        {
            return;
        }
        _cameraTransform.position = Vector3.SmoothDamp(_cameraTransform.position, TargetTransform.position, ref _velocity, _smoothTime);
    }
}
