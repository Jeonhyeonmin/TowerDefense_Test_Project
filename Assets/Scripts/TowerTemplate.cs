using UnityEngine;

[CreateAssetMenu]
public class TowerTemplate : ScriptableObject
{
    public GameObject towerPrefab;
    public GameObject followTowerPrefab;
    public Weapon[] weapon;

    [System.Serializable]
    public struct Weapon
    {
        public Sprite sprite;
        public float damage;
        public float slow; // 0.2면 20%
        public float buff; // 공격력 증가율 (0.2 == +20%)
        public float rate;
        public float range;
        public int cost;
        public int sell;
    }
}
