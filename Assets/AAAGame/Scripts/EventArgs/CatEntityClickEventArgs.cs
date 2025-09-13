using GameFramework.Event;
using GameFramework;

public class CatEntityClickEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(CatEntityClickEventArgs).GetHashCode();
    public override int Id => EventId;
    public RefParams Params { get; private set; }
    public override void Clear()
    {
        if (Params != null)
        {
            ReferencePool.Release(Params);
        }
    }

    public static CatEntityClickEventArgs Create(RefParams eventData = null)
    {
        var instance = ReferencePool.Acquire<CatEntityClickEventArgs>();
        instance.Params = eventData;
        return instance;
    }
}
