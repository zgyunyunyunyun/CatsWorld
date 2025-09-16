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

    public virtual Vector3 HitPoint { get => CachedTransform.position + Vector3.down; }

    private bool isDead = false;

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

        isDead = false;

        MoveDown();
    }
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
    }

    protected override void OnHide(bool isShutdown, object userData)
    {
        base.OnHide(isShutdown, userData);
        // Kill 旧动画，防止复用时动画干扰
        CachedTransform.DOKill();
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

    public void OnHit(int bulletId, int damage)
    {
        if (isDead) return; // 已死亡不再处理

        fish.fishData.hp -= damage;

        var hitPoint = HitPoint;
        var bloodFxParms = EntityParams.Create(hitPoint);
        GF.Entity.ShowEffect("Effect/BloodExplosion", bloodFxParms, 1.5f);

        var damageFxParms = EntityParams.Create(hitPoint);
        GF.Entity.ShowPopText(damageFxParms, damage.ToString(), hitPoint + Vector3.down, 0.5f, 7);
        if (fish.fishData.hp <= 0)
        {
            Die(bulletId);
        }
    }

    public void Die(int bulletId)
    {
        if (isDead) return; // 防止重复调用
        isDead = true;

        // 播放死亡动画
        GF.Entity.HideEntitySafe(Id);

        // 通知鱼池该鱼已死亡
        GF.Event.Fire(this, FishDieEventArgs.Create(bulletId, this));

        // 播放特效（爆炸、水花等）
    }
}