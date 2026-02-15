using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trials", menuName = "ScriptableObjects/Trials")]
public class Trials : ScriptableObject
{
    public string title;
    
    public string description;
    
    public List<Trials> subTrials = new List<Trials>();

    public bool isCompleted;
}
