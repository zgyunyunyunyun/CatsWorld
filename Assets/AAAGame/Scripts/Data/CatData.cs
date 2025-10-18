public class CatData
{
    public int id;
    public string name;
    public string catIcon;
    public int level;

    public int damage;
    public string prefabName;
    // 其他猫咪属性

    public CatData(CatTable drCat)
    {
        id = drCat.Id;
        catIcon = drCat.Image;
        damage = drCat.Damage;
        prefabName = drCat.PrefabName;
    }
}