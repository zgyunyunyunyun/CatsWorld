using GameFramework.Event;
using GameFramework;

public class FishDieEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(FishDieEventArgs).GetHashCode();
    public override int Id => EventId;
    public int BulletId { get; private set; }
    public FishEntity HitFish { get; private set; }
    public RefParams Params { get; private set; }
    public override void Clear()
    {
        if (Params != null)
        {
            ReferencePool.Release(Params);
        }
    }

    public static FishDieEventArgs Create(int bulletId, FishEntity hitFish, RefParams eventData = null)
    {
        var instance = ReferencePool.Acquire<FishDieEventArgs>();
        instance.BulletId = bulletId;
        instance.HitFish = hitFish;
        instance.Params = eventData;
        return instance;
    }
}
