using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityGameFramework.Runtime;

public class SlotEntity : SlotEntityBase
{
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
    }

    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
    }

    public void Test()
    {
        Log.Debug("SlotEntity Test");
    }
}