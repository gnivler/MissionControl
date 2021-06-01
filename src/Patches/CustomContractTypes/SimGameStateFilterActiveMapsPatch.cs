using Harmony;

using BattleTech;
using BattleTech.Data;
using BattleTech.Framework;

using System.Linq;
using System.Collections.Generic;

namespace MissionControl.Patches {
  [HarmonyPatch(typeof(SimGameState), "FilterActiveMaps")]
  public class SimGameStateFilterActiveMapsPatch {
    static void Postfix(SimGameState __instance, ref WeightedList<MapAndEncounters> activeMaps, List<Contract> currentContracts) {
      // Main.LogDebug($"[SimGameStateFilterActiveMapsPatch.Postfix] Running SimGameStateFilterActiveMapsPatch");
      FixActiveMapWeights(activeMaps);
      FilterOnMapsWithEncountersWithValidContractRequirements(__instance, activeMaps, currentContracts);

      if (activeMaps.Count <= 0) HandleLackOfContractsSituation(__instance, activeMaps, currentContracts);
    }

    private static void FixActiveMapWeights(WeightedList<MapAndEncounters> activeMaps) {
      foreach (MapAndEncounters level in activeMaps) {
        int indexOfLevelInActiveList = -1;
        int weight = level.Map.Weight;

        // Get index1 of element in the rootList
        int indexOfLevelInRootList = activeMaps.rootList.IndexOf(level);

        // Get index2 of element in activeList (if it contains it)
        if (activeMaps.activeList.Contains(level)) {
          indexOfLevelInActiveList = activeMaps.activeList.IndexOf(level);
        }

        // Set the weight in the rootWeights in index1 position
        List<int> rootWeights = activeMaps.rootWeights;
        rootWeights[indexOfLevelInRootList] = weight;

        // if index2 is >= 0, remove from index2 of activeWeights
        if (indexOfLevelInActiveList >= 0)
        {
          List<int> activeWeights = activeMaps.activeWeights;
          activeWeights[indexOfLevelInActiveList] = weight;
        }
      }
    }

    private static void FilterOnMapsWithEncountersWithValidContractRequirements(SimGameState simGameState, WeightedList<MapAndEncounters> activeMaps, List<Contract> currentContracts) {
      List<MapAndEncounters> mapsToRemove = new List<MapAndEncounters>();

      StarSystem system = MissionControl.Instance.System;
      var validParticipants = simGameState.GetValidParticipants(system);

      for (int i = 0; i < activeMaps.Count; i++) {
        MapAndEncounters level = activeMaps[i];
        bool removeMap = true;

        foreach (EncounterLayer_MDD encounterLayerMDD in level.Encounters) {
          int contractTypeId = (int)encounterLayerMDD.ContractTypeRow.ContractTypeID;

          // If the encounter ContractTypeID exists in the potential contracts list, continue
          if (MissionControl.Instance.PotentialContracts.ContainsKey(contractTypeId)) {
            // If the contract overrides in the potential contracts by ContractTypeID has a `DoesContractMeetRequirements` sucess, mark remove = false
            List<ContractOverride> contractOverrides = MissionControl.Instance.PotentialContracts[contractTypeId];
            // Main.LogDebug($"[FilterOnMapsWithEncountersWithValidContractRequirements] '{contractTypeId}' - contractOverrides count is: {contractOverrides.Count}");
            for (int j = contractOverrides.Count; j > 0; j--) {
              ContractOverride contractOverride = contractOverrides[j - 1];
              // Main.LogDebug($"[FilterOnMapsWithEncountersWithValidContractRequirements] '{contractTypeId}' - contractOverride is: {contractOverride.ID}");
              // Main.LogDebug($"[FilterOnMapsWithEncountersWithValidContractRequirements] '{contractTypeId}' - validParticipants is: {validParticipants}");
              bool doesContractHaveValidFactions = simGameState.GetValidFaction(system, validParticipants, contractOverride.requirementList, out _);
              // Main.LogDebug($"[FilterOnMapsWithEncountersWithValidContractRequirements] '{contractTypeId}' - Contract '{contractOverride.ID}' has valid fations?: {doesContractHaveValidFactions}");
              if (!doesContractHaveValidFactions) {
                // Main.LogDebug($"[FilterOnMapsWithEncountersWithValidContractRequirements] '{contractTypeId}' - Removing Contract '{contractOverride.ID}' from potential list");
                contractOverrides.RemoveAt(j - 1);
                continue;
              }

              bool doesContractMeetReqs = simGameState.DoesContractMeetRequirements(system, level, contractOverride);
              if (doesContractMeetReqs) {
                // At least one contract override meets the requirements to prevent the infinite spinner so ignore this logic now and continue to the next map/encounter combo
                // Main.LogDebug($"[FilterOnMapsWithEncountersWithValidContractRequirements] '{contractTypeId}' - Level '{level.Map.MapName}.{encounterLayerMDD.Name}' has at least one valid contract override");
                removeMap = false;
                break;
              }
            }
          }
        }

        if (removeMap) {
          // Main.LogDebug($"[FilterOnMapsWithEncountersWithValidContractRequirements] Level '{level.Map.MapName}' had no encounters with any valid contract overrides. Removing map.");
          mapsToRemove.Add(level);
        }
      }

      // Remove maps that have no valid contracts due to failing requirements
      foreach (MapAndEncounters level in mapsToRemove) {
        // Main.LogDebug($"[FilterOnMapsWithEncountersWithValidContractRequirements] Attempting to remove Level '{level.Map.MapName}'");
        activeMaps.Remove(level);
      }

      // Main.LogDebug($"[FilterOnMapsWithEncountersWithValidContractRequirements] There are '{activeMaps.Count}' active maps/encounter combos to use. These are:");
      /*
      for (int k = 0; k < activeMaps.Count; k++) {
        MapAndEncounters level = activeMaps[k];
        Main.LogDebug($"[FilterOnMapsWithEncountersWithValidContractRequirements] - '{level.Map.MapName}' with '{level.Encounters.Length}' encounters");
        foreach (EncounterLayer_MDD encounterLayerMDD in level.Encounters) {
          Main.LogDebug($"[FilterOnMapsWithEncountersWithValidContractRequirements]   - Encounter '{encounterLayerMDD.Name}'");
        }
      }
      */
    }

    private static void HandleLackOfContractsSituation(SimGameState simGameState, WeightedList<MapAndEncounters> activeMaps, List<Contract> currentContracts) {
      // If there are no more active maps, reset the biomes/maps list
      Main.Logger.LogWarning($"[FilterOnMapsWithEncountersWithValidContractRequirements] No valid map/encounter combinations. Handling lack of map/encounter situation.");
      StarSystem system = MissionControl.Instance.System;
      List<string> mapDiscardPile = simGameState.mapDiscardPile;
      mapDiscardPile.Clear();

      WeightedList<MapAndEncounters> playableMaps = SimGameState.GetSinglePlayerProceduralPlayableMaps(system);
      IEnumerable<int> source = from map in playableMaps select map.Map.Weight;
      WeightedList<MapAndEncounters> weightedList = new WeightedList<MapAndEncounters>(WeightedListType.WeightedRandom, playableMaps.ToList(), source.ToList<int>(), 0);

      activeMaps.AddRange(weightedList);

      Main.Logger.LogWarning($"[FilterOnMapsWithEncountersWithValidContractRequirements] Running fresh map list over post processing to ensure no contracts screen freezes");
      HandleContractRepeats(simGameState, activeMaps);
      FilterOnMapsWithEncountersWithValidContractRequirements(simGameState, activeMaps, currentContracts);
      FixActiveMapWeights(activeMaps);
    }

    private static void HandleContractRepeats(SimGameState simGameState, WeightedList<MapAndEncounters> activeMaps)
    {
      List<string> mapDiscardPile = simGameState.mapDiscardPile;
      for (int num = activeMaps.Count - 1; num >= 0; num--) {
        Map_MDD map = activeMaps[num].Map;
        bool doesActiveFlashpointUseSameMap = simGameState.DoesActiveFlashpointUseSameMap(map._mapName);

        if (mapDiscardPile.Contains(map.MapID) || doesActiveFlashpointUseSameMap) {
          activeMaps.RemoveAt(num);
        }
      }
    }
  }
}