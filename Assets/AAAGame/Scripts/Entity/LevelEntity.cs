using GameFramework;
using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;

public class LevelEntity : EntityBase
{
    public const string P_LevelData = "LevelData";
    public const string P_LevelReadyCallback = "OnLevelReady";
    public bool IsAllReady { get; private set; }

    private List<Cat> catCards = new(); // 关卡内所有猫咪卡片数据
    private SlotEntity m_SlotEntity; // 槽位实体

    private Vector3 m_StartPos; // 猫咪堆叠的起始位置
    private float[][] layers; // 每一层的行数、列数，及相对下面一层的x、y偏移
    private int repeatCount; // 每种猫咪的重复数量
    private List<int> catEntityIds = new List<int>();  // 存储已创建的Cat实体ID


    private HashSet<int> m_EntityLoadingList;
    private bool m_IsGameOver;
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        m_EntityLoadingList = new HashSet<int>();

    }
    protected override async void OnShow(object userData)
    {
        base.OnShow(userData);

        GF.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
        GF.Event.Subscribe(HideEntityCompleteEventArgs.EventId, OnHideEntityComplete);
        GF.Event.Subscribe(CatEntityClickEventArgs.EventId, OnCatEntityClick);
        GF.Event.Subscribe(CatMergeEventArgs.EventId, OnCatMerge);

        IsAllReady = false;
        m_IsGameOver = false;
        m_EntityLoadingList.Clear();
        m_SlotEntity = null;
        catCards.Clear();
        catEntityIds.Clear();
        m_StartPos = Vector3.zero;


        m_StartPos = this.CachedTransform.Find("StartPoint").position;
        LevelTable levelTable = Params.Get(P_LevelData) as LevelTable;
        Log.Warning(levelTable.SlotCount);

        layers = levelTable.Layers;
        repeatCount = levelTable.RepeatCount;

        // // 初始化游戏背景、槽位等
        // var bgParams = UIParams.Create();
        // bgParams.Set<VarInt32>(GameBg.P_SlotCount, levelTable.SlotCount);
        // // bgParams.Set<VarAction>(GameBg.P_SlotInitCallback, (Action)GetSlotPoints);
        // m_GameBgUIForm = await GF.UI.OpenUIFormAwait(UIViews.GameBg, bgParams) as GameBg;

        GetStartPoint();
        InitLevel();

        // GetSlotPoints();

        // 创建槽位
        var slotParams = EntityParams.Create();
        slotParams.AttachToEntity = this.Entity;
        slotParams.ParentTransform = this.CachedTransform.Find("SlotPoint");
        slotParams.localPosition = Vector3.zero;
        slotParams.Set<VarInt32>(SlotEntity.P_MaxSlots, levelTable.SlotCount);
        m_SlotEntity = await GF.Entity.ShowEntityAwait<SlotEntity>("Slot_1", Const.EntityGroup.Level, slotParams) as SlotEntity;
    }

    private void GetStartPoint()
    {
        // m_StartPos = m_GameBgUIForm.GetCatPileStartPoint();
        m_StartPos = this.CachedTransform.Find("StartPoint").position;
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
    }
    protected override void OnHide(bool isShutdown, object userData)
    {
        GF.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
        GF.Event.Unsubscribe(HideEntityCompleteEventArgs.EventId, OnHideEntityComplete);
        GF.Event.Unsubscribe(CatEntityClickEventArgs.EventId, OnCatEntityClick);
        GF.Event.Unsubscribe(CatMergeEventArgs.EventId, OnCatMerge);

        base.OnHide(isShutdown, userData);
    }

    public async void InitLevel()
    {
        catCards.Clear();
        // TODO:获取小猫卡片数据
        //动态创建关卡
        var catTb = GF.DataTable.GetDataTable<CatTable>();
        List<CatData> catTypes = new();
        foreach (var row in catTb.GetAllDataRows())
        {
            catTypes.Add(new CatData(row));
        }

        // 1. 构建元素池
        List<int> deck = new List<int>();
        foreach (var type in catTypes)
        {
            for (int i = 0; i < repeatCount; i++)
                deck.Add(type.id);
        }
        Shuffle(deck);

        int deckIndex = 0;

        // 2. 按层摆放
        for (int layer = 0; layer < layers.Length; layer++)
        {
            var l = layers[layer];
            for (int x = 0; x < l[0]; x++)
            {
                for (int y = 0; y < l[1]; y++)
                {
                    if (deckIndex >= deck.Count) break;

                    int type = deck[deckIndex++];

                    var cat = new Cat(catTypes.Find(c => c.id == type));
                    catCards.Add(cat);

                    var catParams = EntityParams.Create();
                    // 设置猫咪位置，同一层的x和y间距为0，往上一层的起始位置为下面一层的偏移，其中l[2]为x方向的偏移，l[3]为y方向的偏移，间距依然为0
                    // z轴方向为负数，层数越高z越小，保证层数高的在上面
                    // catParams.position = m_StartPos + new Vector3(x * 1.5f + layer * l[2] + layer * 0.08f, y * -1.5f - layer * l[3], 0);
                    catParams.position = m_StartPos + new Vector3(x * 2f + layer * l[2] + layer * 0.08f, y * -2f - layer * l[3], -layer * 0.01f);
                    catParams.Set(CatEntity.P_CatData, cat);
                    catParams.Set<VarInt32>(CatEntity.P_SortOrder, layer);
                    CatEntity catEntity = await GF.Entity.ShowEntityAwait<CatEntity>("CatEntity", Const.EntityGroup.Player, catParams) as CatEntity;
                    catEntity.CachedTransform.SetAsLastSibling(); // 保证后生成的在最上层
                    catEntityIds.Add(catEntity.Id);
                }
            }
        }
    }

    // 洗牌算法
    private void Shuffle(List<int> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    public void StartGame()
    {
    }

    private void CheckGameOver()
    {

    }

    private void OnCatEntityClick(object sender, GameEventArgs e)
    {
        var catEntity = sender as CatEntity;
        if (catEntity == null) return;

        // 处理猫咪点击事件
        Log.Debug("猫咪点击事件接收到: " + catEntity);

        // 判断被点击的猫咪上面是否有其他猫咪覆盖
        if (IsSelectable(catEntity))
        {
            m_SlotEntity.AddCat(catEntity);
        }
        else
        {
            Log.Debug("猫咪被挡住，无法点击");
            return;
        }
    }

    // 判断猫咪是否可选（没有被上层猫咪挡住）
    private bool IsSelectable(CatEntity cat)
    {
        float catSize = 1.4f; // 猫的尺寸
        foreach (var other in catEntityIds)
        {
            CatEntity otherCatEntity = GF.Entity.GetEntity(other).Logic as CatEntity;
            if (otherCatEntity != null && otherCatEntity.layerOrder == cat.layerOrder + 1)
            {
                Vector2 otherPos = otherCatEntity.CachedTransform.position;
                Vector2 catPos = cat.CachedTransform.position;

                // 计算两个猫的包围盒
                float half = catSize / 2f;
                Rect catRect = new(catPos.x - half, catPos.y - half, catSize, catSize);
                Rect otherRect = new(otherPos.x - half, otherPos.y - half, catSize, catSize);

                // 判断包围盒是否重叠
                if (catRect.Overlaps(otherRect))
                {
                    return false; // 被挡住
                }
            }
        }
        return true;
    }

    // 处理猫咪消除事件
    private void OnCatMerge(object sender, GameEventArgs e)
    {
        Log.Debug("触发猫咪合成事件");
        var eArgs = e as CatMergeEventArgs;
        List<CatEntity> mergedCats = eArgs.MergedCats;
        if (mergedCats == null || mergedCats.Count < 3) return;

        Dictionary<int, int> counts = new Dictionary<int, int>();
        List<int> result = new List<int>();

        foreach (int num in mergedCats.Select(c => c.GetCatId()))
        {
            if (!counts.ContainsKey(num))
                counts[num] = 0;

            counts[num]++;

            // 刚好到 3 次就加入结果
            if (counts[num] == 3)
            {
                result.Add(num);
            }
        }

        m_SlotEntity.RemoveMergedCats(mergedCats.Where(c => result.Contains(c.GetCatId())).ToList());
        catEntityIds.RemoveAll(id => mergedCats.Any(c => c.Id == id));
        mergedCats.RemoveAll(c => result.Contains(c.GetCatId()));

        if (mergedCats.Count >= m_SlotEntity.GetMaxSlots())
        {
            // 游戏结束
            GF.Event.Fire(this, GameplayEventArgs.Create(GameplayEventType.GameOver));
            return;
        }
    }

    private void OnShowEntitySuccess(object sender, GameEventArgs e)
    {
        var eArgs = e as ShowEntitySuccessEventArgs;
        int entityId = eArgs.Entity.Id;
        if (m_EntityLoadingList.Contains(entityId))
        {
            m_EntityLoadingList.Remove(entityId);
        }
    }


    private void OnHideEntityComplete(object sender, GameEventArgs e)
    {
        var eArgs = e as HideEntityCompleteEventArgs;
        int entityId = eArgs.EntityId;
        // if (m_Enemies.ContainsKey(entityId))
        // {
        //     m_Enemies.Remove(entityId);
        // }
        // else if (m_EntityLoadingList.Contains(entityId))
        // {
        //     m_EntityLoadingList.Remove(entityId);
        // }

        CheckGameOver();
    }
}
