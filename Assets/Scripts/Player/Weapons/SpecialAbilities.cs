using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SpecialAbilities", menuName = "ScriptableObjects/SpecialAbilities")]
public class SpecialAbilities : ScriptableObject
{
    public Sprite uiSprite;

    public bool isLightAttack;

    public bool isAxe;
    
    public string title;

    public string description;

    public string animationHash;
}
