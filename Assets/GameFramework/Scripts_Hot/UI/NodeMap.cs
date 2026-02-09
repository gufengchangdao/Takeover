using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.Hot
{
    public class NodeMap : MonoBehaviour
    {
#if UNITY_EDITOR
        private GameObject editorGameObject;

        private void EditorOnEditorGameObjectChange()
        {
            if (editorGameObject == null)
                return;
            if (nodes == null)
                nodes = new();

            var r = new NodeReference();
            r.GetType().GetMethod("EditorInit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(r, new object[] { editorGameObject });
            nodes.Add(r);
        }
#endif

        [SerializeField]
        private List<NodeReference> nodes;
        private Dictionary<string, Component> m_Map;
        private Dictionary<string, Component> Map
        {
            get
            {
                if (m_Map == null)
                {
                    m_Map = new(nodes.Count);
                    foreach (var node in nodes)
                        m_Map.Add(node.key, node.component);
                }
                return m_Map;
            }
        }

        public T Get<T>(string key) where T : Component
        {
            return Map[key] as T;
        }

        public Transform GetTransform(string key)
        {
            return Map[key] as Transform;
        }

        [Serializable]
        public class NodeReference
        {
            public string key = "";
            public Component component;

#if UNITY_EDITOR
            private GameObject editorGameObject;
            private List<Component> editorComponents = new();

            private void EditorInit(GameObject go)
            {
                editorGameObject = go;
                EditorOnEditorGameObjectChange();
            }

            private void EditorOnEditorGameObjectChange()
            {
                editorComponents.Clear();
                if (editorGameObject == null)
                {
                    key = "";
                    return;
                }

                key = editorGameObject.name;
                editorGameObject.GetComponents<Component>(editorComponents);

                if (key.StartsWith("Img") && editorGameObject.TryGetComponent<Image>(out var image))
                    component = image;
                else if (key.StartsWith("Btn") && editorGameObject.TryGetComponent<GFButton>(out var button))
                    component = button;
                else if (key.StartsWith("Txt") && editorGameObject.TryGetComponent<TextMeshProUGUI>(out var text))
                    component = text;
                else
                    component = editorGameObject.transform;
            }
#endif
        }
    }
}