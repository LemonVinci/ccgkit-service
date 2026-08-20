// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;
using UnityEngine.Events;

namespace CCGKit
{
    public class IntentChangeEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public IntentChangeEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent<Sprite, int> Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(Sprite sprite, int value)
        {
            Response.Invoke(sprite, value);
        }
    }
}