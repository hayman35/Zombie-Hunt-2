using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Create Enemy/EnemyStats", order = 0)]
public class EnemyStats : ScriptableObject 
{
    public string enemyName;
    public int maxHealth;
    
}
