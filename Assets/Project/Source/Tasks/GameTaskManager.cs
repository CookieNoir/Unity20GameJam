using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameTaskManager : MonoBehaviour
{
    [field: SerializeField] public UnityEvent<GameTask> OnTaskStarted { get; private set; }
    [field: SerializeField] public UnityEvent<GameTask> OnTaskCompleted { get; private set; }
    private readonly HashSet<GameTask> _startedTasks = new();
    private readonly HashSet<GameTask> _completedTasks = new();

    public IReadOnlyCollection<GameTask> StartedTasks => _startedTasks;

    public void StartTask(GameTask gameTask)
    {
        if (gameTask == null ||
            _startedTasks.Contains(gameTask) ||
            _completedTasks.Contains(gameTask))
        {
            return;
        }
        _startedTasks.Add(gameTask);
        Debug.Log($"{gameObject.name}: Task {gameTask.name} started.", this);
        OnTaskStarted?.Invoke(gameTask);
    }

    public bool IsTaskStarted(GameTask gameTask)
    {
        return gameTask != null &&
            _startedTasks.Contains(gameTask);
    }

    public void CompleteTask(GameTask gameTask)
    {
        if (gameTask == null ||
            !_startedTasks.Contains(gameTask))
        {
            return;
        }
        _startedTasks.Remove(gameTask);
        _completedTasks.Add(gameTask);
        Debug.Log($"{gameObject.name}: Task {gameTask.name} completed.", this);
        OnTaskCompleted?.Invoke(gameTask);
    }
}
