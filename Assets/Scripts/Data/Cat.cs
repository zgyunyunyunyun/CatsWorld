using System;

[Serializable]
public class Cat
{
    public int cat_id;//小猫的id，从0开始

    public int cat_icon;//小猫头像

    public string cat_name;//小猫名称

    public string introuction;//小猫简介

    //public string work;//工作状态：炼丹中、探索中、空闲中

    public string big_level;//大境界
    public int small_level;//小境界

    public float cultivation;//修为

    public bool canUp;//小猫是否能升级

    public int lingshi_consume;//灵石消耗速度x/s

    public int had_stone;//拥有的灵石数量

    /*
    public static implicit operator GameObject(Cat v)
    {
        throw new NotImplementedException();
    }
    */

    // 250824需求
    public int level; // 小猫等级
    public long currentExp; // 小猫经验值
    public long has_fish; // 拥有的鱼
}