using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Blessings", menuName = "ScriptableObjects/Blessings")]
public class Blessings : ScriptableObject
{
    public Sprite uiSprite;

    public bool isLightAttack;

    public bool isAxe;
    
    public string title;

    public string description;

    public string animationHash;
}
