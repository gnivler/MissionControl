using BattleTech.Framework;

namespace MissionControl.Conditional {
  public class EncounterConditionalBox : DesignConditionalBox {
    public EncounterConditionalBox() { }

    public EncounterConditionalBox(DesignConditional designConditional) {
      this.CargoVTwo = designConditional;
    }
  }
}