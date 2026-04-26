using System.Collections.Generic;

namespace Takeover
{
    public partial class Army
    {
        public List<Army> Targets { get; private set; } = new();

        public bool InCombat => Targets.Count > 0;
    }
}