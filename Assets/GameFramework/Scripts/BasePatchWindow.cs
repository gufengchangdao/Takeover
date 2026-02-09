using System.Collections;
using UnityEngine;
using YooAsset;

public abstract class BasePatchWindow : MonoBehaviour
{
    /// <summary>
    /// 显示错误提示框
    /// </summary>
    /// <param name="message">内容</param>
    /// <param name="options">选项</param>
    /// <returns></returns>
    public abstract IEnumerator ShowErrorMessage(string message, params string[] options);

    /// <summary>
    /// 获取选项
    /// </summary>
    /// <returns></returns>
    public abstract int GetOption();

    /// <summary>
    /// 更新进度条
    /// </summary>
    /// <param name="updateData"></param>
    public abstract void UpdateProgress(DownloadUpdateData updateData);
}