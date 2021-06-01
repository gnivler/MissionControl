using Harmony;

using BattleTech.Framework;

namespace MissionControl.Patches {
  [HarmonyPatch(typeof(ObjectiveGameLogic), "ActivateObjective")]
  public class ObjectiveGameLogicActivateObjectivePatch {
    static void Postfix(ObjectiveGameLogic __instance) {
      Main.LogDebug($"[ObjectiveGameLogicActivateObjectivePatch.Postfix] Running...");
      __instance.OnBuildingSpawned(null);
    }
  }
}