using UnityEngine;

[CreateAssetMenu(menuName = "GreenClouds/Character Stats")]
public class CharacterStats : ScriptableObject
{
    [Header("Player")]
    public int maxHealth = 100;
    public float moveSpeed = 5f;
    public int clickDamage = 1;

    [Header("Ultimate")]
    public int chargeNeeded = 10;     
    public int chargeAddPerClick = 1; 
    public int ultimateDamage = 50;
    public float ultimateRadius = 3f;
}
