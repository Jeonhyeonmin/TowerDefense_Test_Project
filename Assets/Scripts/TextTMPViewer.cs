using TMPro;
using UnityEngine;

public class TextTMPViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPlayerHP;
    [SerializeField] private PlayerHP playerHP;
    [SerializeField] private TextMeshProUGUI textPlayerGOLD;
    [SerializeField] private TextMeshProUGUI textWave;
    [SerializeField] private TextMeshProUGUI textEnemyCount;
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private WaveSystem waveSystem;
    [SerializeField] private EnemySpanwer enemySpawner;

    private void Update()
    {
        textPlayerHP.text = playerHP.CurrentHP + "/" + playerHP.MaxHP;
        textPlayerGOLD.text = playerGold.CurrentGold.ToString();
        textWave.text = waveSystem.CurrentWave + "/" + waveSystem.MaxWave;
        textEnemyCount.text = enemySpawner.CurrentEnemyCount + "/" + enemySpawner.MaxEnemyCount;
    }
}
