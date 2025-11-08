using UnityEngine;

public abstract class TimeDomeCollider : MonoBehaviour
{
    public abstract bool IsFullyInside(TimeDome timeDome);
}
