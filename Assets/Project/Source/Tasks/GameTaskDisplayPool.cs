public class GameTaskDisplayPool : ComponentPool<GameTaskDisplay>, IPoolReleaser<GameTaskDisplay>
{
    protected override void OnGetInstance(GameTaskDisplay component)
    {
        if (component == null)
        {
            return;
        }
        base.OnGetInstance(component);
        component.SetPool(this);
    }

    protected override void OnReleaseInstance(GameTaskDisplay component)
    {
        if (component == null)
        {
            return;
        }
        component.SetPool(null);
        base.OnReleaseInstance(component);
    }
}
