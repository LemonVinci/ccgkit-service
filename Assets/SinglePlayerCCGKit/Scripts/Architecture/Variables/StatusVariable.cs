// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    [CreateAssetMenu(
        menuName = "Single-Player CCG Kit/Architecture/Variables/Status",
        fileName = "Variable",
        order = 1)]
    public class StatusVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = string.Empty;
#endif

        public Dictionary<string, int> Value = new Dictionary<string, int>();
        public Dictionary<string, StatusTemplate> Template = new Dictionary<string, StatusTemplate>();

        public GameEventStatus ValueChangedEvent;

        public int GetValue(string status)
        {
            if (Value.ContainsKey(status))
            {
                return Value[status];
            }

            return 0;
        }

        public void SetValue(StatusTemplate status, int value)
        {
            var statusName = status.Name;
            if (Value.ContainsKey(statusName))
            {
                Value[statusName] = value;
            }
            else
            {
                Value.Add(statusName, value);
            }
            ValueChangedEvent?.Raise(status, value);

            if (!Template.ContainsKey(statusName))
            {
                Template.Add(statusName, status);
            }
        }

        public void SetValue(string status, int value)
        {
            if (Value.ContainsKey(status))
            {
                Value[status] = value;
            }
            else
            {
                Value.Add(status, value);
            }
            ValueChangedEvent?.Raise(Template[status], value);
        }
    }
}