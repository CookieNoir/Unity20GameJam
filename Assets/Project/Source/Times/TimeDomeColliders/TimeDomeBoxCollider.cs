using UnityEngine;

public class TimeDomeBoxCollider : TimeDomeCollider
{
    [SerializeField] private BoxCollider _boxCollider;

    public override bool IsFullyInside(TimeDome timeDome)
    {
        return timeDome.ContainsBox(_boxCollider);
    }
}
