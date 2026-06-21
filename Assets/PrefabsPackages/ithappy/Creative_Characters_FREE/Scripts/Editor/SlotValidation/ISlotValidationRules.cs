using PrefabPackages.ithappy.Creative_Characters_FREE.CharacterCustomizationTool.Editor.Character;

namespace PrefabPackages.ithappy.Creative_Characters_FREE.CharacterCustomizationTool.Editor.SlotValidation
{
    public interface ISlotValidationRules
    {
        void Validate(CustomizableCharacter character, SlotType type, bool isToggled);
    }
}