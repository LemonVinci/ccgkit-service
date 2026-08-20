// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;

namespace CCGKit
{
    [Serializable]
    public class SaveData
    {
        public int Hp;
        public int Shield;
        public List<int> Deck = new List<int>();
    }
}