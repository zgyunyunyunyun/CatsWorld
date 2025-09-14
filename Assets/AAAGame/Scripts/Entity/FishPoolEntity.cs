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
    private List<Fish> fishes = new(); // 鱼类数据列表

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

        SpawnFishEntities();
    }
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
    }

    protected override void OnHide(bool isShutdown, object userData)
    {
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
        Vector3 randomPos = fishSpawnPoints[Random.Range(0, fishSpawnPoints.Count)].position;
        randomPos.x = Random.Range(-4f, 4f); // 随机生成x轴位置
        return randomPos;
    }

    // 生成鱼实体
    public async void SpawnFishEntities()
    {
        if (fishes == null) fishes = new List<Fish>();

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
            fishes.Add(fish);

            var fishParams = EntityParams.Create();
            fishParams.position = spawnPoint;
            fishParams.Set(FishEntity.P_FishData, fish);
            // fishParams.Set<VarInt32>(FishEntity.P_SortOrder, 1); // 初始层级为1
            FishEntity fishEntity = await GF.Entity.ShowEntityAwait<FishEntity>("FishEntity", Const.EntityGroup.Enemy, fishParams) as FishEntity;
            fishEntity.CachedTransform.SetAsLastSibling(); // 保证后生成的在最上层

            spawned++;
            await Task.Delay(2000); // 每2秒生成一个
        }
    }
}