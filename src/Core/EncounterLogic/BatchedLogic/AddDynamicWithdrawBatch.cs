using System.Linq;
using BattleTech;
using BattleTech.Framework;

using MissionControl.Rules;
using MissionControl.Trigger;
using MissionControl.Messages;

namespace MissionControl.Logic {
  public class AddDynamicWithdrawBatch {
    public LogicState state = new LogicState();

    public AddDynamicWithdrawBatch(EncounterRules encounterRules) {
      Main.Logger.Log($"[{this.GetType().Name}] Building Dynamic Withdraw");

      encounterRules.EncounterLogic.Add(new DoesChunkExist(state, "Chunk_Withdraw"));
      encounterRules.EncounterLogic.Add(new AddWithdrawChunk(state, ChunkLogic.DYNAMIC_WITHDRAW_CHUNK_GUID, ChunkLogic.DYNAMIC_WITHDRAW_OBJECTIVE_GUID, ChunkLogic.DYNAMIC_WITHDRAW_REGION_GUID));
      encounterRules.EncounterLogic.Add(new WithdrawChunkTrigger((MessageCenterMessageType)MessageTypes.OnChunkActivated, ChunkLogic.DYNAMIC_WITHDRAW_CHUNK_GUID));
      encounterRules.EncounterLogic.Add(new EndCombatTrigger(MessageCenterMessageType.OnObjectiveSucceeded, ChunkLogic.DYNAMIC_WITHDRAW_OBJECTIVE_GUID, EndCombatTrigger.EndCombatType.RETREAT));
      AddDialogueChunk(encounterRules);

      encounterRules.EncounterLogic.Add(new DialogTrigger((MessageCenterMessageType)MessageTypes.OnChunkActivated, ChunkLogic.DIALOGUE_DYNAMIC_WITHDRAW_ESCAPE_GUID));
    }

    private DialogueOverride GetPresetDynamicWithdrawDialogue() {
      DialogueOverride dialogueOverride = MissionControl.Instance.CurrentContract.Override.dialogueList.FirstOrDefault(
        dialogue => dialogue.GUID == ChunkLogic.DIALOGUE_DYNAMIC_WITHDRAW_ESCAPE_GUID);
      return dialogueOverride;
    }

    private void AddDialogueChunk(EncounterRules encounterRules) {
      DialogueOverride dialogueOverride = GetPresetDynamicWithdrawDialogue();

      if (dialogueOverride == null) {
        encounterRules.EncounterLogic.Add(new AddDialogueChunk(
          ChunkLogic.DIALOGUE_DYNAMIC_WITHDRAW_ESCAPE_GUID,
          "DynamicWithdrawEscape",
          "Start Conversation For Dynamic Withdraw Escape",
          ChunkLogic.DYNAMIC_WITHDRAW_REGION_GUID,
          true,
          "I'm coming in hot. Get to the EZ as soon as you can, Commander",
          UnityGameInstance.BattleTechGame.DataManager.CastDefs.Get("castDef_SumireDefault")
        ));
      } else {
        encounterRules.EncounterLogic.Add(new AddDialogueChunk(
          ChunkLogic.DIALOGUE_DYNAMIC_WITHDRAW_ESCAPE_GUID,
          "DynamicWithdrawEscape",
          "Start Conversation For Dynamic Withdraw Escape",
          dialogueOverride
        ));
      }
    }
  }
}