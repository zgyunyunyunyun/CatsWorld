using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Linq;

[Obfuz.ObfuzIgnore(Obfuz.ObfuzScope.TypeName)]
public class GameProcedure : ProcedureBase
{
    private LevelEntityBase<SlotEntityBase, FishPoolEntityBase> m_Level;
    private IFsm<IProcedureManager> procedure;

    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        this.procedure = procedureOwner;

        if (GF.Base.IsGamePaused)
        {
            GF.Base.ResumeGame();
        }

        GF.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
        GF.Event.Subscribe(CloseUIFormCompleteEventArgs.EventId, OnCloseUIForm);
        GF.Event.Subscribe(GameplayEventArgs.EventId, OnGameplayEvent);

        if (m_Level != null)
        {
            // GF.Entity.HideEntity(m_Level.Id);
        }

        ShowLevel();
        procedureOwner.RemoveData("LevelEntity");
    }


    protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

    }
    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        if (GF.Base.IsGamePaused)
        {
            GF.Base.ResumeGame();
        }
        GF.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
        GF.Event.Unsubscribe(CloseUIFormCompleteEventArgs.EventId, OnCloseUIForm);
        GF.Event.Unsubscribe(GameplayEventArgs.EventId, OnGameplayEvent);
        base.OnLeave(procedureOwner, isShutdown);
    }

    public async void ShowLevel()
    {
        m_Level = null;
        if (GF.Base.IsGamePaused)
        {
            GF.Base.ResumeGame();
        }
        GF.UI.CloseAllLoadingUIForms();
        GF.UI.CloseAllLoadedUIForms();
        GF.Entity.HideAllLoadingEntities();
        GF.Entity.HideAllLoadedEntities();

        //异步打开主菜单UI
        // menuUIFormId = GF.UI.OpenUIForm(UIViews.MenuUIForm);

        //动态创建关卡
        var lvTb = GF.DataTable.GetDataTable<LevelTable>();

        var playerMd = GF.DataModel.GetOrCreate<PlayerDataModel>();
        int currentLevelId = playerMd.LevelId;

        Log.Warning("存档数据: " + playerMd);
        Log.Warning("当前关卡ID: " + currentLevelId);

        var lvRow = lvTb.GetDataRow(currentLevelId);
        // var playerMd = GF.DataModel.GetOrCreate<PlayerDataModel>();
        // var lvRow = lvTb.GetDataRow(currentLevelId);

        var lvParams = EntityParams.Create(Vector3.zero, Vector3.zero, Vector3.one);
        lvParams.Set(LevelEntityBase<SlotEntityBase, FishPoolEntityBase>.P_LevelData, lvRow);

        // 关卡配置表增加 EntityClassName 字段，填写完整类名（如 "Namespace.LevelEntity"）
        string entityClassName = lvRow.LvEntityName;

        // 根据关卡配置的实体类决定使用哪个关卡实体类
        switch (entityClassName)
        {
            case "LevelEntity":
                m_Level = await GF.Entity.ShowEntityAwait<LevelEntity>(lvRow.LvPfbName, Const.EntityGroup.Level, lvParams) as LevelEntity;
                break;
            // 可以添加更多关卡
            default:
                // 默认使用基本的LevelEntity
                m_Level = await GF.Entity.ShowEntityAwait<LevelEntityBase<SlotEntityBase, FishPoolEntityBase>>(lvRow.LvPfbName, Const.EntityGroup.Level, lvParams) as LevelEntityBase<SlotEntityBase, FishPoolEntityBase>;
                break;
        }
        GF.BuiltinView.HideLoadingProgress();
    }


    public void Restart()
    {
        ChangeState<MenuProcedure>(procedure);
    }
    public void BackHome()
    {
        ChangeState<MenuProcedure>(procedure);
    }

    private void OnGameplayEvent(object sender, GameEventArgs e)
    {
        var args = e as GameplayEventArgs;
        if (args.EventType == GameplayEventType.GameOver)
        {
            OnGameOver(args.Params.Get<VarBoolean>("IsWin"));
        }
    }
    private void OnGameOver(bool isWin)
    {
        Log.Info("Game Over, isWin:{0}", isWin);
        procedure.SetData<VarBoolean>("IsWin", isWin);
        ChangeState<GameOverProcedure>(procedure);
    }
    private void CheckGamePause()
    {

    }
    private void OnCloseUIForm(object sender, GameEventArgs e)
    {
        CheckGamePause();
    }

    private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
    {
        var args = e as OpenUIFormSuccessEventArgs;
        CheckGamePause();
    }
}
