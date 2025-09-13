using GameFramework.Event;
using GameFramework;
using System.Collections.Generic;

public class CatMergeEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(CatMergeEventArgs).GetHashCode();
    public override int Id => EventId;
    public List<CatEntity> MergedCats { get; private set; }
    public RefParams Params { get; private set; }
    public override void Clear()
    {
        if (Params != null)
        {
            ReferencePool.Release(Params);
        }
    }

    public static CatMergeEventArgs Create(List<CatEntity> mergedCats, RefParams eventData = null)
    {
        var instance = ReferencePool.Acquire<CatMergeEventArgs>();
        instance.MergedCats = mergedCats;
        instance.Params = eventData;
        return instance;
    }
}
