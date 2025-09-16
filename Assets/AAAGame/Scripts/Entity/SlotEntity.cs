using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityGameFramework.Runtime;

public class SlotEntity : EntityBase
{
    public const string P_MaxSlots = "MaxSlots";
    [SerializeField] private List<SlotItemEntity> slotAnchors; // 元素放置位置
    [SerializeField] private int currentIndex = 0; // 当前槽位索引
    [SerializeField] private int maxSlots; // 最大槽位数量
    [SerializeField] private List<CatEntity> slotCats = new(); // 槽位内的猫咪实体

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);


    }

    protected override async void OnShow(object userData)
    {
        base.OnShow(userData);

        maxSlots = Params.Get<VarInt32>(P_MaxSlots, 5); // 默认5个槽位

        // 初始化槽位位置列表
        if (slotAnchors == null || slotAnchors.Count == 0)
        {
            slotAnchors = new List<SlotItemEntity>();
            for (int i = 0; i < maxSlots; i++)
            {
                // 创建槽位实体
                // var slotItemParams = EntityParams.Create(transform.position + new Vector3(i * 1.5f, 0, 0), Vector3.zero);
                var slotItemParams = EntityParams.Create();
                slotItemParams.AttachToEntity = this.Entity;
                slotItemParams.ParentTransform = this.CachedTransform.Find("Positions");
                var slotItem = await GF.Entity.ShowEntityAwait<SlotItemEntity>($"SlotItem", Const.EntityGroup.Level, slotItemParams) as SlotItemEntity;
                slotAnchors.Add(slotItem);
            }
        }
    }

    // 获取当前最大槽位数量
    public int GetMaxSlots()
    {
        return maxSlots;
    }

    // 获取当前空闲槽位位置
    public Vector3 GetEmptySlotPos()
    {
        if (HasEmptySlot())
        {
            return slotAnchors[currentIndex].CachedTransform.position;
        }
        return Vector3.zero; // 如果没有空闲槽位，返回零向量
    }

    // 判断是否还有空闲槽位
    public bool HasEmptySlot()
    {
        return currentIndex < maxSlots;
    }

    public void AddCat(CatEntity cat)
    {
        if (HasEmptySlot())
        {
            var tween = cat.MoveTo(GetEmptySlotPos());
            currentIndex++;
            slotCats.Add(cat);

            if (slotCats.Count >= 3)
            {
                // 只在最后一只猫动画完成后再触发合成
                tween.OnComplete(() =>
                {
                    // 触发猫咪合成检查事件
                    GF.Event.Fire(this, CatMergeCheckEventArgs.Create(slotCats));
                });
            }
        }
        else
        {
            Log.Debug("没有空余槽位了！");
            return;
        }
    }

    public void RemoveMergedCats(List<CatEntity> mergedCats)
    {
        // 先让所有猫咪向上飘，再消失
        foreach (var cat in mergedCats)
        {
            slotCats.Remove(cat);
            currentIndex--;
            // 目标位置：当前坐标向上偏移 2 单位（可根据实际调整）
            Vector3 upTarget = cat.Entity.transform.position + new Vector3(0, 2f, 0);
            // 先向上飘 0.5 秒，再缩放消失 0.3 秒
            cat.Entity.transform.DOMove(upTarget, 0.5f).OnComplete(() =>
            {
                cat.Entity.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
                {
                    GF.Entity.HideEntitySafe(cat.Id);
                });
            });
        }

        // 消除小猫后进行攻击
        GF.Event.Fire(this, CatMergeAttackEventArgs.Create(mergedCats));

        // 重新排列剩余的猫咪
        RearrangeCats();
    }

    private void RearrangeCats()
    {
        for (int i = 0; i < slotCats.Count; i++)
        {
            var cat = slotCats[i];
            cat.MoveTo(slotAnchors[i].CachedTransform.position);
        }
    }
}