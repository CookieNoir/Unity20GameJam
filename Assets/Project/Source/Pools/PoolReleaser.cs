public interface IPoolReleaser<T>
{
    public void Release(T releasable);
}
