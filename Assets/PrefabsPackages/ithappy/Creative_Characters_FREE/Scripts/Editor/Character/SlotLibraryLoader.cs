using UnityEditor;

namespace PrefabPackages.ithappy.Creative_Characters_FREE.CharacterCustomizationTool.Editor.Character
{
    public static class SlotLibraryLoader
    {
        public static SlotLibrary LoadSlotLibrary()
        {
            return AssetDatabase.LoadAssetAtPath<SlotLibrary>(AssetsPath.SlotLibrary);
        }
    }
}