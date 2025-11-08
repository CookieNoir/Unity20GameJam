using UnityEngine;
using UnityEngine.Pool;

public abstract class ComponentPool<TRequiredComponent> : MonoBehaviour where TRequiredComponent : Component
{
    [SerializeField] private GameObject _prefab;
    [field: SerializeField] public Transform InstanceParent { get; private set; }
    [SerializeField, Min(0)] private int _startInstancesCount = 0;
    [SerializeField] private bool _initializeOnStart = false;
    private ObjectPool<TRequiredComponent> _pool;

    private void Start()
    {
        if (_initializeOnStart)
        {
            EnsurePoolCreated();
        }
    }

    private ObjectPool<TRequiredComponent> Pool
    {
        get
        {
            EnsurePoolCreated();
            return _pool;
        }
    }

    private void EnsurePoolCreated()
    {
        if (_pool != null)
        {
            return;
        }
        _pool = new ObjectPool<TRequiredComponent>(CreateInstance,
            OnGetInstance,
            OnReleaseInstance,
            OnDestroyInstance,
            defaultCapacity: _startInstancesCount);
        for (int i = 0; i < _startInstancesCount; ++i)
        {
            var instance = CreateInstance();
            Release(instance);
        }
    }

    private TRequiredComponent CreateInstance()
    {
        if (_prefab == null)
        {
            Debug.LogError($"{gameObject.name}: Prefab is null.", this);
            return null;
        }
        GameObject instance = Instantiate(_prefab, InstanceParent);
        if (!instance.TryGetComponent(out TRequiredComponent component))
        {
            Destroy(instance);
            Debug.LogError($"{gameObject.name}: Failed to find component of type {typeof(TRequiredComponent)} in prefab instance.", this);
            return null;
        }
        OnInstanceCreated(component);
        return component;
    }

    public bool TryGet(out TRequiredComponent component)
    {
        component = Pool.Get();
        if (component == null)
        {
            Debug.LogError($"{gameObject.name}: Failed to create or retrieve instance from pool.", this);
            return false;
        }
        return true;
    }

    public void Release(TRequiredComponent component)
    {
        if (component == null)
        {
            return;
        }
        Pool.Release(component);
    }

    protected virtual void OnInstanceCreated(TRequiredComponent component)
    {
        component.gameObject.SetActive(false);
    }

    protected virtual void OnGetInstance(TRequiredComponent component)
    {
        if (component == null)
        {
            return;
        }
        component.gameObject.SetActive(true);
    }

    protected virtual void OnReleaseInstance(TRequiredComponent component)
    {
        if (component == null)
        {
            return;
        }
        component.gameObject.SetActive(false);
        component.transform.SetParent(InstanceParent);
    }

    protected virtual void OnDestroyInstance(TRequiredComponent component)
    {
        if (component == null ||
            component.gameObject == null)
        {
            return;
        }
        Destroy(component.gameObject);
    }

    protected void ValidatePrefab()
    {
        if (_prefab == null ||
            _prefab.TryGetComponent<TRequiredComponent>(out _))
        {
            return;
        }
        Debug.LogWarning($"{gameObject.name}: Failed to find component of type {typeof(TRequiredComponent)} in prefab. Prefab will be set null.", this);
        _prefab = null;
    }

    protected virtual void OnValidate()
    {
        ValidatePrefab();
    }

    public void ClearPool()
    {
        if (_pool == null)
        {
            return;
        }
        _pool.Clear();
        _pool = null;
    }

    private void OnDestroy()
    {
        ClearPool();
    }
}
