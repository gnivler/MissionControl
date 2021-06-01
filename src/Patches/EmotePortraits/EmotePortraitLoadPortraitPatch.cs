using UnityEngine;
using System.IO;
using Harmony;

using BattleTech;

namespace MissionControl.Patches {
  [HarmonyPatch(typeof(EmotePortrait), "LoadPortrait")]
  public class EmotePortraitLoadPortraitPatch {
    static void Postfix(EmotePortrait __instance, ref Sprite __result) {
      string path = Utilities.PathUtils.AppendPath(Main.Path, __instance.portraitAssetPath, false);
      if (File.Exists(path)) {
				__result = Utilities.ImageUtils.LoadSprite(path);
			}
    }
  }
}