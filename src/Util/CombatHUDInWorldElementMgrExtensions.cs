using BattleTech;
using BattleTech.UI;

using Harmony;

using System.Collections.Generic;

public static class CombatHUDInWorldElementMgrExtensions {
  public static CombatHUDFloatieStackActor GetFloatieStackForCombatant(this CombatHUDInWorldElementMgr combatHUDInWorldElementMgr, ICombatant combatant)
  {
    List<CombatHUDFloatieStackActor> FloatieStacks = combatHUDInWorldElementMgr.FloatieStacks;
    return FloatieStacks.Find((CombatHUDFloatieStackActor x) => x.DisplayedCombatant == combatant);
  }
}