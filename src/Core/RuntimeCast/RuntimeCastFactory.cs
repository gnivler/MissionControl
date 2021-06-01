using BattleTech;

using HBS.Data;

using MissionControl.Rules;

namespace MissionControl.RuntimeCast {
  public class RuntimeCastFactory {
    public static CastDef CreateCast() {
      Contract contract = MissionControl.Instance.CurrentContract;
      FactionValue employerFaction = contract.GetTeamFaction(EncounterRules.EMPLOYER_TEAM_ID);
      string factionId = employerFaction.FactionDefID;
      string employerFactionName = "Military Support";

      if (employerFaction.Name != "INVALID_UNSET" && employerFaction.Name != "NoFaction") {
        FactionDef employerFactionDef = UnityGameInstance.Instance.Game.DataManager.Factions.Get(factionId);
        if (employerFactionDef == null) Main.Logger.LogError($"[RuntimeCastFactory] Error finding FactionDef for faction with id '{factionId}'");
        employerFactionName = employerFactionDef.Name.ToUpper();
      }

      string employerFactionKey = (employerFaction.Name != "INVALID_UNSET" && employerFaction.Name != "NoFaction") ? "All" : employerFaction.ToString();

      string gender = DataManager.Instance.GetRandomGender();
      string firstName = DataManager.Instance.GetRandomFirstName(gender, employerFactionKey);
      string lastName = DataManager.Instance.GetRandomLastName(employerFactionKey);
      string rank = DataManager.Instance.GetRandomRank(employerFactionKey);
      string portraitPath = DataManager.Instance.GetRandomPortraitPath(gender);
      Gender btGender = Gender.Male;
      if (gender == "Female") btGender = Gender.Female;
      if (gender == "Unspecified") btGender = Gender.NonBinary;

      CastDef runtimeCastDef = new CastDef();
      // Temp test data
      runtimeCastDef.id = $"castDef_{rank}{firstName}{lastName}";
      runtimeCastDef.internalName = $"{rank}{firstName}{lastName}";
      runtimeCastDef.firstName = $"{rank} {firstName}";
      runtimeCastDef.lastName = lastName;
      runtimeCastDef.callsign = rank;
      runtimeCastDef.rank = employerFactionName;
      runtimeCastDef.gender = btGender;
      runtimeCastDef.FactionValue = employerFaction;
      runtimeCastDef.showRank = true;
      runtimeCastDef.showFirstName = true;
      runtimeCastDef.showCallsign = false;
      runtimeCastDef.showLastName = true;
      runtimeCastDef.defaultEmotePortrait.portraitAssetPath = portraitPath;

      ((DictionaryStore<CastDef>)UnityGameInstance.BattleTechGame.DataManager.CastDefs).Add(runtimeCastDef.id, runtimeCastDef);

      return runtimeCastDef;
    }
  }
}