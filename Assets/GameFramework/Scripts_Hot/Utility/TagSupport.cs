using System.Collections.Generic;

namespace GameFramework.Hot
{
    public class TagSupport : IRecyclable
    {
        private readonly Dictionary<int, HashSet<string>> tags = new();

        public void AddTag(int tag, string source = "")
        {
            if (!tags.TryGetValue(tag, out var sets))
            {
                sets = new HashSet<string>();
                tags.Add(tag, sets);
            }
            sets.Add(source);
        }

        public bool HasTag(int tag)
        {
            return tags.ContainsKey(tag);
        }

        public void RemoveTag(int tag, string source = "")
        {
            if (tags.TryGetValue(tag, out var sets))
            {
                sets.Remove(source);
                if (sets.Count == 0)
                    tags.Remove(tag);
            }
        }

        public void ClearTag()
        {
            tags.Clear();
        }

        public void ResetTag(int[] states)
        {
            tags.Clear();
            foreach (var tag in states)
                AddTag(tag);
        }

        public void OnRecycle()
        {
            tags.Clear();
        }
    }
}