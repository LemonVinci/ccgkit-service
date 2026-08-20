// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    public abstract class BaseSystem : MonoBehaviour
    {
        protected CharacterObject Player;
        protected List<CharacterObject> Enemies; 

        public virtual void Initialize(CharacterObject player, List<CharacterObject> enemies)
        {
            Player = player;
            Enemies = enemies;
        }
    }
}