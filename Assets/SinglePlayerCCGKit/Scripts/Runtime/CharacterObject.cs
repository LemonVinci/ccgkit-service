// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    public class CharacterObject : MonoBehaviour
    {
        public CharacterTemplate Template;
        public RuntimeCharacter Character;

        public bool IsDead;

        public void OnCharacterDied()
        {
            if (Character.Hp.Value <= 0)
            {
                GetComponent<BoxCollider2D>().enabled = false;
                var numChilds = transform.childCount;
                for (int i = numChilds - 1; i >= 0; i--)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}