using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

    static AOTGenericReferences()
    {
        // 把link.xml里的每个程序集都引用一遍，防止程序集被裁剪
        var Log = typeof(GameFramework.AOT.Log);
        var BeanBase = typeof(Luban.BeanBase);
        var GeneratedCodeAttribute = typeof(System.CodeDom.Compiler.GeneratedCodeAttribute);
        var Enumerable = typeof(System.Linq.Enumerable);
        var InputAction = typeof(UnityEngine.InputSystem.InputAction);
        var AnimationClip = typeof(UnityEngine.AnimationClip);
        var AudioClip = typeof(UnityEngine.AudioClip);
        var Camera = typeof(UnityEngine.Camera);
        var Collider2D = typeof(UnityEngine.Collider2D);
        var Button = typeof(UnityEngine.UI.Button);
        var Component = typeof(UnityEngine.Component);
        var Canvas = typeof(UnityEngine.Canvas);
        var Array = typeof(System.Array);
        var YooAssets = typeof(YooAsset.YooAssets);
        var Action = typeof(System.Action);
    }

    // {{ AOT assemblies
    public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
    {
        "System.Core.dll",
        "System.dll",
        "Unity.InputSystem.dll",
        "UnityEngine.CoreModule.dll",
        "YooAsset.dll",
        "mscorlib.dll",
    };
    // }}

    // {{ constraint implement type
    // }} 

    // {{ AOT generic types
    // System.Action<UnityEngine.InputSystem.InputAction.CallbackContext>
    // System.Action<int>
    // System.Action<object,object,object>
    // System.Action<object,object>
    // System.Action<object>
    // System.Collections.Generic.ArraySortHelper<int>
    // System.Collections.Generic.ArraySortHelper<object>
    // System.Collections.Generic.Comparer<int>
    // System.Collections.Generic.Comparer<object>
    // System.Collections.Generic.Dictionary.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.Enumerator<object,GameFramework.Hot.GFUI.WaitOpenPanelInfo>
    // System.Collections.Generic.Dictionary.Enumerator<object,byte>
    // System.Collections.Generic.Dictionary.Enumerator<object,float>
    // System.Collections.Generic.Dictionary.Enumerator<object,int>
    // System.Collections.Generic.Dictionary.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,GameFramework.Hot.GFUI.WaitOpenPanelInfo>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,byte>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,float>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
    // System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.KeyCollection<int,object>
    // System.Collections.Generic.Dictionary.KeyCollection<object,GameFramework.Hot.GFUI.WaitOpenPanelInfo>
    // System.Collections.Generic.Dictionary.KeyCollection<object,byte>
    // System.Collections.Generic.Dictionary.KeyCollection<object,float>
    // System.Collections.Generic.Dictionary.KeyCollection<object,int>
    // System.Collections.Generic.Dictionary.KeyCollection<object,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,GameFramework.Hot.GFUI.WaitOpenPanelInfo>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,byte>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,float>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
    // System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
    // System.Collections.Generic.Dictionary.ValueCollection<int,object>
    // System.Collections.Generic.Dictionary.ValueCollection<object,GameFramework.Hot.GFUI.WaitOpenPanelInfo>
    // System.Collections.Generic.Dictionary.ValueCollection<object,byte>
    // System.Collections.Generic.Dictionary.ValueCollection<object,float>
    // System.Collections.Generic.Dictionary.ValueCollection<object,int>
    // System.Collections.Generic.Dictionary.ValueCollection<object,object>
    // System.Collections.Generic.Dictionary<int,object>
    // System.Collections.Generic.Dictionary<object,GameFramework.Hot.GFUI.WaitOpenPanelInfo>
    // System.Collections.Generic.Dictionary<object,byte>
    // System.Collections.Generic.Dictionary<object,float>
    // System.Collections.Generic.Dictionary<object,int>
    // System.Collections.Generic.Dictionary<object,object>
    // System.Collections.Generic.EqualityComparer<GameFramework.Hot.GFUI.WaitOpenPanelInfo>
    // System.Collections.Generic.EqualityComparer<byte>
    // System.Collections.Generic.EqualityComparer<float>
    // System.Collections.Generic.EqualityComparer<int>
    // System.Collections.Generic.EqualityComparer<object>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,GameFramework.Hot.GFUI.WaitOpenPanelInfo>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,byte>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,float>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
    // System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.ICollection<int>
    // System.Collections.Generic.ICollection<object>
    // System.Collections.Generic.IComparer<int>
    // System.Collections.Generic.IComparer<object>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,GameFramework.Hot.GFUI.WaitOpenPanelInfo>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,byte>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,float>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
    // System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IEnumerable<int>
    // System.Collections.Generic.IEnumerable<object>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,GameFramework.Hot.GFUI.WaitOpenPanelInfo>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,byte>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,float>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
    // System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
    // System.Collections.Generic.IEnumerator<int>
    // System.Collections.Generic.IEnumerator<object>
    // System.Collections.Generic.IEqualityComparer<int>
    // System.Collections.Generic.IEqualityComparer<object>
    // System.Collections.Generic.IList<int>
    // System.Collections.Generic.IList<object>
    // System.Collections.Generic.KeyValuePair<int,object>
    // System.Collections.Generic.KeyValuePair<object,GameFramework.Hot.GFUI.WaitOpenPanelInfo>
    // System.Collections.Generic.KeyValuePair<object,byte>
    // System.Collections.Generic.KeyValuePair<object,float>
    // System.Collections.Generic.KeyValuePair<object,int>
    // System.Collections.Generic.KeyValuePair<object,object>
    // System.Collections.Generic.LinkedList.Enumerator<object>
    // System.Collections.Generic.LinkedList<object>
    // System.Collections.Generic.LinkedListNode<object>
    // System.Collections.Generic.List.Enumerator<int>
    // System.Collections.Generic.List.Enumerator<object>
    // System.Collections.Generic.List<int>
    // System.Collections.Generic.List<object>
    // System.Collections.Generic.ObjectComparer<int>
    // System.Collections.Generic.ObjectComparer<object>
    // System.Collections.Generic.ObjectEqualityComparer<GameFramework.Hot.GFUI.WaitOpenPanelInfo>
    // System.Collections.Generic.ObjectEqualityComparer<byte>
    // System.Collections.Generic.ObjectEqualityComparer<float>
    // System.Collections.Generic.ObjectEqualityComparer<int>
    // System.Collections.Generic.ObjectEqualityComparer<object>
    // System.Collections.Generic.Queue.Enumerator<GameFramework.Hot.PoolObj<object>>
    // System.Collections.Generic.Queue<GameFramework.Hot.PoolObj<object>>
    // System.Collections.ObjectModel.ReadOnlyCollection<int>
    // System.Collections.ObjectModel.ReadOnlyCollection<object>
    // System.Comparison<int>
    // System.Comparison<object>
    // System.EventHandler<object>
    // System.Func<object,byte>
    // System.Func<object,float>
    // System.Func<object,int>
    // System.Func<object,object,byte>
    // System.Func<object,object>
    // System.Func<object>
    // System.IComparable<object>
    // System.Linq.Enumerable.Iterator<object>
    // System.Linq.Enumerable.WhereEnumerableIterator<object>
    // System.Linq.Enumerable.WhereSelectArrayIterator<object,object>
    // System.Linq.Enumerable.WhereSelectEnumerableIterator<object,object>
    // System.Linq.Enumerable.WhereSelectListIterator<object,object>
    // System.Predicate<int>
    // System.Predicate<object>
    // UnityEngine.InputSystem.InputBindingComposite<UnityEngine.Vector2>
    // UnityEngine.InputSystem.InputControl<UnityEngine.Vector2>
    // UnityEngine.InputSystem.InputProcessor<UnityEngine.Vector2>
    // UnityEngine.InputSystem.Utilities.InlinedArray<object>
    // }}

    public void RefMethods()
    {
        // object System.Activator.CreateInstance<object>()
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
        // System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
        // System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<object>.Select<object>(System.Func<object,object>)
        // object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
        // System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
        // System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<UnityEngine.Vector2>(UnityEngine.Vector2&)
        // int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<UnityEngine.Vector2>()
        // object UnityEngine.Component.GetComponent<object>()
        // object UnityEngine.Component.GetComponentInChildren<object>()
        // object UnityEngine.Component.GetComponentInParent<object>()
        // object[] UnityEngine.Component.GetComponentsInChildren<object>()
        // object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
        // bool UnityEngine.Component.TryGetComponent<object>(object&)
        // object UnityEngine.GameObject.AddComponent<object>()
        // object UnityEngine.GameObject.GetComponent<object>()
        // object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
        // bool UnityEngine.GameObject.TryGetComponent<object>(object&)
        // UnityEngine.Vector2 UnityEngine.InputSystem.InputAction.CallbackContext.ReadValue<UnityEngine.Vector2>()
        // UnityEngine.Vector2 UnityEngine.InputSystem.InputActionState.ApplyProcessors<UnityEngine.Vector2>(int,UnityEngine.Vector2,UnityEngine.InputSystem.InputControl<UnityEngine.Vector2>)
        // UnityEngine.Vector2 UnityEngine.InputSystem.InputActionState.ReadValue<UnityEngine.Vector2>(int,int,bool)
        // object UnityEngine.Object.FindAnyObjectByType<object>()
        // object[] UnityEngine.Object.FindObjectsByType<object>(UnityEngine.FindObjectsSortMode)
        // object UnityEngine.Object.Instantiate<object>(object)
        // object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
        // object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
        // object[] UnityEngine.Resources.ConvertObjects<object>(UnityEngine.Object[])
        // object YooAsset.AssetHandle.GetAssetObject<object>()
        // YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetAsync<object>(string,uint)
        // YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetSync<object>(string)
    }
}