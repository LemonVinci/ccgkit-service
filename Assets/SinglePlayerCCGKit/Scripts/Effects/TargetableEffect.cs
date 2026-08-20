// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace CCGKit
{
    /// <summary>
    /// The base type of all the "targetable" card effects; meaning those that have a target
    /// (with that target being a character entity or a group of character entities). Note
    /// the abstract Resolve method taking an Entity parameter, which will be overriden by
    /// subclasses to implement the appropriate gameplay logic for that effect.
    /// </summary>
    public abstract class TargetableEffect : Effect
    {
        public EffectTargetType Target;

        public abstract void Resolve(RuntimeCharacter instigator, RuntimeCharacter target);
    }
}
