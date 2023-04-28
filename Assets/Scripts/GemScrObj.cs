using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GemType { Sphene, Potassium, Sapphire, Ruby };

[CreateAssetMenu(fileName = "Gem", menuName = "Gem")]
public class GemScrObj : ScriptableObject
{
    public GemType gemType;
    public int score;
    public string description;
    public Sprite sprite;
    public Color color;
}
