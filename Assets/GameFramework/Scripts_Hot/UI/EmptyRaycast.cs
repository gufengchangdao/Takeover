using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Hot
{
    /// <summary>
    /// 空白点击区域，省去着色过程
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    [DisallowMultipleComponent]
    public class EmptyRaycast : Graphic
    {
        protected EmptyRaycast()
        {
            useLegacyMeshGeneration = false;
        }

        public override Material material => null;
        public override Texture mainTexture => null;

        protected override void OnPopulateMesh(VertexHelper vh) => vh.Clear();
        public override void SetMaterialDirty() { }
        public override void SetVerticesDirty() { }
    }
}