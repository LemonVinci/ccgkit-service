// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Utility class that makes the associated GameObject move up and down. It is used
    /// with the intent widgets of the enemies.
    /// </summary>
    public class FloatingObject : MonoBehaviour
    {
        public float Amplitude = 0.1f;
        public float Frequency = 0.75f;

        private Vector3 pos;

        private void Start()
        {
            pos = transform.position;
        }

        private void Update()
        {
            var newPos = pos;
            newPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * Frequency) * Amplitude;
            transform.position = newPos;
        }
    }
}
