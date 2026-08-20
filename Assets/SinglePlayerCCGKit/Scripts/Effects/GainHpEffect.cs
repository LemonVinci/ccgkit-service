// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;

namespace CCGKit
{
    /// <summary>
    /// The type corresponding to the "Gain X HP" card effect.
    /// </summary>
    [Serializable]
    public class GainHpEffect : IntegerEffect, IEntityEffect
    {
        public override string GetName()
        {
            return $"Gain {Value.ToString()} HP";
        }

        public override void Resolve(RuntimeCharacter instigator, RuntimeCharacter target)
        {
            var targetHp = target.Hp;
            var finalHp = targetHp.Value + Value;
            if (finalHp > target.MaxHp)
            {
                finalHp = target.MaxHp;
            }
            targetHp.SetValue(finalHp);
        }
    }
}
