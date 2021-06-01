using System.Collections.Generic;

public static class ListExtensions {
  public static T GetRandom<T>(this List<T> list) {
    if (list.Count >= 0) {
      return list[UnityEngine.Random.Range(0, list.Count)];  
    }

    return default(T);
  }
}