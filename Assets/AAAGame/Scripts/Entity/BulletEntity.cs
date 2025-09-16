using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

public class BulletEntity : EntityBase
{
    public const string P_Speed = "Speed";
    public const string P_TargetFish = "TargetFish";
    public const string P_BulletData = "BulletData";
    private float speed;
    private FishEntity targetFish;

    private Bullet bullet;

    private bool hasHit = false;

    private Tween moveTween;

    // Rigidbody m_body;
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
    }
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);

        speed = Params.Get<VarFloat>(P_Speed).Value;
        targetFish = Params.Get(P_TargetFish) as FishEntity;
        bullet = Params.Get(P_BulletData) as Bullet;
        hasHit = false;

        // 计算目标点
        if (targetFish != null)
        {
            Vector3 targetPos = targetFish.CachedTransform.position;
            float distance = Vector3.Distance(CachedTransform.position, targetPos);
            float duration = distance / speed; // 按速度计算飞行时间

            moveTween = CachedTransform.DOMove(targetPos, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                // 可在这里回收子弹或触发命中逻辑
                // GF.Entity.HideEntity(Entity);
            });
        }
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
        // if (targetFish == null) return;


        // Vector3 dir = (targetFish.CachedTransform.position - CachedTransform.position).normalized;
        // CachedTransform.position += elapseSeconds * speed * dir;
    }

    // 在 OnHide 或回收时 Kill 动画
    protected override void OnHide(bool isShutdown, object userData)
    {
        moveTween?.Kill();
        base.OnHide(isShutdown, userData);
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
