using UnityEngine;

using Harmony;

using BattleTech.UI;

namespace MissionControl.Patches {
  [HarmonyPatch(typeof(LanceConfiguratorPanel), "ContinueConfirmClicked")]
  public class LanceConfiguratorPanelContinueConfirmClickedPatch {
    static void Postfix(LanceConfiguratorPanel __instance) {
      Main.LogDebug("[LanceConfiguratorPanelContinueConfirmClickedPatch] Patching");
      GameObject lanceHeaderWidgetGo = GameObject.Find("uixPrfPanel_LC_LanceConfigTopBar-Widget-MANAGED");
      if (lanceHeaderWidgetGo != null) {
        LanceHeaderWidget lanceHeaderWidget = lanceHeaderWidgetGo.GetComponent<LanceHeaderWidget>();
        SGDifficultyIndicatorWidget lanceRatingWidget = lanceHeaderWidget.lanceRatingWidget;

        MissionControl.Instance.PlayerLanceDropDifficultyValue = lanceRatingWidget.Difficulty;
        MissionControl.Instance.PlayerLanceDropSkullRating = lanceRatingWidget.Difficulty / 2f;
        CalculateTonnage();
      } else {
        Main.Logger.LogError("[LanceConfiguratorPanelContinueConfirmClickedPatch] Unable to get object 'uixPrfPanel_LC_LanceConfigTopBar-Widget-MANAGED'. Name must have changed in an BT update.");
      }
    }

    public static void CalculateTonnage() {
      GameObject lanceConfigPanelGo = GameObject.Find("uixPrfScrn_LC_LanceConfig-Screen(Clone)");
      if (lanceConfigPanelGo != null) {
        float tonnage = 0f;
        LanceConfiguratorPanel lanceConfigPanel = lanceConfigPanelGo.GetComponent<LanceConfiguratorPanel>();
        LanceLoadoutSlot[] loadoutSlots = lanceConfigPanel.loadoutSlots;

        foreach (LanceLoadoutSlot lanceLoadoutSlot in loadoutSlots) {
          if (lanceLoadoutSlot.SelectedMech != null) {
            tonnage += lanceLoadoutSlot.SelectedMech.MechDef.Chassis.Tonnage;
          }
        }

        MissionControl.Instance.PlayerLanceDropTonnage = tonnage;
      } else {
        Main.Logger.LogError("[CalculateTonnage] Unable to get object 'uixPrfScrn_LC_LanceConfig-Screen(Clone)'. Name must have changed in an BT update.");
      }
    }
  }
}