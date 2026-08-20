// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Architecture/Events/Game event (Intent change)",
        fileName = "GameEvent",
        order = 3)]
    public class IntentChangeEvent : ScriptableObject
    {
        private readonly List<IntentChangeEventListener> listeners = new List<IntentChangeEventListener>();

        public void Raise(Sprite sprite, int value)
        {
            for (var i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised(sprite, value);
        }

        public void RegisterListener(IntentChangeEventListener listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void UnregisterListener(IntentChangeEventListener listener)
        {
            if (listeners.Contains(listener))
                listeners.Remove(listener);
        }
    }
}