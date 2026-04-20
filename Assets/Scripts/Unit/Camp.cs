using System;
using System.Collections.Generic;
using GameFramework.Hot;
using TableStructure;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Takeover
{
    public class Camp : MonoBehaviour
    {
        [SerializeField]
        private ECamp m_CurCamp = ECamp.Green;
        public ECamp CurCamp
        {
            get { return m_CurCamp; }
            set
            {
                if (m_CurCamp != value)
                {
                    m_CurCamp = value;
                    UpdateCampSprite();
                    OnCampChange?.InvokeSafe(m_CurCamp);
                }
            }
        }

        [SerializeField]
        private string assetPathBase;

        [SerializeField]
        private bool hasBroken;
        [SerializeField]
        private bool setNativeSize = true;

        public event Action<ECamp> OnCampChange;

        private bool _isBroken;
        public bool IsBroken
        {
            get
            {
                return _isBroken;
            }
            set
            {
                if (_isBroken != value)
                {
                    _isBroken = value;
                    if (hasBroken)
                        UpdateCampSprite();
                }
            }
        }

        private string loadingAssetPath;
        private GameObject childNode;

        private void UpdateCampSprite()
        {
            bool isSprite = TryGetComponent<SpriteRenderer>(out var spriteRenderer);
            bool isImage = TryGetComponent<Image>(out var image);
            bool isPrefab = !isSprite && !isImage;

            string assetPath = assetPathBase;
            if (hasBroken && IsBroken)
                assetPath += "_broken";
            assetPath += $"_{CurCamp.ToString().ToLower()}";
            if (isPrefab)
                assetPath += ".prefab";
            else
                assetPath += ".png";
            if (loadingAssetPath == assetPath)
                return; //已经在请求了

            loadingAssetPath = assetPath;
            if (!Application.isPlaying)
            {
                if (isPrefab)
                    LoadPrefabInEditor(assetPath);
                else
                    LoadSpriteInEditor(assetPath);
            }
            else
            {
                if (isPrefab)
                    GFGlobal.Resource.LoadAssetAsync<GameObject>(assetPath, OnPrefabLoaded, assetPath);
                else
                    GFGlobal.Resource.LoadAssetAsync<Sprite>(assetPath, OnSpriteLoaded, assetPath);
            }
        }

        private bool LoadCheck(object userdata)
        {
            if (!gameObject) //对象已经销毁
                return false;
            if (userdata as string != loadingAssetPath)
                return false;

            loadingAssetPath = null;
            return true;
        }

        private void OnPrefabLoaded(GameObject obj, object userdata)
        {
            if (LoadCheck(userdata))
            {
                if (childNode)
                    Destroy(childNode);
                childNode = Instantiate(obj, transform);
            }
        }

        private void SetImage(Sprite sprite)
        {
            if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                spriteRenderer.sprite = sprite;
            }
            else if (TryGetComponent<Image>(out var image))
            {
                image.sprite = sprite;
                if (setNativeSize)
                    image.SetNativeSize();
            }
        }

        private void OnSpriteLoaded(Sprite sprite, object userdata)
        {
            if (LoadCheck(userdata))
                SetImage(sprite);
        }

        void Start()
        {
            if (Global.LevelData != null) //根据当前阵营初始化一下
                CurCamp = Global.LevelData.Camp;
        }

        void OnDestroy()
        {
            OnCampChange = null;
        }



        private void LoadPrefabInEditor(string assetPath)
        {
#if UNITY_EDITOR
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            loadingAssetPath = null;
            if (!prefab)
            {
                Debug.LogError($"Camp prefab not found: {assetPath}", this);
                return;
            }

            if (childNode)
                DestroyImmediate(childNode);
            childNode = PrefabUtility.InstantiatePrefab(prefab, transform) as GameObject;
#endif
        }

        private void LoadSpriteInEditor(string assetPath)
        {
#if UNITY_EDITOR
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            loadingAssetPath = null;
            if (!sprite)
            {
                Debug.LogError($"Camp sprite not found: {assetPath}", this);
                return;
            }

            SetImage(sprite);
#endif
        }
#if UNITY_EDITOR
        private void SelectSameCampGameObject()
        {
            var cur = gameObject.GetComponent<Camp>().CurCamp;
            var camps = GameObject.FindObjectsByType<Camp>(FindObjectsSortMode.None);
            var selected = new List<UnityEngine.Object>();
            foreach (var camp in camps)
            {
                if (camp.CurCamp == cur)
                    selected.Add(camp.gameObject);
            }
            UnityEditor.Selection.objects = selected.ToArray();
        }
#endif
    }
}
