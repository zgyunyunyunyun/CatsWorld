using DG.Tweening;
using UnityEngine;
using UnityGameFramework.Runtime;

public class FishEntity : EntityBase
{
    public const string P_FishData = "FishData";
    public const string P_SortOrder = "SortOrder";

    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    private Fish fish;
    // public int layerOrder = 0; // 渲染层级

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
    }
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        fish = Params.Get(P_FishData) as Fish;
        m_SpriteRenderer.SetSprite(fish.fishData.fishIcon);
        // layerOrder = Params.Get<VarInt32>(P_SortOrder).Value;
        // m_SpriteRenderer.sortingOrder = layerOrder;

        MoveDown();

    }
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
    }

    protected override void OnHide(bool isShutdown, object userData)
    {
        base.OnHide(isShutdown, userData);
    }

    // 鱼向下游动
    public Tween MoveDown(float distance = 14f, float duration = 10f)
    {
        Vector3 target = CachedTransform.position + new Vector3(0, -distance, 0);
        return CachedTransform.DOMove(target, duration).SetEase(Ease.Flash);
    }

    // 获取鱼类数据
    public Fish GetFishData()
    {
        return fish;
    }
    public int GetFishId()
    {
        return fish.fishData.id;
    }
}