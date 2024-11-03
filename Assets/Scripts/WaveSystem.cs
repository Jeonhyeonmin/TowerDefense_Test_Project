using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private EnemySpanwer enemySpawner;
    private int currentWaveIndex = -1;

    public int CurrentWave => currentWaveIndex + 1;
    public int MaxWave => waves.Length;

    public void StartWave()
    {
        if(enemySpawner.EnemyList.Count == 0 && currentWaveIndex < waves.Length -1)
        {
            currentWaveIndex++;
            enemySpawner.StartWave(waves[currentWaveIndex]);
        }
    }

}

[System.Serializable]
public struct Wave
{
    public float spawnTime;
    public int maxEnemyCount;
    public GameObject[] enemyPrefabs;
}
