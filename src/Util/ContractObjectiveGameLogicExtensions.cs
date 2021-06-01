
using BattleTech;
using BattleTech.Framework;

using Harmony;

public static class ContractObjectiveGameLogicExtensions {
  public static void SetInactiveContractControlledObjectivesNotRequired(this ContractObjectiveGameLogic contractObjectiveGameLogic) {
    ItemRegistry itemRegistry = UnityGameInstance.BattleTechGame.Combat.ItemRegistry;

    foreach (ObjectiveRef objectiveRef in contractObjectiveGameLogic.objectiveRefList) {
      ObjectiveGameLogic objectiveGameLogic = objectiveRef.GetEncounterObject(itemRegistry);

      if (objectiveGameLogic.IsAnInactiveContractControlledObjective()) {
        MissionControl.Main.LogDebug($"[SetInactiveContractControlledObjectivesNotRequired] Setting '{objectiveGameLogic.gameObject.name}' as non-primary");
        objectiveGameLogic.primary = false;
      }
    }
  }
}