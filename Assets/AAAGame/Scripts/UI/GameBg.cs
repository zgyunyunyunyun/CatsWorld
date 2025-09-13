using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

public partial class GameBg : UIFormBase
{
    public const string P_SlotCount = "SlotCount";
    public const string P_SlotInitCallback = "OnSlotInit";
    private List<SlotItem> slotList = new List<SlotItem>();

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
    }

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);

        int count = Params.Get<VarInt32>(P_SlotCount, 5).Value;
        InitSlots(count);
    }

    private void InitSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var slot = SpawnItem<UIItemObject>(varSlotItem, varSlotBg.transform);
            slotList.Add(slot.itemLogic as SlotItem);
        }
    }

    // 获取各个槽位点位置
    public List<Transform> InitSlotPoints()
    {
        var slotPoints = new List<Transform>();
        foreach (var slot in slotList)
        {
            slotPoints.Add(slot.gameObject.transform);
        }
        return slotPoints;
    }

    // 获取小猫堆生成点，即Tile的中点
    public Vector3 GetCatPileStartPoint()
    {
        return varTileBg.transform.position;
    }
}
