using GameFramework.Fsm;
using GameFramework.Procedure;
[Obfuz.ObfuzIgnore(Obfuz.ObfuzScope.TypeName)]
public class MenuProcedure : ProcedureBase
{
    IFsm<IProcedureManager> procedure;

    protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnInit(procedureOwner);
    }

    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);

        procedure = procedureOwner;
        EnterGame(); //加载关卡
    }

    public void EnterGame()
    {
        ChangeState<GameProcedure>(procedure);
    }
}
