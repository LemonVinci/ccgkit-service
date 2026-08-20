// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    public class StatusWidget : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private GameObject elementPrefab;
#pragma warning restore 649

        private bool isCharacterDead;

        private readonly List<StatusElementWidget> elements = new List<StatusElementWidget>(4);

        public void OnStatusChanged(StatusTemplate status, int value)
        {
            if (isCharacterDead)
            {
                return;
            }

            var foundElement = false;
            foreach (var element in elements)
            {
                if (element.Type == status.Name)
                {
                    if (value > 0)
                    {
                        element.UpdateModifier(value);
                        foundElement = true;
                        break;
                    }

                    elements.Remove(element);
                    element.FadeAndDestroy();
                    foundElement = true;
                    break;
                }
            }

            if (!foundElement)
            {
                var newElement = Instantiate(elementPrefab, transform, false);
                var widget = newElement.GetComponent<StatusElementWidget>();
                widget.Initialize(status, value);
                widget.Show();
                elements.Add(widget);
            }
        }

        public void OnHpChanged(int hp)
        {
            if (hp <= 0)
            {
                isCharacterDead = true;
                foreach (var element in elements)
                {
                    element.FadeAndDestroy();
                }
            }
        }
    }
}
