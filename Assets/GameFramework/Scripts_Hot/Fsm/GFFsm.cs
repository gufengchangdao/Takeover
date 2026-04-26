using System.Collections.Generic;
using GameFramework.AOT;
using UnityEngine;

namespace GameFramework.Hot
{
    public class GFFsm : GFBaseModule
    {
        private List<FsmBase> fsms = new();
        private int traverseCounter = 0;
        private HashSet<FsmBase> waitRemoveFsms = new();

        public override void ModuleUpdate()
        {
            base.ModuleUpdate();
            traverseCounter++;
            int i = 0;
            while (i < fsms.Count) //用while允许遍历时新增
            {
                try
                {
                    if (!waitRemoveFsms.Contains(fsms[i]))
                        fsms[i].Update(Time.deltaTime, Time.unscaledDeltaTime);
                }
                catch (System.Exception e)
                {
                    Log.Error($"[Fsm] {e.Message}");
                }
                i++;
            }
            traverseCounter--;

            TryRemoveFsms();
        }

        public Fsm<T> CreateFsm<T>(T owner, params FsmState<T>[] states) where T : class
        {
            var fsm = Fsm<T>.Create(owner, states);
            fsms.Add(fsm);
            return fsm;
        }

        public void DestroyFsm(FsmBase fsm)
        {
            if (fsm == null) return;

            waitRemoveFsms.Add(fsm);
            TryRemoveFsms();
        }

        private void TryRemoveFsms()
        {
            if (traverseCounter > 0)
                return;

            foreach (var fsm in waitRemoveFsms)
            {
                if (fsms.Remove(fsm))
                    fsm.Shutdown();
            }
            waitRemoveFsms.Clear();
        }
    }
}