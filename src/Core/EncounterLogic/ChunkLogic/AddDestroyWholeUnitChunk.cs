using System.Collections.Generic;

using BattleTech;
using BattleTech.Designed;
using BattleTech.Framework;
using MissionControl.Rules;
using MissionControl.EncounterFactories;
using Harmony;

namespace MissionControl.Logic {
  public class AddDestroyWholeUnitChunk : ChunkLogic {
    private EncounterRules encounterRules;
    private string teamGuid;
    private string lanceGuid;
    private List<string> unitGuids;
    private string spawnerName;
    private string objectiveGuid;
    private string objectiveLabel;
    private int priority;
    private bool isPrimary;
    private bool displayToUser;
    private string contractObjectiveGameLogicGuid;

    public AddDestroyWholeUnitChunk(EncounterRules encounterRules, string teamGuid, string lanceGuid, List<string> unitGuids,
      string spawnerName, string objectiveGuid, string objectiveLabel, int priority, bool isPrimary, bool displayToUser, string contractObjectiveGameLogicGuid = null) {
      this.encounterRules = encounterRules;
      this.teamGuid = teamGuid;
      this.lanceGuid = lanceGuid;
      this.unitGuids = unitGuids;
      this.spawnerName = spawnerName;
      this.objectiveGuid = objectiveGuid;
      this.objectiveLabel = objectiveLabel;
      this.priority = priority;
      this.isPrimary = isPrimary;
      this.displayToUser = displayToUser;
      this.contractObjectiveGameLogicGuid = contractObjectiveGameLogicGuid;
    }

    public override void Run(RunPayload payload) {
      Main.Logger.Log($"[AddDestroyWholeUnitChunk] Adding encounter structure");
      EncounterLayerData encounterLayerData = MissionControl.Instance.EncounterLayerData;
      DestroyWholeLanceChunk destroyWholeChunk = ChunkFactory.CreateDestroyWholeLanceChunk();
      destroyWholeChunk.encounterObjectGuid = System.Guid.NewGuid().ToString();

      this.objectiveLabel = MissionControl.Instance.CurrentContract.Interpolate(this.objectiveLabel).ToString();

      bool spawnOnActivation = true;
      LanceSpawnerGameLogic lanceSpawner = LanceSpawnerFactory.CreateLanceSpawner(
        destroyWholeChunk.gameObject,
        spawnerName,
        lanceGuid,
        teamGuid,
        spawnOnActivation,
        SpawnUnitMethodType.InstantlyAtSpawnPoint,
        unitGuids
      );
      LanceSpawnerRef lanceSpawnerRef = new LanceSpawnerRef(lanceSpawner);

      bool showProgress = true;
      DestroyLanceObjective objective = ObjectiveFactory.CreateDestroyLanceObjective(
        objectiveGuid,
        destroyWholeChunk.gameObject,
        lanceSpawnerRef,
        lanceGuid,
        objectiveLabel,
        showProgress,
        ProgressFormat.PERCENTAGE_COMPLETE,
        "The primary objective to destroy the enemy lance",
        priority,
        displayToUser,
        ObjectiveMark.AttackTarget,
        contractObjectiveGameLogicGuid,
        Main.Settings.ActiveAdditionalLances.GetRewards()
      );

      if (isPrimary)
      {
        objective.primary = true;
      } else {
        objective.primary = false;
      }

      DestroyLanceObjectiveRef destroyLanceObjectiveRef = new DestroyLanceObjectiveRef();
      destroyLanceObjectiveRef.encounterObject = objective;

      destroyWholeChunk.lanceSpawner = lanceSpawnerRef;
      destroyWholeChunk.destroyObjective = destroyLanceObjectiveRef;
    }
  }
}