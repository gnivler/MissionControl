using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using BattleTech;
using BattleTech.Designed;

using MissionControl.Rules;
using MissionControl.Utils;

namespace MissionControl.Logic {
  public class SpawnLanceMembersAroundTarget : SpawnLanceLogic {
    private string lanceKey = "";
    private string orientationTargetKey = "";
    private string lookTargetKey = "";

    private GameObject lance;
    private GameObject orientationTarget;
    private GameObject lookTarget;
    private LookDirection lookDirection;
    private float minDistanceFromTarget = 50f;
    private float maxDistanceFromTarget = 100f;
    private float minDistanceToSpawnFromInvalidSpawn = 30f;
    private List<Vector3> invalidSpawnLocations = new List<Vector3>();

    public SpawnLanceMembersAroundTarget(EncounterRules encounterRules, string lanceKey, string orientationTargetKey, LookDirection lookDirection) :
      this(encounterRules, lanceKey, orientationTargetKey, lookDirection, 10, 10) { } // TODO: Replace the hard coded values with a setting.json setting

    public SpawnLanceMembersAroundTarget(EncounterRules encounterRules, string lanceKey, string orientationTargetKey, LookDirection lookDirection, float minDistance, float maxDistance) :
      this(encounterRules, lanceKey, orientationTargetKey, orientationTargetKey, lookDirection, minDistance, maxDistance) { }

    public SpawnLanceMembersAroundTarget(EncounterRules encounterRules, string lanceKey, string orientationTargetKey, string lookTargetKey, LookDirection lookDirection, float minDistance, float maxDistance) : base(encounterRules) {
      this.lanceKey = lanceKey;
      this.orientationTargetKey = orientationTargetKey;
      this.lookTargetKey = lookTargetKey;
      this.lookDirection = lookDirection;
      minDistanceFromTarget = minDistance;
      maxDistanceFromTarget = maxDistance;
    }

    public override void Run(RunPayload payload) {
      GetObjectReferences();
      Main.Logger.Log($"[SpawnLanceMembersAroundTarget] For {lance.name}");
      lance.transform.position = orientationTarget.transform.position;
  
      List<GameObject> spawnPoints = lance.FindAllContains("SpawnPoint");
      foreach (GameObject spawnPoint in spawnPoints) {
        SpawnLanceMember(spawnPoint, orientationTarget, lookTarget, lookDirection);
      }

      invalidSpawnLocations.Clear();
    }

    protected override void GetObjectReferences() {
      this.EncounterRules.ObjectLookup.TryGetValue(lanceKey, out lance);
      this.EncounterRules.ObjectLookup.TryGetValue(orientationTargetKey, out orientationTarget);
      this.EncounterRules.ObjectLookup.TryGetValue(lookTargetKey, out lookTarget);

      if (lance == null || orientationTarget == null || lookTarget == null) {
        Main.Logger.LogError("[SpawnLanceMembersAroundTarget] Object references are null");
      }
    }

    public void SpawnLanceMember(GameObject spawnPoint, GameObject orientationTarget, GameObject lookTarget, LookDirection lookDirection) {
      CombatGameState combatState = UnityGameInstance.BattleTechGame.Combat;
      MissionControl encounterManager = MissionControl.Instance;

      Vector3 spawnPointPosition = combatState.HexGrid.GetClosestPointOnGrid(spawnPoint.transform.position);
      Vector3 newSpawnPosition = GetRandomPositionFromTarget(orientationTarget, minDistanceFromTarget, maxDistanceFromTarget);
      newSpawnPosition.y = combatState.MapMetaData.GetLerpedHeightAt(newSpawnPosition);

      if (encounterManager.EncounterLayerData.IsInEncounterBounds(newSpawnPosition)) {
        if (!IsWithinDistanceOfInvalidPosition(newSpawnPosition)) {      
          spawnPoint.transform.position = newSpawnPosition;
          invalidSpawnLocations.Add(newSpawnPosition);

          if (lookDirection == LookDirection.TOWARDS_TARGET) {
            RotateToTarget(spawnPoint, lookTarget);
          } else {
            RotateAwayFromTarget(spawnPoint, lookTarget);
          }

          if (!IsSpawnValid(spawnPoint, orientationTarget)) {
            SpawnLanceMember(spawnPoint, orientationTarget, lookTarget, lookDirection);
          } else {
            Main.Logger.Log("[SpawnLanceMembersAroundTarget] Lance member spawn complete");
          }
        } else {
          Main.Logger.LogWarning("[SpawnLanceMembersAroundTarget] Cannot spawn a lance member on an invalid spawn. Finding new spawn point.");
          SpawnLanceMember(spawnPoint, orientationTarget, lookTarget, lookDirection);
        }
      } else {
        Main.Logger.LogWarning("[SpawnLanceMembersAroundTarget] Selected lance spawn point is outside of the boundary. Select a new lance spawn point.");
        SpawnLanceMember(spawnPoint, orientationTarget, lookTarget, lookDirection);  
      }
    }

    private bool IsWithinDistanceOfInvalidPosition(Vector3 newSpawn) {
      foreach (Vector3 invalidSpawn in invalidSpawnLocations) {
        Vector3 vectorToInvalidSpawn = newSpawn - invalidSpawn;
        vectorToInvalidSpawn.y = 0;
        float disatanceToInvalidSpawn = vectorToInvalidSpawn.magnitude;
        if (disatanceToInvalidSpawn < minDistanceToSpawnFromInvalidSpawn) return true;
      }
      return false;
    }
  }
}