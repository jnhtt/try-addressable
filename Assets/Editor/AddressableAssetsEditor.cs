using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace Lib.Editor
{
    public static class AddressableAssetsEditor
    {
        public static string ADDRESSABLE_ASSETS_PATH = "Assets/AddressableAssets/";

        [MenuItem("Addressable/Open")]
        public static void OpenAddressables()
        {
            var type = GetTypeByClassName("AddressableAssetsWindow");
            MethodInfo initMethod = null;
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var method in methods)
            {
                if (method.Name == "Init" && method.GetParameters().Length == 0)
                {
                    initMethod = method;
                    break;
                }
            }
            if (initMethod != null)
            {
                object obj = Activator.CreateInstance(type);
                initMethod.Invoke(obj, null);
            }
        }

        [MenuItem("Addressable/Update/All")]
        public static void UpdateAll()
        {
            CreateAssetGroups();
            UpdateAssetGroupEntries();
        }

        private static void CreateAssetGroups()
        {
            var aaSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);

            var dirs = System.IO.Directory.GetDirectories(ADDRESSABLE_ASSETS_PATH, "*", System.IO.SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                var dirname = System.IO.Path.GetFileName(dir);
                var addressableAssetGroup = aaSettings.groups.Find(g => g.Name == dirname);
                if (addressableAssetGroup == null)
                {
                    var group = aaSettings.CreateGroup(dirname, false, false, true, new List<AddressableAssetGroupSchema>(), new System.Type[] { });
                    var schema = group.AddSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>();
                    group.AddSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.ContentUpdateGroupSchema>();
                    aaSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
                }
            }
        }

        private static void UpdateAssetGroupEntries()
        {
            var dirs = System.IO.Directory.GetDirectories(ADDRESSABLE_ASSETS_PATH, "*", System.IO.SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                var dirname = System.IO.Path.GetFileName(dir);
                UpdateAddressableAssets(dirname);
            }
        }

        private static void UpdateAddressableAssetsWithLabel(string category)
        {
            UpdateAddressableAssets(category);
            SetLabelWithDirectoryName(category);
        }

        private static void SetLabelWithDirectoryName(string category)
        {
            var aaSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
            var masterPath = string.Format("{0}{1}", ADDRESSABLE_ASSETS_PATH, category);
            if (!System.IO.Directory.Exists(masterPath))
            {
                Debug.LogWarningFormat("not exist masterpath {0}", masterPath);
                return;
            }
            var dirs = System.IO.Directory.EnumerateDirectories(masterPath, "*", System.IO.SearchOption.AllDirectories);

            var addressableAssetGroup = aaSettings.groups.Find(g => g.Name == category);
            foreach (var dir in dirs)
            {
                var files = System.IO.Directory.EnumerateFiles(dir, "*", System.IO.SearchOption.AllDirectories);
                var label = System.IO.Path.GetFileNameWithoutExtension(dir);
                aaSettings.AddLabel(label);
                foreach (var file in files)
                {
                    var ext = System.IO.Path.GetExtension(file);
                    if (ext == ".meta" || ext == ".DS_Store")
                    {
                        continue;
                    }
                    var path = file.Replace('\\', '/');
                    var basePath = path.Replace(ADDRESSABLE_ASSETS_PATH, "");
                    var baseDir = basePath.Split('/')[0];
                    var baseFilename = System.IO.Path.GetFileNameWithoutExtension(basePath);
                    var group = aaSettings.groups.Find(g => g.Name == baseDir);
                    if (group == null)
                    {
                        group = aaSettings.CreateGroup(baseDir, false, false, true, new List<AddressableAssetGroupSchema>(), new System.Type[] { });
                        aaSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
                    }

                    var entry = Find(group, baseFilename);
                    if (entry != null)
                    {
                        entry.labels.Clear();
                        entry.SetLabel(label, true);
                    }
                }
            }
        }

        private static void UpdateAddressableAssets(string category)
        {
            var aaSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
            var masterPath = string.Format("{0}{1}", ADDRESSABLE_ASSETS_PATH, category);
            if (!System.IO.Directory.Exists(masterPath))
            {
                Debug.LogWarningFormat("not exist masterpath {0}", masterPath);
                return;
            }
            var files = System.IO.Directory.EnumerateFiles(masterPath, "*", System.IO.SearchOption.AllDirectories);
            var addressableAssetGroup = aaSettings.groups.Find(g => g.Name == category);
            var removedFiles = new List<AddressableAssetEntry>();
            if (addressableAssetGroup != null)
            {
                removedFiles = addressableAssetGroup.entries.ToList();
            }
            foreach (var file in files)
            {
                var ext = System.IO.Path.GetExtension(file);
                if (ext == ".meta")
                {
                    continue;
                }
                var path = file.Replace('\\', '/');
                var basePath = path.Replace(ADDRESSABLE_ASSETS_PATH, "");
                var baseDir = basePath.Split('/')[0];
                //var baseFilename = System.IO.Path.GetFileNameWithoutExtension(basePath);
                var baseFilename = System.IO.Path.GetFileName(basePath);
                var group = aaSettings.groups.Find(g => g.Name == baseDir);
                if (group == null)
                {
                    group = aaSettings.CreateGroup(baseDir, false, false, true, new List<AddressableAssetGroupSchema>(), new System.Type[] { });
                    aaSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
                }
                else
                {
                }

                bool shouldCreate = true;
                var entry = Find(group, baseFilename);
                if (entry != null)
                {
                    if (string.IsNullOrEmpty(entry.AssetPath))
                    {
                        group.RemoveAssetEntry(entry);
                        shouldCreate = true;
                    }
                    else if (entry.TargetAsset == null)
                    {
                        group.RemoveAssetEntry(entry);
                        shouldCreate = true;
                    }
                    else
                    {
                        shouldCreate = false;
                    }
                }

                if (shouldCreate)
                {
                    entry = aaSettings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(file), group);
                    entry.address = baseFilename;
                }

                if (removedFiles.Contains(entry))
                {
                    removedFiles.Remove(entry);
                }
            }

            Debug.Log(category + " ============== Remove Start");
            foreach (var t in removedFiles)
            {
                Debug.Log(t.address);
                aaSettings.groups.Find(g => g.Name == category)?.RemoveAssetEntry(t);
            }
            Debug.Log(category + " ============== Remove End");

            if (addressableAssetGroup != null)
            {
                var schema = addressableAssetGroup.GetSchema<BundledAssetGroupSchema>();
                if (schema.BundleMode == BundledAssetGroupSchema.BundlePackingMode.PackTogetherByLabel)
                {
                    SetLabelWithDirectoryName(category);
                }
            }
        }

        private static AddressableAssetEntry Find(AddressableAssetGroup group, string address)
        {
            foreach (var entry in group.entries)
            {
                if (entry.address == address)
                {
                    return entry;
                }
            }
            return null;
        }

        public static Type GetTypeByClassName(string className)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == className)
                    {
                        return type;
                    }
                }
            }
            return null;
        }
    }
}
