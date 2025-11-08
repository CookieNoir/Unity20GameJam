public class ElectricParticlePool : ComponentPool<ElectricParticle>, IPoolReleaser<ElectricParticle>
{
    protected override void OnGetInstance(ElectricParticle component)
    {
        if (component == null)
        {
            return;
        }
        base.OnGetInstance(component);
        component.SetPool(this);
    }

    protected override void OnReleaseInstance(ElectricParticle component)
    {
        if (component == null)
        {
            return;
        }
        component.SetPool(null);
        base.OnReleaseInstance(component);
    }
}
