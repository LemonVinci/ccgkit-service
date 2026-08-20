// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CCGKit
{
    public class EnemyButton : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] 
        private AssetReference enemyTemplate;
#pragma warning restore 649
        
        //private Map map;

        private void Start()
        {
            /*map = FindFirstObjectByType<Map>();
            Assert.IsNotNull(map);*/
        }
        
        public void OnButtonPressed()
        {
            /*var go = new GameObject("MapInfo");
            var info = go.AddComponent<MapInfo>();
            info.EnemyTemplate = enemyTemplate;
            DontDestroyOnLoad(go);
            
            map.TravelToGameScene();*/
        }
    }
}