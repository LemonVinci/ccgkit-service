// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;

namespace CCGKit
{
    public class EnemyBrain
    {
        public CharacterObject Enemy;

        public int Pattern;
        public int Count;

        public List<Effect> Effects;

        public EnemyBrain(CharacterObject enemy)
        {
            Enemy = enemy;
        }
    }
}