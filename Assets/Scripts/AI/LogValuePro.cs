using System.Collections.Generic;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime.Variables;

namespace Takeover
{
    public class LogValuePro : Action
    {
        public string message;
        public SharedVariable value1;
        public SharedVariable value2;
        public SharedVariable value3;

        public override TaskStatus OnUpdate()
        {
            string s = message;
            bool hasValu1 = !(value1 == null || value1.Scope == SharedVariable.SharingScope.Empty);
            bool hasValu2 = !(value2 == null || value2.Scope == SharedVariable.SharingScope.Empty);
            bool hasValu3 = !(value3 == null || value3.Scope == SharedVariable.SharingScope.Empty);

            if (hasValu1)
            {
                if (hasValu2)
                {
                    if (hasValu3)
                    {
                        s = string.Format(s, GetValueString(value1), GetValueString(value2), GetValueString(value3));
                    }
                    else
                    {
                        s = string.Format(s, GetValueString(value1), GetValueString(value2));
                    }
                }
                else
                {
                    s = string.Format(s, GetValueString(value1));
                }
            }

            GameFramework.AOT.Log.Info(s);
            return TaskStatus.Success;
        }

        private string GetValueString(SharedVariable value)
        {
            object v = value.GetValue();
            if (v is System.Collections.IEnumerable enumerable && v is not string)
            {
                var list = new List<string>();
                foreach (var item in enumerable)
                    list.Add(item?.ToString() ?? "null");
                return string.Join(", ", list);
            }

            return v?.ToString() ?? "null";
        }
    }
}