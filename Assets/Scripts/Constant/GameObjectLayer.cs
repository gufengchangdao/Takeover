using UnityEngine;

namespace Takeover
{
    public static class GameObjectLayer
    {
        public static int Army = LayerMask.GetMask("Army");
        public static int Unit = LayerMask.GetMask("Unit");
    }
}