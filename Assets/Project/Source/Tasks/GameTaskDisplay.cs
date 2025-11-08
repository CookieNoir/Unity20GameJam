using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameTaskDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _textField;
    [field: SerializeField] public UnityEvent OnTaskCompleted { get; private set; }
    private IPoolReleaser<GameTaskDisplay> _pool;
    private bool _isTaskCompleted = false;

    public bool IsTaskCompleted => _isTaskCompleted;

    public void SetPool(IPoolReleaser<GameTaskDisplay> pool)
    {
        _pool = pool;
    }

    public void SetTask(GameTask gameTask)
    {
        _isTaskCompleted = false;
        if (gameTask == null ||
            _textField == null)
        {
            return;
        }
        _textField.text = gameTask.Description;
    }

    public void Release()
    {
        if (_pool == null ||
            _pool as MonoBehaviour == null)
        {
            Destroy(gameObject);
            return;
        }
        _pool.Release(this);
    }

    public void SetTaskCompleted()
    {
        if (_isTaskCompleted)
        {
            return;
        }
        _isTaskCompleted = true;
        OnTaskCompleted?.Invoke();
    }
}
