using System;
using System.Collections.Generic;
using GameFramework.Hot;
using TableStructure;
using UnityEngine;
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
        private bool isPrefab;
        [SerializeField]
        private bool hasBroken;

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
                _isBroken = value;

                if (_isBroken != value && hasBroken)
                    UpdateCampSprite();
            }
        }

        private string currentLoadingAssetPath;
        private GameObject childNode;

        private void UpdateCampSprite()
        {
            string assetPath = assetPathBase;
            if (hasBroken && IsBroken)
                assetPath += "_broken";
            assetPath += $"_{CurCamp.ToString().ToLower()}";
            if (isPrefab)
                assetPath += ".prefab";
            else
                assetPath += ".png";
            if (currentLoadingAssetPath == assetPath)
                return; //已经在请求了

            currentLoadingAssetPath = assetPath;
            if (!Application.isPlaying)
            {
                if (isPrefab)
                {
                    LoadPrefabInEditor(assetPath);
                }
                else
                {
                    LoadSpriteInEditor(assetPath);
                }
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
            if (userdata as string != currentLoadingAssetPath)
                return false;

            currentLoadingAssetPath = null;
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

        private void OnSpriteLoaded(Sprite sprite, object userdata)
        {
            if (LoadCheck(userdata))
            {
                if (!childNode)
                {
                    childNode = new GameObject("Sprite", typeof(SpriteRenderer));
                    childNode.transform.SetParent(transform, false);
                }
                childNode.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }

        void Start()
        {
            if (currentLoadingAssetPath == null && !childNode)
            {
                UpdateCampSprite();
                OnCampChange?.InvokeSafe(m_CurCamp);
            }
        }

        void OnDestroy()
        {
            OnCampChange = null;
        }


#if UNITY_EDITOR
        private void LoadPrefabInEditor(string assetPath)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            currentLoadingAssetPath = null;
            if (!prefab)
            {
                Debug.LogError($"Camp prefab not found: {assetPath}", this);
                return;
            }

            if (childNode)
                DestroyImmediate(childNode);
            childNode = PrefabUtility.InstantiatePrefab(prefab, transform) as GameObject;
        }

        private void LoadSpriteInEditor(string assetPath)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            currentLoadingAssetPath = null;
            if (!sprite)
            {
                Debug.LogError($"Camp sprite not found: {assetPath}", this);
                return;
            }

            if (!childNode)
            {
                childNode = new GameObject("Sprite", typeof(SpriteRenderer));
                childNode.transform.SetParent(transform, false);
            }
            childNode.GetComponent<SpriteRenderer>().sprite = sprite;
        }

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
