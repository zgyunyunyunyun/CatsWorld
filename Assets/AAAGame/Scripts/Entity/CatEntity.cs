public class CatEntity : CatEntityBase
{
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
    }
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
    }
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
    }

    protected override void OnHide(bool isShutdown, object userData)
    {
        base.OnHide(isShutdown, userData);
    }
}