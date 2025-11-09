using UnityEngine;

public class CameraTargetChanger : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private CameraMovement _cameraMovement;

    public void ChangeTarget()
    {
        if (_cameraMovement == null)
        {
            return;
        }
        _cameraMovement.TargetTransform = _targetTransform;
    }
}
