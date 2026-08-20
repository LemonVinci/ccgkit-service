// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Utility component that can be attached to any GameObject and will automatically destroy
    /// it after a configurable amount of time.
    /// </summary>
    public class AutoKill : MonoBehaviour
    {
        public float Duration;

        private float accTime;

        private void Update()
        {
            accTime += Time.deltaTime;
            if (accTime >= Duration)
            {
                Destroy(gameObject);
            }
        }
    }
}
