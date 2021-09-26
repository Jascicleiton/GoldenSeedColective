[System.Serializable]

public struct CharacterAttribute 
{
    public CharacterPartAnimator characterPart;
    public PartVariantColor partVariantColor;
    public PartVariantType partVariantType;

    public CharacterAttribute(CharacterPartAnimator characterPart, PartVariantColor partVariantColor, PartVariantType partVariantType)
    {
        this.characterPart = characterPart;
        this.partVariantColor = partVariantColor;
        this.partVariantType = partVariantType;
    }
}
