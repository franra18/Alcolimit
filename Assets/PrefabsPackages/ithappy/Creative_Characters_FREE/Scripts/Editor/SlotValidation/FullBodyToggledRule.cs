using System.Linq;
using PrefabPackages.ithappy.Creative_Characters_FREE.CharacterCustomizationTool.Editor.Character;

namespace PrefabPackages.ithappy.Creative_Characters_FREE.CharacterCustomizationTool.Editor.SlotValidation
{
    public class FullBodyToggledRule : ISlotValidationRules
    {
        private readonly SlotType[] _slotExceptions =
        {
            SlotType.FullBody,
            SlotType.Body,
            SlotType.Faces,
        };

        public void Validate(CustomizableCharacter character, SlotType type, bool isToggled)
        {
            if (type != SlotType.FullBody || !isToggled)
            {
                return;
            }

            foreach (var slot in character.Slots.Where(s => !_slotExceptions.Contains(s.Type)))
            {
                slot.Toggle(false);
            }
        }
    }
}