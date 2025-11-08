using UnityEngine;

public class GameTaskController : MonoBehaviour
{
    [SerializeField] private GameTask _gameTask;
    [SerializeField] private GameTaskManager _gameTaskManager;
    [SerializeField] private bool _startTaskOnEnable = false;

    private void OnEnable()
    {
        if (_startTaskOnEnable)
        {
            StartTask();
        }
    }

    public void StartTask()
    {
        if (_gameTask == null ||
            _gameTaskManager == null)
        {
            return;
        }
        _gameTaskManager.StartTask(_gameTask);
    }

    public void CompleteTask()
    {
        if (_gameTask == null ||
            _gameTaskManager == null)
        {
            return;
        }
        _gameTaskManager.CompleteTask(_gameTask);
    }
}
