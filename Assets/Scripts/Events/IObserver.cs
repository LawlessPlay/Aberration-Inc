public interface IObserver
{
    public void OnNotify();
    public void OnNotify(ResourceValue resource);
    public void OnNotify(Job job);
}
