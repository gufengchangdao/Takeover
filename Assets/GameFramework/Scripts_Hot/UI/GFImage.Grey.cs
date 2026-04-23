using GameFramework.AOT;
using UnityEngine;

namespace GameFramework.Hot
{
    public partial class GFImage
    {
        private static Material greyMatRes;
        private Material greyMat;
        private Material oldMat;

        public bool Gray => greyMat != null && material == greyMat;

        public void SetGray(bool isGray, bool allChildImage)
        {
            GFImage[] images = null;
            if (allChildImage)
                images = gameObject.GetComponentsInChildren<GFImage>();
            SetGray(isGray, images);
        }

        public void SetGray(bool isGray, params GFImage[] images)
        {
            if (isGray && !greyMatRes)
            {
                if (!GFGlobal.Resource)
                {
                    Log.Error("GFGlobal.Resource不存在，无法加载材质资源！");
                    return;
                }
                greyMatRes = GFGlobal.Resource.LoadAssetSync<Material>("Assets/Content/UI/Material/CommonUIMat.mat");
                if (!greyMatRes)
                {
                    Log.Error("图片置灰失败，请检查置灰材质路径是否正确！");
                    return;
                }
            }
            SetGrayImpl(isGray);

            if (images != null)
                foreach (var image in images)
                {
                    if (image != this)
                        image.SetGrayImpl(isGray);
                }
        }

        private void SetGrayImpl(bool isGray)
        {
            if (Gray == isGray)
                return;

            if (isGray)
            {
                oldMat = material;
                if (!greyMat) //如果外部修改了材质
                {
                    greyMat = new Material(greyMatRes);
                    greyMat.SetFloat("_GreyscaleBlend", 1);
                }
                material = greyMat;
            }
            else
            {
                if (material == greyMat) //防止置灰期间材质被修改
                    material = oldMat;
                if (greyMat)
                    Destroy(greyMat);
                oldMat = null;
                greyMat = null;
            }
        }
    }
}