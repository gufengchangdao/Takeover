using GameFramework.AOT;

namespace GameFramework.Hot
{
    public class GFProcedure : GFBaseModule
    {
        private Fsm<GFProcedure> fsm;

        public ProcedureBase Current
        {
            get
            {
                if (fsm == null)
                    return null;

                return fsm.CurrentState as ProcedureBase;
            }
        }

        // 等业务逻辑程序集初始化流程
        public void Init(params ProcedureBase[] procedures)
        {
            fsm = GFGlobal.Fsm.CreateFsm(this, procedures);
        }

        public void ChangeState<T>(object userData = null) where T : ProcedureBase
        {
            Log.Info("[Procedure] start the first procedure : {0}", typeof(T));
            fsm.ChangeState<T>(userData);
        }

        public T GetProcedure<T>() where T : ProcedureBase
        {
            return fsm.GetState(typeof(T)) as T;
        }
    }
}