using BattleTech;
using BattleTech.Framework;

using Harmony;

namespace MissionControl.Result {
  public class ShowObjectiveResult : EncounterResult {
    public string ObjectiveGuid { get; set; }
    public bool IsContractObjective { get; set; } = false;

    public override void Trigger(MessageCenterMessage inMessage, string triggeringName) {
      Main.LogDebug($"[ShowObjectiveResult] Showing objective for object guid '{ObjectiveGuid}'");

      if (IsContractObjective) {
        ContractObjectiveGameLogic contractObjectiveGameLogic = MissionControl.Instance.EncounterLayerData.GetContractObjectiveGameLogicByGUID(ObjectiveGuid);
        ShowContractObjective(contractObjectiveGameLogic);
      } else {
        ObjectiveGameLogic objectiveGameLogic = UnityGameInstance.BattleTechGame.Combat.ItemRegistry.GetItemByGUID<ObjectiveGameLogic>(ObjectiveGuid);
        if (objectiveGameLogic != null) {
          ShowObjective(objectiveGameLogic);
        } else {
          Main.Logger.LogError($"[ShowObjectiveResult] ObjectiveGameLogic not found with objective guid '{ObjectiveGuid}'");
        }
      }
    }

    public void ShowObjective(ObjectiveGameLogic objectiveGameLogic) {
      objectiveGameLogic.displayToUser = true;
      objectiveGameLogic.ShowObjective();
    }

    public void ShowContractObjective(ContractObjectiveGameLogic contractObjectiveGameLogic) {
      contractObjectiveGameLogic.LogObjective("Contract Objective Shown");
      contractObjectiveGameLogic.displayToUser = true;
      contractObjectiveGameLogic.currentObjectiveStatus = ObjectiveStatus.Active;
      EncounterLayerParent.EnqueueLoadAwareMessage(new ObjectiveUpdated(contractObjectiveGameLogic.encounterObjectGuid));
    }
  }
}
