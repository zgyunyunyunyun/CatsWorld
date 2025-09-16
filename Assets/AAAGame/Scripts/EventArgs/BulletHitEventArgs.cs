using GameFramework.Event;
using GameFramework;

public class BulletHitEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(BulletHitEventArgs).GetHashCode();
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

    public static BulletHitEventArgs Create(int bulletId, FishEntity hitFish, RefParams eventData = null)
    {
        var instance = ReferencePool.Acquire<BulletHitEventArgs>();
        instance.BulletId = bulletId;
        instance.HitFish = hitFish;
        instance.Params = eventData;
        return instance;
    }
}
