using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using BattleTech;
using BattleTech.Designed;
using BattleTech.Framework;

using MissionControl.Logic;
using MissionControl.Rules;
using MissionControl.EncounterFactories;
using MissionControl.LogicComponents.Spawners;
using MissionControl.Utils;

namespace MissionControl.Logic {
  public class AddPlayerLanceAiSpawnChunk : ChunkLogic {
    private string teamGuid;
    private string lanceGuid;
    private List<string> unitGuids;
    private string spawnerName;
    private string debugDescription;

    public AddPlayerLanceAiSpawnChunk(string teamGuid, string lanceGuid, List<string> unitGuids, string spawnerName, string debugDescription) {
      this.teamGuid = teamGuid;
      this.lanceGuid = lanceGuid;
      this.unitGuids = unitGuids;
      this.spawnerName = spawnerName;
      this.debugDescription = debugDescription;
    }

    public override void Run(RunPayload payload) {
      Main.Logger.Log($"[AddPlayerLanceAiSpawnChunk] Adding encounter structure");
      EncounterLayerData encounterLayerData = MissionControl.Instance.EncounterLayerData;
      EmptyCustomChunkGameLogic emptyCustomChunk = ChunkFactory.CreateEmptyCustomChunk("Chunk_Lance");
      emptyCustomChunk.encounterObjectGuid = System.Guid.NewGuid().ToString();
      emptyCustomChunk.notes = debugDescription;

      bool spawnOnActivation = true;
      PlayerLanceAiSpawnerGameLogic lanceSpawner = LanceSpawnerFactory.CreatePlayerAiLanceSpawner(
        emptyCustomChunk.gameObject,
        spawnerName,
        lanceGuid,
        teamGuid,
        spawnOnActivation,
        SpawnUnitMethodType.InstantlyAtSpawnPoint,
        unitGuids
      );
      lanceSpawner.transform.position = Vector3.zero;
    }
  }
}