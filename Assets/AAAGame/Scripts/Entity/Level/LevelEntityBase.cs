using GameFramework.Event;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;

public class LevelEntityBase<T, U> : EntityBase where T : SlotEntityBase where U : FishPoolEntityBase
{
    public const string P_LevelData = "LevelData";
    public const string P_LevelReadyCallback = "OnLevelReady";
    public bool IsAllReady { get; private set; }

    protected U m_FishPoolEntity; // 鱼池实体
    protected T m_SlotEntity; // 槽位实体

    protected LevelTable levelTable; // 当前关卡数据

    protected List<CatEntity> catCards = new(); // 关卡内所有猫咪卡片数据


    private Vector3 m_StartPos; // 猫咪堆叠的起始位置
    protected int m_SlotCount; // 槽位数量
    protected List<LayerTable> layerConfigs = new(); // 每一层卡片配置
    protected int totalCatsCount; // 猫咪的总数量
    private float m_CatEntitySize = 1f; // 猫咪实体的尺寸（假设为正方形，边长1单位）

    private bool m_IsGameOver;
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
    }
    protected override async void OnShow(object userData)
    {
        base.OnShow(userData);

        GF.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
        GF.Event.Subscribe(HideEntityCompleteEventArgs.EventId, OnHideEntityComplete);
        GF.Event.Subscribe(CatEntityClickEventArgs.EventId, OnCatEntityClick);
        GF.Event.Subscribe(CatMergeCheckEventArgs.EventId, OnCatMergeCheck);
        GF.Event.Subscribe(CatMergeAttackEventArgs.EventId, OnAttack);
        GF.Event.Subscribe(BulletHitEventArgs.EventId, OnBulletHit);
        GF.Event.Subscribe(FishDieEventArgs.EventId, OnFishDie);

        levelTable = null;
        m_FishPoolEntity = null;
        m_SlotEntity = null;
        IsAllReady = false;
        m_IsGameOver = false;
        catCards.Clear();
        layerConfigs.Clear();
        m_StartPos = Vector3.zero;

        levelTable = Params.Get(P_LevelData) as LevelTable;

        // // 初始化游戏背景、槽位等
        // var bgParams = UIParams.Create();
        // bgParams.Set<VarInt32>(GameBg.P_SlotCount, levelTable.SlotCount);
        // // bgParams.Set<VarAction>(GameBg.P_SlotInitCallback, (Action)GetSlotPoints);
        // m_GameBgUIForm = await GF.UI.OpenUIFormAwait(UIViews.GameBg, bgParams) as GameBg;

        if (!InitLevelData())// 初始化关卡配置
        {
            return;
        }

        InitFishPool(); // 创建鱼池
        InitSlot(); // 创建槽位

        GetStartPoint(); // 获取初始位置
        await InitLevelCats(); // 创建消除小猫堆
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
        GF.Event.Unsubscribe(CatMergeCheckEventArgs.EventId, OnCatMergeCheck);
        GF.Event.Unsubscribe(CatMergeAttackEventArgs.EventId, OnAttack);
        GF.Event.Unsubscribe(BulletHitEventArgs.EventId, OnBulletHit);
        GF.Event.Unsubscribe(FishDieEventArgs.EventId, OnFishDie);

        base.OnHide(isShutdown, userData);
    }

    // 创建鱼池
    protected virtual async void InitFishPool()
    {
        // 创建鱼池
        var fishPoolParams = EntityParams.Create();
        fishPoolParams.AttachToEntity = this.Entity;
        fishPoolParams.ParentTransform = this.CachedTransform.Find("FishPoolPoint");
        fishPoolParams.localPosition = Vector3.zero;

        // 调用创建槽位的工厂方法，让子类决定创建什么类型的槽位
        m_FishPoolEntity = await CreateFishPoolEntity(fishPoolParams);
    }

    // 工厂方法，让子类可以重写来返回特定类型的FishPoolEntity
    protected virtual async Task<U> CreateFishPoolEntity(EntityParams fishPoolParams)
    {
        // 基类默认创建FishPoolEntityBase类型
        return await GF.Entity.ShowEntityAwait<U>("FishPool_1", Const.EntityGroup.Level, fishPoolParams) as U;
    }

    // 创建槽位
    protected virtual async void InitSlot()
    {
        // 创建槽位
        var slotParams = EntityParams.Create();
        slotParams.AttachToEntity = this.Entity;
        slotParams.ParentTransform = this.CachedTransform.Find("SlotPoint");
        slotParams.localPosition = Vector3.zero;
        slotParams.Set<VarInt32>(SlotEntity.P_MaxSlots, m_SlotCount);

        // 调用创建槽位的工厂方法，让子类决定创建什么类型的槽位
        m_SlotEntity = await CreateSlotEntity(slotParams);
    }

    // 工厂方法，让子类可以重写来返回特定类型的SlotEntity
    protected virtual async Task<T> CreateSlotEntity(EntityParams slotParams)
    {
        // 基类默认创建SlotEntityBase类型
        return await GF.Entity.ShowEntityAwait<T>("Slot_1", Const.EntityGroup.Level, slotParams) as T;
    }

    // 获取初始位置，后续的猫咪堆叠位置都基于此位置进行计算
    private void GetStartPoint()
    {
        // m_StartPos = m_GameBgUIForm.GetCatPileStartPoint();
        m_StartPos = CachedTransform.Find("TileBg").Find("StartPoint").position;
    }

    // 初始化猫咪堆叠层数和每层行列数
    protected virtual bool InitLevelData()
    {
        m_SlotCount = levelTable.SlotCount;

        var layerIds = levelTable.Layers;
        // 获取层配置
        var layers = GF.DataTable.GetDataTable<LayerTable>();
        for (int i = 0; i < layerIds.Length; i++)
        {
            layerConfigs.Add(layers.GetDataRow(layerIds[i]));
        }

        // 计算所有层配置的小猫总数
        totalCatsCount = layerConfigs.Sum(l => l.PosArr.Length);
        // 判断是否为否为3的倍数
        if (totalCatsCount % 3 != 0)
        {
            Log.Error($"当前关卡配置的小猫总数 {totalCatsCount} 不是3的倍数，请检查关卡配置");
            return false;
        }
        return true;
    }

    // 初始化关卡内的小猫
    public virtual async Task InitLevelCats()
    {
        catCards.Clear();

        // 1. 获取猫咪类型数据
        List<CatData> catTypes = GetAllCatTypes();

        // 2. 构建随机猫咪牌组
        List<int> deck = BuildShuffledDeck(catTypes);

        // 3. 按层摆放猫咪
        await PlaceCatsByLayer(catTypes, deck);

        // 所有猫咪创建完成后，更新每个猫咪的可点击状态
        UpdateAllCatsClickableState();
    }

    // 获取所有猫咪类型数据
    private List<CatData> GetAllCatTypes()
    {
        var catTb = GF.DataTable.GetDataTable<CatTable>();
        List<CatData> catTypes = new();
        foreach (var row in catTb.GetAllDataRows())
        {
            catTypes.Add(new CatData(row));
        }
        return catTypes;
    }

    // 处理均分每种小猫牌组（每种3的倍数），catsCount为总数（也是3的倍数）
    public virtual List<int> BuildCatsDeckBy3(int catsCount, List<CatData> catTypes)
    {
        List<int> deck = new();

        // 根据catsCount计算每种猫咪的数量，确保每种猫咪数量都为3的倍数
        int totalRepeatCount = catsCount / catTypes.Count;
        int minueCount = totalRepeatCount % 3;
        int repeatCount = totalRepeatCount - minueCount;

        // 总数减去所有猫咪重复的次数，计算剩余的数量，随机分配给几种猫咪，确保总数满足要求
        int remainingCount = (catsCount - (repeatCount * catTypes.Count)) / 3;

        foreach (var type in catTypes)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                deck.Add(type.id);
            }
        }

        // 随机分配剩余的猫咪
        for (int i = 0; i < remainingCount; i++)
        {
            int randomIndex = Random.Range(0, catTypes.Count);
            for (int j = 0; j < 3; j++)
            {
                deck.Add(catTypes[randomIndex].id);
            }
        }

        return deck;
    }

    // 构建随机猫咪牌组
    protected virtual List<int> BuildShuffledDeck(List<CatData> catTypes)
    {
        List<int> deck = BuildCatsDeckBy3(totalCatsCount, catTypes);

        Shuffle(deck);
        return deck;
    }

    // 按层摆放猫咪
    protected virtual async Task<int> PlaceCatsByLayer(List<CatData> catTypes, List<int> deck)
    {
        int deckIndex = 0;

        for (int layerIdx = 0; layerIdx < layerConfigs.Count; layerIdx++)
        {
            LayerTable layerConfig = layerConfigs[layerIdx];
            Vector3[] layerCatPosArr = layerConfig.PosArr; // 每层猫咪的位置

            for (int rowIdx = 0; rowIdx < layerCatPosArr.Length; rowIdx++)
            {
                if (deckIndex >= deck.Count) break;
                await CreateCatEntity(catTypes, deck[deckIndex], layerIdx, layerCatPosArr[rowIdx]);
                deckIndex += 1; // 更新索引
            }

            await Task.Yield(); // 创建一层后稍微等待，避免卡顿
        }

        return deckIndex;
    }

    // 创建单个猫咪实体
    protected async Task<CatEntity> CreateCatEntity(List<CatData> catTypes, int catTypeId, int layerIdx,
                                                 Vector3 pos)
    {
        // 根据id找到对应的猫咪数据
        var cat = new Cat(catTypes.Find(c => c.id == catTypeId));

        // 计算猫咪位置
        Vector3 position = m_StartPos + new Vector3(pos.x, pos.y, -(layerConfigs.Count - (layerIdx + 1)) * 0.2f);

        // 创建实体参数
        var catParams = EntityParams.Create();
        catParams.position = position;
        // catParams.AttachToEntity = this.Entity;
        // catParams.ParentTransform = this.CachedTransform.Find("TileBg").Find("CatPile");
        catParams.Set(CatEntity.P_CatData, cat);

        // 设置渲染层级，确保后创建的在上层，但Sorting in Layer不一定能保证点击时在最上面的先被OnMouseDown方法触发，需要结合z值处理
        catParams.Set<VarInt32>(CatEntity.P_SortOrder, layerIdx + 1); // 从1开始，避免0层乘以别的数都为0被盖住

        // 显示实体
        CatEntity catEntity = await GF.Entity.ShowEntityAwait<CatEntity>(cat.catData.prefabName, Const.EntityGroup.Player, catParams) as CatEntity;
        catEntity.CachedTransform.SetAsLastSibling(); // 保证后生成的在最上层
        catCards.Add(catEntity);

        return catEntity;
    }

    // 洗牌算法
    protected virtual void Shuffle(List<int> list)
    {
        System.Random rng = new();
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

    // 处理猫咪点击事件
    protected virtual void OnCatEntityClick(object sender, GameEventArgs e)
    {
        var catEntity = sender as CatEntity;
        if (catEntity == null) return;

        // 处理猫咪点击事件
        Log.Debug("猫咪点击事件接收到: " + catEntity);

        // 判断被点击的猫咪上面是否有其他猫咪覆盖
        if (IsSelectable(catEntity))
        {
            m_SlotEntity.AddCat(catEntity); // 放入槽位
            catCards.Remove(catEntity); // 从当前关卡的猫咪列表中移除

            // 当猫咪被点击并移动到槽位时，更新剩余猫咪的可点击状态
            // 使用延迟调用，确保在猫咪移动动画完成后更新
            StartCoroutine(DelayUpdateClickableState());
        }
        else
        {
            Log.Debug("猫咪被挡住，无法点击");
            return;
        }
    }

    // 延迟更新可点击状态的协程
    private System.Collections.IEnumerator DelayUpdateClickableState()
    {
        // 等待一段时间，确保猫咪移动动画已经完成
        yield return new WaitForSeconds(0.5f);

        // 更新所有猫咪的可点击状态
        UpdateAllCatsClickableState();
    }

    // 判断猫咪是否可选（没有被上层猫咪挡住）
    private bool IsSelectable(CatEntity cat)
    {
        foreach (var other in catCards)
        {
            if (other.GetLayerOrder() >= cat.GetLayerOrder() + 1)
            {
                Vector2 otherPos = other.CachedTransform.position;
                Vector2 catPos = cat.CachedTransform.position;

                // 添加一个小的容差值，避免边缘相接时误判为重叠
                float tolerance = 0.02f;

                // 手动检查两个AABB包围盒是否有实际重叠（考虑容差）
                bool overlapsX = Mathf.Abs(catPos.x - otherPos.x) < m_CatEntitySize - tolerance;
                bool overlapsY = Mathf.Abs(catPos.y - otherPos.y) < m_CatEntitySize - tolerance;

                if (overlapsX && overlapsY)
                {
                    // 记录详细的重叠信息以便调试
                    float xDist = Mathf.Abs(catPos.x - otherPos.x);
                    float yDist = Mathf.Abs(catPos.y - otherPos.y);
                    Log.Debug($"猫咪 {cat.Id}({catPos}) 被猫咪 {other.Id}({otherPos}) 挡住，X距离:{xDist}/{m_CatEntitySize}，Y距离:{yDist}/{m_CatEntitySize}");
                    return false; // 被挡住
                }
            }
        }
        return true;
    }

    // 更新所有猫咪的可点击状态
    protected void UpdateAllCatsClickableState()
    {
        // 检查每只猫咪是否可选，如果可选则设为可点击
        foreach (var cat in catCards)
        {
            bool canSelect = IsSelectable(cat);
            cat.SetClickAble(canSelect);
        }
    }

    // 处理3只小猫时是否可触发消除事件
    protected virtual void OnCatMergeCheck(object sender, GameEventArgs e)
    {
        Log.Debug("触发猫咪合成事件");
        var eArgs = e as CatMergeCheckEventArgs;
        List<CatEntity> slotCats = eArgs.MergedCats;
        if (slotCats == null || slotCats.Count < 3) return;

        Dictionary<int, int> counts = new(); // 记录每种猫咪的数量
        List<int> result = new(); // 记录需要消除的猫咪ID

        // 统计槽位中每种猫咪的数量
        foreach (int num in slotCats.Select(c => c.GetCatId()))
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

        // 消除符合条件的猫咪
        m_SlotEntity.RemoveMergedCats(slotCats.Where(c => result.Contains(c.GetCatId())).ToList());
        slotCats.RemoveAll(c => result.Contains(c.GetCatId()));

        // 猫咪消除后，更新剩余猫咪的可点击状态
        StartCoroutine(DelayUpdateClickableState());

        // 消除后检查是否还有空槽位，没有则游戏结束
        if (slotCats.Count >= m_SlotEntity.GetMaxSlots())
        {
            // 游戏结束
            OnNonMergeCats();
        }
        else
        {
            CheckGameOver();
        }
    }

    // 处理猫咪攻击事件
    private void OnAttack(object sender, GameEventArgs e)
    {
        Log.Debug("触发猫咪攻击事件");
        var eArgs = e as CatMergeAttackEventArgs;
        List<CatEntity> mergedCats = eArgs.MergedCats;
        if (mergedCats == null || mergedCats.Count == 0) return;

        // 计算攻击力
        int attackPower = mergedCats.Sum(c => c.GetCatData().catData.damage) / mergedCats.Count;
        Log.Debug($"攻击力: {attackPower}");

        // 出现子弹
        var bulletTable = GF.DataTable.GetDataTable<BulletTable>();
        BulletTable defaultBullet = bulletTable.GetDataRow(0);
        var bulletParams = EntityParams.Create();
        bulletParams.position = mergedCats[1].CachedTransform.position;
        bulletParams.Set(BulletEntity.P_BulletData, new Bullet(new BulletData(defaultBullet)));
        bulletParams.Set(BulletEntity.P_TargetFish, m_FishPoolEntity.GetNearestFishToDefense(new Vector3(0, 1, 0), 1));
        // 添加鱼池引用，用于子弹在目标消失时重新寻找目标
        bulletParams.Set(BulletEntity.P_FishPoolEntity, m_FishPoolEntity);
        GF.Entity.ShowEntity<BulletEntity>("Bullet", Const.EntityGroup.Bullet, bulletParams);
        // 播放攻击动画或特效（可选）
    }

    // 处理子弹命中事件
    private void OnBulletHit(object sender, GameEventArgs e)
    {
        var eArgs = e as BulletHitEventArgs;
        int bulletId = eArgs.BulletId;
        FishEntity hitFish = eArgs.HitFish;

        if (hitFish != null)
        {
            hitFish.OnHit(bulletId, 1); // 假设子弹伤害为1
        }
    }

    // 处理鱼死亡事件
    private void OnFishDie(object sender, GameEventArgs e)
    {
        var eArgs = e as FishDieEventArgs;
        int bulletId = eArgs.BulletId;
        FishEntity hitFish = eArgs.HitFish;
        if (m_FishPoolEntity != null && hitFish != null)
        {
            m_FishPoolEntity.OnFishDie(hitFish);
        }
    }

    // 检测防线是否有鱼通过
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("鱼碰到防线！");

            FishEntity fish = other.GetComponent<FishEntity>();
            fish?.Die(-1); // -1表示不是被子弹击杀的
        }
    }

    // 处理没有可合成的猫咪时游戏结束
    private void OnNonMergeCats()
    {
        if (m_IsGameOver) return;
        m_IsGameOver = true;
        var eParms = RefParams.Create();
        eParms.Set<VarBoolean>("IsWin", true);
        GF.Event.Fire(GameplayEventArgs.EventId, GameplayEventArgs.Create(GameplayEventType.GameOver, eParms));
    }

    private void CheckGameOver()
    {
        if (m_IsGameOver) return;
        if (catCards.Count < 1 && m_SlotEntity.GetSlotCats().Count < 1)
        {
            m_IsGameOver = true;
            var eParms = RefParams.Create();
            eParms.Set<VarBoolean>("IsWin", true);
            GF.Event.Fire(GameplayEventArgs.EventId, GameplayEventArgs.Create(GameplayEventType.GameOver, eParms));
        }
    }

    private void OnShowEntitySuccess(object sender, GameEventArgs e)
    {
        var eArgs = e as ShowEntitySuccessEventArgs;
        int entityId = eArgs.Entity.Id;
    }


    private void OnHideEntityComplete(object sender, GameEventArgs e)
    {
        var eArgs = e as HideEntityCompleteEventArgs;
        int entityId = eArgs.EntityId;
    }
}
