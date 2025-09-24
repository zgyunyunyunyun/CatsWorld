using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

public class CatEntity : EntityBase
{
    public const string P_CatData = "CatData";
    public const string P_SortOrder = "SortOrder";
    public const string P_ClickAble = "ClickAble";

    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    private Cat cat;
    public int layerOrder = 0; // 渲染层级

    private bool clickAble = false; // 是否已禁用点击

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
    }
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        cat = Params.Get(P_CatData) as Cat;
        m_SpriteRenderer.SetSprite(cat.catData.catIcon);
        layerOrder = Params.Get<VarInt32>(P_SortOrder).Value;
        m_SpriteRenderer.sortingOrder = layerOrder;
        clickAble = Params.Get<VarBoolean>(P_ClickAble, false).Value;
        SetClickAble(clickAble);
        SetClickAbleStyle();
    }
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
    }

    protected override void OnHide(bool isShutdown, object userData)
    {
        base.OnHide(isShutdown, userData);
    }

    // 设置点击状态
    public void SetClickAble(bool able, bool updateStyle = true)
    {
        clickAble = able;
        if (updateStyle)
        {
            SetClickAbleStyle();
        }
    }

    // 设置可点击样式
    public void SetClickAbleStyle()
    {
        m_SpriteRenderer.color = clickAble ? Color.white : Color.gray;
    }

    // 点击事件
    public void OnClick()
    {
        if (!clickAble)
        {
            Log.Debug("当前猫咪不可点击");
            return;
        }
        Log.Debug("CatEntity Pos: " + transform.position);
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Log.Debug("Mouse World Pos: " + mouseWorld);

        // 将猫咪的sortOrder提升到最高，避免被其他猫咪遮挡
        m_SpriteRenderer.sortingOrder = 1000;

        var eParms = RefParams.Create();
        eParms.Set(P_CatData, cat);
        GF.Event.Fire(this, CatEntityClickEventArgs.Create(eParms));
    }

    // 移动到指定位置
    public Tween MoveTo(Vector3 position)
    {
        // 返回 Tween 以便外部监听动画完成
        return CachedTransform.DOMove(position, 0.5f).SetEase(Ease.Linear);
    }

    // 获取猫咪数据
    public Cat GetCatData()
    {
        return cat;
    }
    public int GetCatId()
    {
        return cat.catData.id;
    }
}