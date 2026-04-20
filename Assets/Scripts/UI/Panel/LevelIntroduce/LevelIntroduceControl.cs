using System;
using GameFramework.Hot;
using TableStructure;

namespace Takeover
{
    public class LevelIntroduceControl : BaseControl
    {
        public int Level { get; private set; }

        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            Level = (int)userData;
        }
    }
}