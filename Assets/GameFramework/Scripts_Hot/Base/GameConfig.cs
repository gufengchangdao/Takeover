using UnityEngine;
using YooAsset;

namespace GameFramework.Hot
{
    [CreateAssetMenu(menuName = "Config/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public const string DefaultPackage = "DefaultPackage";

        public string datatableAssetPath = "Assets/Content/DataTable";
        public string inputActionAssetPath = "Assets/Content/Config/PlayerInputAsset.inputactions";
    }
}