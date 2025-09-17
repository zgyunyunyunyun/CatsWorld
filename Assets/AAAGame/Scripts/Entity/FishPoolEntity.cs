using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using Random = UnityEngine.Random;

public class FishPoolEntity : EntityBase
{
    public const string P_FishCount = "FishCount";

    private int fishCount; // 鱼的数量
    private List<Transform> fishSpawnPoints = new(); // 鱼类生成点列表
    private List<FishEntity> fishes = new(); // 鱼类数据列表
    private float spawnTimer = 0f; // 生成计时器
    private int spawnedCount = 0; // 已生成的鱼数量
    private float spawnInterval = 3.0f; // 生成间隔(秒)
    private List<FishData> fishTypes; // 鱼类型缓存
    private bool isSpawning = false; // 是否正在生成

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
    }
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);

        // 初始化鱼类生成点
        GetFishSpawnPoints();

        // 获取鱼的数量
        fishCount = Params.Get<VarInt32>(P_FishCount, 50);

        // 初始化鱼类型数据
        InitFishTypes();

        // 重置计时器和计数
        spawnTimer = 0f;
        spawnedCount = 0;
        isSpawning = true;
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        // 使用Update方法基于计时器生成鱼
        if (isSpawning && spawnedCount < fishCount)
        {
            spawnTimer += elapseSeconds;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                SpawnSingleFish();
            }
        }
    }

    protected override void OnHide(bool isShutdown, object userData)
    {
        // 停止生成
        isSpawning = false;

        // 清理资源
        if (fishes != null)
        {
            fishes.Clear();
        }

        if (fishTypes != null)
        {
            fishTypes.Clear();
        }

        base.OnHide(isShutdown, userData);
    }

    // 获取鱼类生成点
    private List<Transform> GetFishSpawnPoints()
    {
        if (fishSpawnPoints == null || fishSpawnPoints.Count == 0)
        {
            var pointsParent = CachedTransform.Find("SpawnPoints");
            if (pointsParent != null)
            {
                foreach (Transform child in pointsParent)
                {
                    fishSpawnPoints.Add(child);
                }
            }
        }
        return fishSpawnPoints;
    }

    // 随机生成与fishSpawnPoints中点位y轴相同的鱼类生成点
    private Vector3 GetRandomFishSpawnPos()
    {
        var spawnTransform = fishSpawnPoints[Random.Range(0, fishSpawnPoints.Count)];
        if (spawnTransform == null)
        {
            return Vector3.zero;
        }
        Vector3 randomPos = spawnTransform.position;
        randomPos.x = Random.Range(-4f, 4f); // 随机生成x轴位置
        return randomPos;
    }

    // 初始化鱼类型数据
    private void InitFishTypes()
    {
        if (fishTypes == null) fishTypes = new();
        else fishTypes.Clear();

        var fishTb = GF.DataTable.GetDataTable<FishTable>();
        foreach (var row in fishTb.GetAllDataRows())
        {
            fishTypes.Add(new FishData(row));
        }
    }

    // 生成单条鱼，使用协程确保实体创建后能添加到列表中
    private async void SpawnSingleFish()
    {
        if (fishes == null) fishes = new();
        if (fishTypes == null || fishTypes.Count == 0) return;

        // 随机选择一个生成点
        var spawnPoint = GetRandomFishSpawnPos();

        // 随机选择一种鱼类
        var fishType = fishTypes[Random.Range(0, fishTypes.Count)];
        var fish = new Fish(fishType);

        var fishParams = EntityParams.Create();
        fishParams.position = spawnPoint;
        fishParams.Set(FishEntity.P_FishData, fish);

        // 使用await方式显示实体并添加到列表
        FishEntity fishEntity = await GF.Entity.ShowEntityAwait<FishEntity>("FishEntity", Const.EntityGroup.Enemy, fishParams) as FishEntity;
        if (fishEntity != null)
        {
            fishes.Add(fishEntity);
            spawnedCount++; // 增加已生成计数
        }
    }

    // 生成鱼实体（异步方法在微信小游戏中不兼容，暂时保留）
    private async void SpawnFishEntities()
    {
        if (fishes == null) fishes = new();

        var fishTb = GF.DataTable.GetDataTable<FishTable>();
        List<FishData> fishTypes = new();
        foreach (var row in fishTb.GetAllDataRows())
        {
            fishTypes.Add(new FishData(row));
        }

        int spawned = 0;
        while (spawned < fishCount)
        {
            // 随机选择一个生成点
            var spawnPoint = GetRandomFishSpawnPos();

            // 随机选择一种鱼类
            var fishType = fishTypes[Random.Range(0, fishTypes.Count)];
            var fish = new Fish(fishType);

            var fishParams = EntityParams.Create();
            fishParams.position = spawnPoint;
            fishParams.Set(FishEntity.P_FishData, fish);
            // fishParams.Set<VarInt32>(FishEntity.P_SortOrder, 1); // 初始层级为1
            FishEntity fishEntity = await GF.Entity.ShowEntityAwait<FishEntity>("FishEntity", Const.EntityGroup.Enemy, fishParams) as FishEntity;

            fishes.Add(fishEntity);

            spawned++;
            await Task.Delay(3000); // 每2秒生成一个
        }
    }

    // 获取当前距离防守位置最近的鱼，支持第二个参数，为防守位置类型defendPosType，0表示一个点，1表示x轴不同y轴相同的线
    public FishEntity GetNearestFishToDefense(Vector3 defensePosition, int defendPosType = 1)
    {
        if (fishes == null || fishes.Count == 0) return null;

        FishEntity nearestFish = null;
        float minDistance = float.MaxValue;

        foreach (var fish in fishes)
        {
            if (fish == null) continue;
            Vector3 fishPos = fish.CachedTransform.position;
            float distance = 0f;

            if (defendPosType == 0)
            {
                // 计算鱼与防守点的距离
                distance = Vector3.Distance(fishPos, defensePosition);
            }
            else if (defendPosType == 1)
            {
                // 计算鱼与防守线的垂直距离（假设防守线为y轴相同的水平线）
                distance = Mathf.Abs(fishPos.y - defensePosition.y);
            }

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestFish = fish;
            }
        }

        return nearestFish;
    }

    // 鱼被击中，移除鱼实体
    public void OnFishDie(FishEntity fish)
    {
        if (fishes.Contains(fish))
        {
            fishes.Remove(fish);
        }
    }
}