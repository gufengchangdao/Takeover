
using System.Collections.Generic;
using UnityEngine;

public interface IUpdateable
{
    void OnUpdate(float dt);
}

/// <summary>
/// 自定义Update方法的好处：
/// 1. 可以给Update添加优先级，还可以在CPU高的时候跳过一些低优先级的update
/// 2. 避免直接使用MonoBehavior的Update，避免本地-托管的桥接，减少性能开销
/// </summary>
public class GameLogic : MonoBehaviour
{
    private static GameLogic _instance;
    public static GameLogic Instance
    {
        get
        {
            if (!_instance)
            {
                var go = new GameObject("GameLogic");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<GameLogic>();
            }
            return _instance;
        }
    }

    List<IUpdateable> updateableObjects = new();

    public void RegisterUpdateableObject(IUpdateable obj)
    {
        if (!updateableObjects.Contains(obj))
            updateableObjects.Add(obj);
    }

    public void DeregisterUpdateableObject(IUpdateable obj)
    {
        updateableObjects.Remove(obj);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        for (int i = 0; i < updateableObjects.Count; i++)
            updateableObjects[i].OnUpdate(dt);
    }
}

public abstract class UpdateableComponent : MonoBehaviour, IUpdateable
{
    protected virtual void Start()
    {
        GameLogic.Instance.RegisterUpdateableObject(this);
    }

    public virtual void OnUpdate(float dt) { }

    protected virtual void OnDestroy()
    {
        GameLogic.Instance.DeregisterUpdateableObject(this);
    }
}