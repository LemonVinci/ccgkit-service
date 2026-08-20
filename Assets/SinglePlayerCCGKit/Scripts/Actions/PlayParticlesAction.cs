// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CCGKit
{
    public class PlayParticlesAction : EffectAction
    {
        public ParticleSystem Particles;
        public Vector3 Offset;

        public override string GetName()
        {
            return "Play particles";
        }

        public override void Execute(GameObject go)
        {
            var pos = go.transform.position;
            var particles = Instantiate(Particles);
            particles.transform.position = pos + Offset;
            particles.Play();
            var autoKill = particles.gameObject.AddComponent<AutoKill>();
            autoKill.Duration = 2.0f;
        }

#if UNITY_EDITOR
        public override void Draw(Rect rect)
        {
            var popupStyle = new GUIStyle(EditorStyles.popup);
            popupStyle.fixedWidth = 100;
            var numberFieldStyle = new GUIStyle(EditorStyles.numberField);
            numberFieldStyle.fixedWidth = 40;

            Particles = (ParticleSystem)EditorGUI.ObjectField(rect, "Particles", Particles, typeof(ParticleSystem), false);
            rect.y += RowHeight;

            Offset.x = EditorGUI.FloatField(rect, "X", Offset.x, numberFieldStyle);
            rect.y += RowHeight;
            Offset.y = EditorGUI.FloatField(rect, "Y", Offset.y, numberFieldStyle);
            rect.y += RowHeight;
            Offset.z = EditorGUI.FloatField(rect, "Z", Offset.z, numberFieldStyle);
        }

        public override int GetNumRows()
        {
            return 5;
        }
#endif
    }
}
