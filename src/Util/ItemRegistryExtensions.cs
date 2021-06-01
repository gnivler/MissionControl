using System.Collections.Generic;
using Harmony;

public static class ItemRegistryExtensions {
  public static void RemoveItem(this ItemRegistry registry, string guid)
  {
    Dictionary<string, ITaggedItem> itemsByGuid = registry.itemsByGuid;
    Dictionary<TaggedObjectType, List<ITaggedItem>> itemsByType = registry.itemsByType;

    if (itemsByGuid.ContainsKey(guid)) {
      ITaggedItem item = itemsByGuid[guid];
      itemsByGuid.Remove(guid);

      if (itemsByType.ContainsKey(item.Type)) {
        itemsByType[item.Type].Remove(item);
      }
    }
  }
}