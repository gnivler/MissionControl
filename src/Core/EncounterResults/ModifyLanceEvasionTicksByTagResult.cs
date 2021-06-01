using BattleTech;
using BattleTech.Framework;

using HBS.Collections;

using Harmony;

using System;
using System.Collections.Generic;

namespace MissionControl.Result {
  public class ModifyLanceEvasionTicksByTagResult : EncounterResult {
    public string[] Tags { get; set; }
    public int Amount { get; set; }

    public override void Trigger(MessageCenterMessage inMessage, string triggeringName) {
      Main.LogDebug($"[ModifyLanceEvasionTicksByTagResult] Modifying evasion by '{Amount}' on combatants with tags '{String.Concat(Tags)}'");
      List<ICombatant> combatants = ObjectiveGameLogic.GetTaggedCombatants(UnityGameInstance.BattleTechGame.Combat, new TagSet(Tags));

      Main.LogDebug($"[ModifyLanceEvasionTicksByTagResult] Found'{combatants.Count}' combatants");

      foreach (ICombatant combatant in combatants) {
        if (combatant is AbstractActor) {
          AbstractActor actor = combatant as AbstractActor;

          actor.EvasivePipsCurrent += Amount;
          if (actor.EvasivePipsCurrent < 0) actor.EvasivePipsCurrent = 0;

          actor.EvasivePipsTotal = actor.EvasivePipsCurrent;
          UnityGameInstance.BattleTechGame.Combat.MessageCenter.PublishMessage(new EvasiveChangedMessage(actor.GUID, actor.EvasivePipsCurrent));
        }
      }
    }
  }
}
