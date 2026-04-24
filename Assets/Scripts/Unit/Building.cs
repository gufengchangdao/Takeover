using TableStructure;
using UnityEngine;

namespace Takeover
{
    public class Building : MonoBehaviour
    {
        [SerializeField] private string _tableId;
        public string TableId => _tableId;

        /// <summary>
        /// 是否建造了东西
        /// </summary>
        public bool IsActive => !string.IsNullOrEmpty(TableId);

        public int GoldSpeed { get; private set; }
        public int ManaSpeed { get; private set; }

        public ECamp Camp { get; private set; }
    }
}