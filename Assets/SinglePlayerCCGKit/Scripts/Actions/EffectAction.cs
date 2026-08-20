// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace CCGKit
{
    public abstract class EffectAction : ScriptableObject
    {
        public abstract string GetName();

        public abstract void Execute(GameObject go);

#if UNITY_EDITOR
        protected float RowHeight = EditorGUIUtility.singleLineHeight + 2;

        public abstract void Draw(Rect rect);

        public virtual float GetHeight()
        {
            var spacing = EditorGUIUtility.singleLineHeight;
            return EditorGUIUtility.singleLineHeight*GetNumRows() + spacing;
        }

        public virtual int GetNumRows()
        {
            return 3;
        }
#endif
    }
}
