using UnityEngine;

public class TimeDomeSphereCollider : TimeDomeCollider
{
    [SerializeField] private SphereCollider _sphereCollider;

    public override bool IsFullyInside(TimeDome timeDome)
    {
        return timeDome.ContainsSphere(_sphereCollider);
    }
}
