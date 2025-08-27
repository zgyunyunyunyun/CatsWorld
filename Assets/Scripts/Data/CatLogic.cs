using System;
using UnityEngine;

/// <summary>
/// 处理小猫逻辑的类
/// </summary>
public class CatLogic
{
    public Cat CatData { get; private set; } // 基础猫咪数据

    // 可监听属性
    public ObservableProperty<int> Level { get; private set; }
    public ObservableProperty<long> CurrentExp { get; private set; }

    // 公式参数（可以配置到全局表里）
    private int baseExp = 100; // 基础经验值
    private float expGrowthRate = 1.2f; // 经验增长率

    private int baseEatFish = 10; // 基础吃鱼速度
    private float eatFishGrowthRate = 1.15f; // 吃鱼速度增长率

    private int baseCatchFish = 2; // 基础抓鱼速度
    private float catchFishGrowthRate = 1.1f; // 抓鱼速度增长率

    public CatLogic(Cat data)
    {
        CatData = data;

        Level = new ObservableProperty<int>(CatData.level);
        CurrentExp = new ObservableProperty<long>(CatData.currentExp);
    }

    public long MaxExp => (long)Math.Round(baseExp * Mathf.Pow(expGrowthRate, Level.Value - 1));
    public int EatFishPerMin => Mathf.RoundToInt(baseEatFish * Mathf.Pow(eatFishGrowthRate, Level.Value - 1));
    public int CatchFishPerSec => Mathf.RoundToInt(baseCatchFish * Mathf.Pow(catchFishGrowthRate, Level.Value - 1));

    // 增加经验值
    public void AddExp(long amount)
    {
        CurrentExp.Value += amount;

        long maxExp = MaxExp;
        while (CurrentExp.Value >= maxExp)
        {
            CurrentExp.Value -= maxExp;
            Level.Value++;
            maxExp = MaxExp; // 升级后重新计算
        }
    }


    // 转回基础数据
    public Cat ToData()
    {
        CatData.level = Level.Value;
        CatData.currentExp = CurrentExp.Value;
        return CatData;
    }
}