using System;
using GameFramework.AOT;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameFramework.Hot
{
    /// <summary>
    /// 图片组件
    /// 1. 图片切换，从statuses数组里根据字符串切换图片，图片不能太多，因为不用也会保留图片引用
    /// 2. 图片置灰效果
    /// </summary>
    public partial class GFImage : Image
    {
        [SerializeField] private ImageStatus[] statuses;

        private IRequestHandle requestHandle;

        public void SetImageByStatus(string status)
        {
            if (statuses != null)
                for (int i = 0; i < statuses.Length; i++)
                {
                    if (statuses[i].status == status)
                    {
                        sprite = statuses[i].sprite;
                        return;
                    }
                }

            Log.Error("没有找到{0}对象{1}状态对应的图片。", gameObject.name, status);
        }

        private void CancelLoadImage()
        {
            requestHandle?.Cancel();
            requestHandle = null;
        }

        private void OnAtlasLoaded(SpriteAtlas atlas, object userData)
        {
            if (!atlas)
            {
                Log.Error("图集加载失败");
                return;
            }

            string imageName = userData as string;
            Sprite sprite = atlas.GetSprite(imageName);
            if (sprite == null)
            {
                Log.Error("图集{0}里不存在图片{1}", atlas, imageName);
                return;
            }

            this.sprite = sprite;
        }

        /// <summary>
        /// 异步加载图集里的图片
        /// </summary>
        public void SetImageAsync(string atlasPath, string imageName)
        {
            CancelLoadImage();
            requestHandle = GFGlobal.Resource.LoadAssetAsync<SpriteAtlas>(atlasPath, OnAtlasLoaded, imageName);
        }

        protected override void OnDestroy()
        {
            if (greyMat)
                Destroy(greyMat);
            oldMat = null;
            greyMat = null;

            CancelLoadImage();

            base.OnDestroy();
        }


        [Serializable]
        public class ImageStatus
        {
            public string status;
            public Sprite sprite;
        }
    }
}