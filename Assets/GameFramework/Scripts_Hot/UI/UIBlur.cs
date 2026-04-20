using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Hot
{
    /// <summary>
    /// 简单的全屏模糊，只在OnEnable的时候模糊一次
    /// 理想中的UI模糊为后处理实现模糊效果，可以实时更新被模糊内容，组件挂在到某个UI对象上，该对象前UI对象和游戏内容一块儿被模糊，对象后UI对象正常显示
    /// </summary>
    public class UIBlur : MonoBehaviour
    {
        [SerializeField]
        private float blurRange = 0.79f;
        [SerializeField]
        private int iteration = 2;
        [SerializeField]
        private int downSample = 8;

        private RenderTexture blurTexture;
        private Material blurMaterial;

        void Start()
        {
            blurMaterial = new Material(Shader.Find("Hidden/GaussianBlur"));
            blurMaterial.SetFloat("_BlurRange", blurRange);
            Blur();
        }

        void OnEnable()
        {
            if (blurMaterial != null)
                Blur(); //初始化的时候这里不执行，Start里才去模糊，因为这时候相机渲染不到UI内容
        }

        private void OnDisable()
        {
            Release();
        }

        private void Release()
        {
            if (blurTexture != null)
            {
                RenderTexture.ReleaseTemporary(blurTexture);
                blurTexture = null;
            }
        }

        private void Blur()
        {
            Release();

            var rawImage = gameObject.GFGetOrAddComponent<RawImage>();
            bool rawImageEnabled = rawImage.enabled;
            rawImage.enabled = false; //先禁用掉，不要把RawImage的图像也渲染了

            RenderTexture fullSizeRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            CaptureMainAndUI(fullSizeRT);

            blurMaterial.SetTexture("_MainTex", fullSizeRT);

            int width = Screen.width / downSample;
            int height = Screen.height / downSample;
            RenderTexture buffer1 = RenderTexture.GetTemporary(width, height);
            RenderTexture buffer2 = RenderTexture.GetTemporary(width, height);

            Graphics.Blit(fullSizeRT, buffer1);
            for (int i = 0; i < iteration; i++)
            {
                Graphics.Blit(buffer1, buffer2, blurMaterial, 0);
                Graphics.Blit(buffer2, buffer1, blurMaterial, 1);
            }

            blurTexture = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(buffer1, blurTexture);

            RenderTexture.ReleaseTemporary(fullSizeRT);
            RenderTexture.ReleaseTemporary(buffer1);
            RenderTexture.ReleaseTemporary(buffer2);

            blurMaterial.SetTexture("_MainTex", null);

            rawImage.texture = blurTexture;
            rawImage.enabled = rawImageEnabled;
        }

        private void CaptureMainAndUI(RenderTexture target)
        {
            Camera mainCamera = Camera.main;
            Camera uiCamera = GFGlobal.UI.UICamera;

            if (mainCamera == null)
            {
                var uiPrev = uiCamera.targetTexture;
                uiCamera.targetTexture = target;
                uiCamera.Render();
                uiCamera.targetTexture = uiPrev;
                return;
            }

            var mainPrev = mainCamera.targetTexture;
            mainCamera.targetTexture = target;
            mainCamera.Render();
            mainCamera.targetTexture = mainPrev;
        }


        void OnDestroy()
        {
            Release();
            if (blurMaterial != null)
            {
                Destroy(blurMaterial);
                blurMaterial = null;
            }
        }
    }
}