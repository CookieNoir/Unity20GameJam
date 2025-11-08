public class ElectricParticlePool : ComponentPool<ElectricParticle>, IPoolReleaser<ElectricParticle>
{
    protected override void OnGetInstance(ElectricParticle component)
    {
        if (component == null)
        {
            return;
        }
        component.ResetState();
        component.SetPool(this);
        base.OnGetInstance(component);
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
