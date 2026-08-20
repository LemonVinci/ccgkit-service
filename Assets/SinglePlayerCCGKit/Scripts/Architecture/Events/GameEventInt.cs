// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Architecture/Events/Game event (int)",
        fileName = "GameEvent",
        order = 1)]
    public class GameEventInt : ScriptableObject
    {
        private readonly List<GameEventIntListener> listeners = new List<GameEventIntListener>();

        public void Raise(int value)
        {
            for (var i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised(value);
        }

        public void RegisterListener(GameEventIntListener listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void UnregisterListener(GameEventIntListener listener)
        {
            if (listeners.Contains(listener))
                listeners.Remove(listener);
        }
    }
}