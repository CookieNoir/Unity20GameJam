using System.Collections.Generic;
using UnityEngine;

public class GameTaskDisplayController : MonoBehaviour
{
    [SerializeField] private GameTaskManager _gameTaskManager;
    [SerializeField] private GameTaskDisplayPool _displayPool;
    [SerializeField] private Transform _displayParent;
    [SerializeField] private bool _releaseDisplayOnTaskCompleted = false;
    private readonly Dictionary<GameTask, GameTaskDisplay> _taskDisplayPairs = new();

    private void OnEnable()
    {
        if (_gameTaskManager != null)
        {
            _gameTaskManager.OnTaskStarted?.AddListener(AddDisplay);
            _gameTaskManager.OnTaskCompleted?.AddListener(SetTaskCompleted);
            AddDisplays(_gameTaskManager.StartedTasks);
        }
    }

    private void OnDisable()
    {
        if (_gameTaskManager != null)
        {
            _gameTaskManager.OnTaskStarted?.RemoveListener(AddDisplay);
            _gameTaskManager.OnTaskCompleted?.RemoveListener(SetTaskCompleted);
        }
        ReleaseAllDisplays();
    }

    private void AddDisplay(GameTask gameTask)
    {
        if (gameTask == null ||
            _taskDisplayPairs.ContainsKey(gameTask) ||
            _displayPool == null ||
            !_displayPool.TryGet(out var display))
        {
            return;
        }
        display.transform.SetParent(_displayParent);
        display.SetTask(gameTask);
        _taskDisplayPairs[gameTask] = display;
    }

    private void AddDisplays(IReadOnlyCollection<GameTask> startedTasks)
    {
        if (startedTasks == null ||
            startedTasks.Count == 0)
        {
            return;
        }
        foreach (var gameTask in startedTasks)
        {
            AddDisplay(gameTask);
        }
    }

    private void SetTaskCompleted(GameTask gameTask)
    {
        if (gameTask == null ||
            !_taskDisplayPairs.TryGetValue(gameTask, out var display))
        {
            return;
        }
        _taskDisplayPairs.Remove(gameTask);
        if (display == null)
        {
            return;
        }
        display.SetTaskCompleted();
        if (_releaseDisplayOnTaskCompleted)
        {
            display.Release();
        }
    }

    private void ReleaseDisplay(GameTask gameTask)
    {
        if (gameTask == null ||
            !_taskDisplayPairs.TryGetValue(gameTask, out var display))
        {
            return;
        }
        display.Release();
    }

    private void ReleaseAllDisplays()
    {
        if (_taskDisplayPairs.Count == 0)
        {
            return;
        }
        foreach (var gameTask in _taskDisplayPairs.Keys)
        {
            ReleaseDisplay(gameTask);
        }
        _taskDisplayPairs.Clear();
    }
}
