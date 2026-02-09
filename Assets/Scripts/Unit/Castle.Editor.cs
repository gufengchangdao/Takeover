using UnityEngine;

namespace Takeover
{
    public partial class Castle
    {
#if UNITY_EDITOR
        private void FillNearbyDecorate()
        {
            var castles = GameObject.FindObjectsByType<Castle>(FindObjectsSortMode.None);
            foreach (var castle in castles)
            {
                castle.mapDecorations.Clear();
                UnityEditor.EditorUtility.SetDirty(castle);
            }

            var camps = GameObject.FindObjectsByType<Camp>(FindObjectsSortMode.None);
            foreach (var camp in camps)
            {
                if (!camp.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                    continue;

                if (spriteRenderer.sortingLayerName != "Decorate")
                    continue;

                // 查找离自己最近的城堡
                float minDistance = float.MaxValue;
                Castle closestCastle = null;
                foreach (var castle in castles)
                {
                    float distance = Vector2.Distance(castle.transform.position, camp.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestCastle = castle;
                    }
                }

                if (closestCastle != null)
                {
                    closestCastle.mapDecorations.Add(camp);
                    camp.CurCamp = closestCastle.GetComponent<Camp>().CurCamp;
                    UnityEditor.EditorUtility.SetDirty(closestCastle);
                    UnityEditor.EditorUtility.SetDirty(camp);
                }
            }
        }
#endif
    }
}