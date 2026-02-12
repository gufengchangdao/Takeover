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

        private static int[] s_DownSampleType = new[] { 1, 2, 4, 8 };

        private RenderTexture blurTexture;
        private Material blurMaterial;

        void Start()
        {
            // Material BlurMaterial = new Material(Shader.Find("Hidden/DualBlur"));
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

            // 获得摄像机内容
            RenderTexture fullSizeRT = RenderTexture.GetTemporary(Screen.width, Screen.height);
            Camera uiCamera = GFGlobal.UI.UICamera;
            RenderTexture previousRT = uiCamera.targetTexture;
            uiCamera.targetTexture = fullSizeRT;
            uiCamera.Render(); // 渲染一帧到临时纹理
            uiCamera.targetTexture = previousRT;

            blurMaterial.SetTexture("_MainTex", fullSizeRT);

            // 使用双缓冲：创建两个临时纹理用于下采样和上采样
            int width = Screen.width / downSample;
            int height = Screen.height / downSample;
            RenderTexture buffer1 = RenderTexture.GetTemporary(width, height);
            RenderTexture buffer2 = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(fullSizeRT, buffer1); //先降采样
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

            // 清空材质球纹理引用
            blurMaterial.SetTexture("_MainTex", null);

            var rawImage = gameObject.GFGetOrAddComponent<RawImage>();
            rawImage.texture = blurTexture;
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