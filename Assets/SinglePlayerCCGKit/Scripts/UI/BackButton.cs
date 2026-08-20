// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    public class BackButton : MonoBehaviour
    {
        public Canvas canvas;
        public Canvas prevCanvas;

        public void OnButtonPressed()
        {
            canvas.gameObject.SetActive(false);
            if (prevCanvas != null)
            {
                prevCanvas.gameObject.SetActive(true);
            }
        }
    }
}