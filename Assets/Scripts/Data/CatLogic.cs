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
    private int baseExp = 50; // 基础经验值
    private float expGrowthRate = 2f; // 经验增长率

    private int baseEatFish = 5; // 基础吃鱼速度
    private float eatFishGrowthRate = 1f; // 吃鱼速度增长率

    private int baseCatchFish = 2; // 基础抓鱼速度
    private float catchFishGrowthRate = 1f; // 抓鱼速度增长率

    public CatLogic(Cat data)
    {
        CatData = data;

        Level = new ObservableProperty<int>(CatData.level);
        CurrentExp = new ObservableProperty<long>(CatData.currentExp);
    }

    public long MaxExp => (long)(baseExp * Level.Value * Mathf.Pow(expGrowthRate, Level.Value / 10));
    public int EatFishPerMin => Mathf.RoundToInt(Level.Value * eatFishGrowthRate * baseEatFish);
    public int CatchFishPerSec => Mathf.RoundToInt(baseCatchFish * Mathf.Pow(catchFishGrowthRate, Level.Value - 1));

    // 普通吃鱼增加经验值
    public void AddExp()
    {
        // 小猫吃鱼
        bool enoughFish = CatData.has_fish >= EatFishPerMin;
        // 判断小猫当前鱼的数量是否足够吃，如果不足，则仅吃目前有的
        if (enoughFish)
        {
            bool canLevelUp = CurrentExp.Value + EatFishPerMin >= MaxExp;
            if (canLevelUp)
            {
                CatData.has_fish -= MaxExp - CurrentExp.Value;
                CurrentExp.Value = MaxExp; // 先满经验
                Level.Value += 1;
                CurrentExp.Value = 0;

                Debug.Log("小猫升级了，序号为：" + CatData.cat_id);
            }
            else
            {
                CatData.has_fish -= EatFishPerMin;
                CurrentExp.Value += EatFishPerMin;
            }
        }
        else
        {
            bool canLevelUp = CurrentExp.Value + CatData.has_fish >= MaxExp;
            if (canLevelUp)
            {
                CatData.has_fish -= MaxExp - CurrentExp.Value;
                CurrentExp.Value = MaxExp; // 先满经验
                Level.Value += 1;
                CurrentExp.Value = 0;

                Debug.Log("小猫升级了，序号为：" + CatData.cat_id);
            }
            else
            {
                CurrentExp.Value += CatData.has_fish;
                CatData.has_fish = 0;
            }
        }
    }

    // 一次性增加经验，可一次升多级
    public void AddExp(long fishAmount)
    {
        CurrentExp.Value += fishAmount;

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