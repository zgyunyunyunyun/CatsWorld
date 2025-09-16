using GameFramework.Event;
using GameFramework;
using System.Collections.Generic;

public class CatMergeCheckEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(CatMergeCheckEventArgs).GetHashCode();
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

    public static CatMergeCheckEventArgs Create(List<CatEntity> mergedCats, RefParams eventData = null)
    {
        var instance = ReferencePool.Acquire<CatMergeCheckEventArgs>();
        instance.MergedCats = mergedCats;
        instance.Params = eventData;
        return instance;
    }
}
