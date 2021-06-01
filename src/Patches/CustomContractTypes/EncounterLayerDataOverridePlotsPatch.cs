using Harmony;
using BattleTech;

namespace MissionControl.Patches {
  [HarmonyPatch(typeof(EncounterLayerData), "OverridePlots")]
  public class EncounterLayerDataOverridePlotsPatch {
    static void Postfix(EncounterLayerData __instance) {
      Main.LogDebug($"[EncounterLayerData.Postfix] Running...");
      EncounterDataManager.Instance.HandleCustomContractType();
    }
  }
}