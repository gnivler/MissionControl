using BattleTech.Framework;

namespace MissionControl.Result {
  public class EncounterResultBox : DesignResultBox {
    public EncounterResultBox() { }

    public EncounterResultBox(DesignResult designResult) {
      this.CargoVTwo = designResult;
    }
  }
}