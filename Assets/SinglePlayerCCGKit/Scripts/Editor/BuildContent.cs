// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace CCGKit
{
    public class BuildContent : MonoBehaviour
    {
        [MenuItem("Tools/Single-Player CCG Kit/Build content", false, 0)]
        static void BuildAddressableContent()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("You have not created an Addressables Settings. Please create one by opening the Window/Asset Management/Addressables/Groups window and clicking on the 'Create Addressables Settings' button.");
                return;
            }

            var group = settings.DefaultGroup;
            var guids = AssetDatabase.FindAssets("t:HeroTemplate t:EnemyTemplate");

            var newEntries = new List<AddressableAssetEntry>();
            foreach (var guid in guids)
            {
                var entry = settings.CreateOrMoveEntry(guid, group, false, false);
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var filename = Path.GetFileNameWithoutExtension(path);
                entry.address = filename;

                newEntries.Add(entry);
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, newEntries, true);

            AddressableAssetSettings.BuildPlayerContent();
        }
    }
}
