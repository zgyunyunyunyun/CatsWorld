public class BulletData
{
    public int id;
    public string name;

    // 其他子弹属性

    public BulletData(BulletTable drBullet)
    {
        id = drBullet.Id;
    }
}