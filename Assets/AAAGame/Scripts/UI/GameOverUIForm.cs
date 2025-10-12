using DG.Tweening;
using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
[Obfuz.ObfuzIgnore(Obfuz.ObfuzScope.TypeName)]
public partial class GameOverUIForm : UIFormBase
{
    public const string P_IsWin = "IsWin";

    private bool isWin;
    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);

        isWin = Params.Get<VarBoolean>(P_IsWin);
        varTitleTxt.text = isWin ? GF.Localization.GetString("Victory") : GF.Localization.GetString("Failed");

        varNextLevelBtn.gameObject.SetActive(isWin);
        varRestartBtn.gameObject.SetActive(!isWin);
    }
    protected override void OnButtonClick(object sender, Button btSelf)
    {
        Log.Warning("OnButtonClick");
        base.OnButtonClick(sender, btSelf);
        if (btSelf == varBackBtn)
        {
            //(GF.Procedure.CurrentProcedure as GameOverProcedure).BackHome();
        }

        if (btSelf == varRestartBtn)
        {
            (GF.Procedure.CurrentProcedure as GameOverProcedure).Restart();
        }

        if (btSelf == varNextLevelBtn)
        {
            (GF.Procedure.CurrentProcedure as GameOverProcedure).NextLevel();
        }
    }
}
