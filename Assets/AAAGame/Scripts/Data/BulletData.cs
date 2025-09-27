public class BulletData
{
    public int id;
    public string name;
    public float speed;

    // 其他子弹属性

    public BulletData(BulletTable drBullet)
    {
        id = drBullet.Id;
        speed = drBullet.Speed;
    }
}