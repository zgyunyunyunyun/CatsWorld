public class FishData
{
    public int id;
    public string fishIcon;

    public int hp;
    // 其他鱼类属性

    public FishData(FishTable drFish)
    {
        id = drFish.Id;
        fishIcon = drFish.Image;
        hp = drFish.HP;
    }
}