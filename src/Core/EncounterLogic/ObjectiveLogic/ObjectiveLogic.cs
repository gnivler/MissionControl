namespace MissionControl.Logic {
  public abstract class ObjectiveLogic : LogicBlock {
    public ObjectiveLogic() {
      this.Type = LogicType.CONTRACT_OVERRIDE_MANIPULATION;
    }
  }
}