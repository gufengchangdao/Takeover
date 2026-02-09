#if USE_HYBRIDCLR
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public static class HybridCLRMenus
{
    private static void TryAddAssemblyData(Dictionary<string, HashSet<string>> dict, Type type)
    {
        string typeName = type.FullName;
        var assemblyName = type.Assembly.GetName();
        if (typeName.StartsWith("UnityEngine") || typeName.StartsWith("TMPro"))
        {
            if (!dict.TryGetValue(assemblyName.Name, out var typeList))
            {
                typeList = new HashSet<string>();
                dict.Add(assemblyName.Name, typeList);
            }
            typeList.Add(typeName);
        }
    }

    public static string[] prefabDirs = new string[] { "Assets/Content" };

    /// <summary>
    /// 扫描给定路径下的预制件的脚本，根据脚本的类型以及里面的变量类型，结合HybridCLR生成的link.xml生成新的link.xml，会覆盖Assets/link.xml
    /// </summary>
    [MenuItem("HybridCLR/Generate/扫描预制件脚本生成新的link.xml")]
    private static void GenerateLinkXml()
    {
        var asstIds = AssetDatabase.FindAssets("t:Prefab", prefabDirs);
        var count = 0;
        Dictionary<string, HashSet<string>> increasinglyAssemblyDic = new Dictionary<string, HashSet<string>>();
        //遍历所有预制体
        for (int i = 0; i < asstIds.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(asstIds[i]);
            var pfb = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            var components = pfb.GetComponentsInChildren<Component>();
            //遍历预制体所有组件
            foreach (var component in components)
            {
                var type = component.GetType();
                TryAddAssemblyData(increasinglyAssemblyDic, type);

                string typeName = type.FullName;
                if (typeName.StartsWith("UnityEngine") || typeName.StartsWith("TMPro"))
                {
                    var properties = type.GetProperties();
                    //获取组件的属性，如果属性是Unity对象，则再获取一次属性
                    foreach (var propertyInfo in properties)
                    {
                        type = propertyInfo.PropertyType;
                        TryAddAssemblyData(increasinglyAssemblyDic, type);

                        if (type.BaseType == typeof(UnityEngine.Object))
                        {
                            //为了确保大部分类都被获取到，直接获取组件的属性类
                            foreach (var property in propertyInfo.PropertyType.GetProperties())
                            {
                                var propertyType = property.PropertyType.GetType();
                                if (property.PropertyType.IsArray)
                                    propertyType = property.PropertyType.GetElementType();
                                TryAddAssemblyData(increasinglyAssemblyDic, propertyType);
                            }
                        }
                    }
                }
            }

            count++;
            Debug.Log($"<color=#7CFC00>Find Prefab: {pfb.name}, Count: {count}/{asstIds.Length}</color>");
        }

        foreach (var item in increasinglyAssemblyDic)
        {
            Debug.Log($"{item.Key}: {string.Join(", ", item.Value)}");
        }


        string filePath = @$"{Application.dataPath}/HybridCLRGenerate/link.xml";
        var data = File.ReadAllText(filePath);

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(data);
        XmlNode linker = xml.SelectSingleNode(xml.DocumentElement.Name);
        XmlNodeList assemblyList = linker.ChildNodes;

        Dictionary<string, HashSet<string>> assemblyDic = new Dictionary<string, HashSet<string>>();
        foreach (var typeListItem in assemblyList)
        {
            var typeListElement = (XmlElement)typeListItem;
            var assemblyNmae = typeListElement.GetAttribute("fullname");
            if (!assemblyDic.ContainsKey(assemblyNmae))
            {
                assemblyDic.Add(assemblyNmae, new HashSet<string>());
            }

            var typeListNodeList = (XmlNode)typeListItem;
            foreach (var typeItem in typeListNodeList.ChildNodes)
            {
                var typeElement = (XmlElement)typeItem;
                var typeName = typeElement.GetAttribute("fullname");
                assemblyDic[assemblyNmae].Add(typeName);
            }
        }

        foreach (var assemblyName in increasinglyAssemblyDic.Keys)
        {
            if (!assemblyDic.ContainsKey(assemblyName))
            {
                var assemblyNode = xml.CreateElement(linker.FirstChild.Name);
                assemblyNode.SetAttribute("fullname", assemblyName);
                assemblyDic.Add(assemblyName, increasinglyAssemblyDic[assemblyName]);
                foreach (var typeName in increasinglyAssemblyDic[assemblyName])
                {
                    var typeNode = xml.CreateElement(linker.FirstChild.FirstChild.Name);
                    typeNode.SetAttribute("fullname", typeName);
                    typeNode.SetAttribute("preserve", "all");
                    assemblyNode.AppendChild(typeNode);
                }

                linker.AppendChild(assemblyNode);
                continue;
            }

            foreach (var typeName in increasinglyAssemblyDic[assemblyName])
            {
                if (!assemblyDic[assemblyName].Contains(typeName))
                {
                    var typeNode = xml.CreateElement(linker.FirstChild.FirstChild.Name);
                    typeNode.SetAttribute("fullname", typeName);
                    typeNode.SetAttribute("preserve", "all");
                    //assemblyNode.AppendChild(typeNode);
                    foreach (XmlElement assemblyElement in assemblyList)
                    {
                        if (assemblyElement.GetAttribute("fullname") == assemblyName)
                        {
                            assemblyElement.AppendChild(typeNode);
                        }
                    }
                }
            }
        }

        xml.Save($"{Application.dataPath}/link.xml");
        AssetDatabase.Refresh();
    }

    public static List<string> CopyDllFileToByte(string[] fileNames, string originDir, string targetDir)
    {
        string projectPath = Directory.GetParent(Application.dataPath).FullName;
        List<string> bytesFiles = new List<string>();
        foreach (var fileName in fileNames)
        {
            Debug.LogError($"{projectPath}, {originDir}, {fileName}");
            var dllFilePath = Path.Combine(projectPath, originDir, $"{fileName}.dll");
            if (!File.Exists(dllFilePath))
            {
                Debug.Log($"{dllFilePath}不存在");
                continue;
            }

            var targetFileName = $"{fileName}.bytes";
            var dllRawFilePath = Path.Combine(targetDir, targetFileName);
            File.Copy(dllFilePath, dllRawFilePath, true);
            bytesFiles.Add(fileName);
        }

        return bytesFiles;
    }

    [MenuItem("HybridCLR/Generate/根据AOTGenericReferences拷贝裁剪dll并显示引用程序集类")]
    private static void FillAOTGenericReferences()
    {
        // 1. 把AOTGenericReferences里定义的被裁剪的dll拷贝到StreamingAssets下
        // 2. 拷贝AOTGenericReferences到自己的程序集下并且添加对link.xml里类的引用

    }
}

#endif