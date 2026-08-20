// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace CCGKit
{
    /// <summary>
    /// Miscellaneous card-related utilities.
    /// </summary>
    public static class CardUtils
    {
        public static bool CardCanBePlayed(CardTemplate card, IntVariable playerMana)
        {
            // Ingredients are always playable from the hand; mana is not used as a resource here.
            return true;
        }

        public static bool CardHasTargetableEffect(CardTemplate card)
        {
            // Ingredients use the targeting arrow to pick which dish station receives the card.
            return card != null && card.BonusEffect == null;
        }
    }
}
