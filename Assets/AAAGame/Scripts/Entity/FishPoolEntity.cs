using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

public class FishPoolEntity : EntityBase
{
    Button m_Button;
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        m_Button = GetComponent<Button>();
    }
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
    }
    protected void OnUpdate()
    {

    }
}