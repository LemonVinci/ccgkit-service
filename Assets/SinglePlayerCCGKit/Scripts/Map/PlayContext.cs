// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace CCGKit
{
  /// <summary>
  /// Identifies isolated play sessions so map/save data do not leak between prototypes.
  /// </summary>
  public static class PlayContext
  {
    public const string Kitchen = "kitchen";
    public const string TrickTaking = "trick_taking";

    public const string KitchenMapPref = "map";
    public const string KitchenSavePref = "save";

    public const string TrickTakingMapPref = "trick_map";
    public const string TrickTakingSavePref = "trick_save";
  }
}
