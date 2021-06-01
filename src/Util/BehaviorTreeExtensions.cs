using MissionControl;

public static class BehaviourTreeExtensions {
  public static BehaviorVariableValue GetCustomBehaviorVariableValue(this BehaviorTree tree, string key) {  
    return AiManager.Instance.GetBehaviourVariableValue(tree.unit, key);
  }
}