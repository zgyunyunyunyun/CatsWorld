using DG.Tweening;
using UnityEngine;
using UnityGameFramework.Runtime;

public class BulletEntity : EntityBase
{
    public const string P_TargetFish = "TargetFish";
    public const string P_BulletData = "BulletData";
    public const string P_FishPoolEntity = "FishPoolEntity"; // 添加鱼池实体引用

    private FishEntity targetFish;
    private FishPoolEntity fishPoolEntity; // 鱼池实体引用，用于重新寻找目标

    private Bullet bullet;

    private bool hasHit = false;
    private bool isTargetTracking = true; // 是否正在跟踪目标
    private float checkTargetInterval = 0.5f; // 检查目标是否存在的间隔时间
    private float lastCheckTime = 0f; // 上次检查时间
    private bool isWaiting = false; // 是否正在等待新目标
    private float waitCheckInterval = 1.0f; // 等待状态下检查新目标的间隔

    private Tween moveTween;
    private Tween waitEffectTween; // 等待效果动画    // Rigidbody m_body;
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
    }
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);

        targetFish = Params.Get(P_TargetFish) as FishEntity;
        bullet = Params.Get(P_BulletData) as Bullet;
        fishPoolEntity = Params.Get(P_FishPoolEntity) as FishPoolEntity; // 获取鱼池实体引用
        hasHit = false;
        lastCheckTime = 0f;
        isWaiting = false;

        // 初始方向朝向目标
        if (targetFish != null)
        {
            Vector3 direction = (targetFish.CachedTransform.position - CachedTransform.position).normalized;
            CachedTransform.up = direction; // 假设子弹的右方向是前进方向
        }

        // 开始追踪目标
        MoveToTarget();
    }

    // 向目标移动的方法
    private void MoveToTarget()
    {
        // 停止当前的移动动画和等待效果动画
        moveTween?.Kill();
        waitEffectTween?.Kill();

        // 如果目标不存在或已失效，尝试寻找新目标
        if (targetFish == null || targetFish.gameObject == null || !targetFish.gameObject.activeSelf)
        {
            FindNewTarget();

            // 如果找不到新目标，则停留在原地等待
            if (targetFish == null)
            {
                isTargetTracking = false;
                isWaiting = true;
                Log.Debug("BulletEntity: 未找到目标，停留在原地等待");

                // 添加等待状态的视觉效果（旋转效果）
                StartWaitingEffect();
                return;
            }
        }

        isTargetTracking = true;
        isWaiting = false;

        // 恢复正常外观
        if (GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        // 计算目标点和飞行时间
        Vector3 targetPos = targetFish.CachedTransform.position;
        float targetDistance = Vector3.Distance(CachedTransform.position, targetPos);
        float moveDuration = targetDistance / bullet.bulletData.speed; // 按速度计算飞行时间

        // 更新子弹朝向
        Vector3 direction = (targetPos - CachedTransform.position).normalized;
        CachedTransform.right = direction; // 假设子弹的右方向是前进方向

        // 创建新的移动动画
        moveTween = CachedTransform.DOMove(targetPos, moveDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            // 如果到达目标位置但没有触发碰撞，检查目标是否还存在
            if (!hasHit)
            {
                // 尝试寻找新目标
                MoveToTarget();
            }
        });
    }

    // 寻找新的目标鱼
    private void FindNewTarget()
    {
        if (fishPoolEntity == null)
        {
            // 如果没有鱼池引用，则无法寻找新目标
            Log.Warning("BulletEntity: 无法寻找新目标，缺少FishPoolEntity引用");
            return;
        }

        // 使用FishPoolEntity提供的方法获取最近的鱼
        targetFish = fishPoolEntity.GetNearestFishToDefense(CachedTransform.position, 1);

        if (targetFish != null)
        {
            Log.Debug($"BulletEntity: 找到新目标 {targetFish.Id}");
        }
        else
        {
            Log.Debug("BulletEntity: 未找到新目标");
        }
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        // 如果已经命中，则不需要检查目标
        if (hasHit) return;

        lastCheckTime += elapseSeconds;

        // 如果正在追踪目标，定期检查目标是否有效
        if (isTargetTracking && lastCheckTime >= checkTargetInterval)
        {
            lastCheckTime = 0f;

            // 检查目标是否失效
            if (targetFish == null || targetFish.gameObject == null || !targetFish.gameObject.activeSelf)
            {
                Log.Debug("BulletEntity: 目标失效，重新寻找目标");
                MoveToTarget(); // 重新寻找目标并移动
            }
        }
        // 如果正在等待新目标，定期检查是否有新目标出现
        else if (isWaiting && lastCheckTime >= waitCheckInterval)
        {
            lastCheckTime = 0f;

            // 尝试寻找新目标
            FindNewTarget();

            // 如果找到新目标，则开始移动
            if (targetFish != null)
            {
                Log.Debug("BulletEntity: 等待中发现新目标，开始追踪");
                MoveToTarget();
            }
        }
    }

    // 在 OnHide 或回收时 Kill 动画
    protected override void OnHide(bool isShutdown, object userData)
    {
        moveTween?.Kill();
        waitEffectTween?.Kill();
        base.OnHide(isShutdown, userData);
    }

    // 开始等待状态的视觉效果
    private void StartWaitingEffect()
    {
        // 闪烁效果（如果有SpriteRenderer组件）
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            // 创建颜色闪烁效果，在白色和淡蓝色之间变化
            waitEffectTween = renderer.DOColor(new Color(0.7f, 0.7f, 1f, 0.8f), 0.5f)
                .SetLoops(-1, LoopType.Yoyo) // 无限循环，来回变化
                .SetEase(Ease.InOutSine); // 平滑的颜色变化
        }

        // 旋转效果
        waitEffectTween = CachedTransform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart) // 无限循环旋转
            .SetEase(Ease.Linear); // 匀速旋转
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        if (other.CompareTag("Enemy"))
        {
            hasHit = true;
            GF.Event.Fire(this, BulletHitEventArgs.Create(bullet.bulletData.id, other.GetComponent<FishEntity>()));
            GF.Entity.HideEntitySafe(Id);
        }
    }
}
