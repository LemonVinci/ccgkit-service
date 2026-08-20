// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Architecture/Events/Game event (Status)",
        fileName = "GameEvent",
        order = 2)]
    public class GameEventStatus : ScriptableObject
    {
        private readonly List<GameEventStatusListener> listeners = new List<GameEventStatusListener>();

        public void Raise(StatusTemplate status, int value)
        {
            for (var i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised(status, value);
        }

        public void RegisterListener(GameEventStatusListener listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void UnregisterListener(GameEventStatusListener listener)
        {
            if (listeners.Contains(listener))
                listeners.Remove(listener);
        }
    }
}