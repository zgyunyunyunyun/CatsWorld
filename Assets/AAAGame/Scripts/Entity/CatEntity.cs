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

    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    private Cat cat;
    public int layerOrder = 0; // 渲染层级

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

    }
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
    }

    // 点击事件
    void OnMouseDown()
    {
        Log.Debug("CatEntity Pos: " + transform.position);
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Log.Debug("Mouse World Pos: " + mouseWorld);
        Log.Debug(cat.catData.id + " Clicked!");

        var eParms = RefParams.Create();
        eParms.Set(P_CatData, cat);
        GF.Event.Fire(this, CatEntityClickEventArgs.Create(eParms));
    }

    // 获取点击后前往的槽位
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