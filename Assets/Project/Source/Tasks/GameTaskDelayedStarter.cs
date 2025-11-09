using System.Collections;
using UnityEngine;

public class GameTaskDelayedStarter : MonoBehaviour
{
    [System.Serializable]
    private class GameTaskDelayPair
    {
        [field: SerializeField] public GameTask GameTask { get; private set; }
        [field: SerializeField, Min(0f)] public float Delay { get; private set; } = 0f;
    }

    [SerializeField] private GameTaskManager _taskManager;
    [SerializeField] private GameTaskDelayPair[] _taskDelayPairs;
    [SerializeField] private bool _startTasksOnEnable = false;
    private IEnumerator _startSequenceCoroutine;

    private void OnEnable()
    {
        if (_startTasksOnEnable)
        {
            StartTasks();
        }
    }

    public void StartTasks()
    {
        if (!isActiveAndEnabled ||
            _taskManager == null ||
            _taskDelayPairs == null ||
            _taskDelayPairs.Length == 0)
        {
            return;
        }
        if (_startSequenceCoroutine != null)
        {
            StopCoroutine(_startSequenceCoroutine);
        }
        _startSequenceCoroutine = StartSequence();
        StartCoroutine(_startSequenceCoroutine);
    }

    private IEnumerator StartSequence()
    {
        for (int i = 0; i < _taskDelayPairs.Length; ++i)
        {
            var pair = _taskDelayPairs[i];
            yield return new WaitForSeconds(pair.Delay);
            if (_taskManager == null)
            {
                _startSequenceCoroutine = null;
                yield break;
            }
            _taskManager.StartTask(pair.GameTask);
            if (_taskDelayPairs == null)
            {
                _startSequenceCoroutine = null;
                yield break;
            }
        }
        _startSequenceCoroutine = null;
    }

    private void OnDisable()
    {
        _startSequenceCoroutine = null;
    }
}
