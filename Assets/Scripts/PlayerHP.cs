using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private Image imageScreen;
    [SerializeField] private float maxHP = 20;
    private float currentHP;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        StopCoroutine(HitAlphaAnimation());
        StartCoroutine(HitAlphaAnimation());

        if(currentHP <= 0)
        {

        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        Color color = imageScreen.color;

        color.a = 0.4f;

        imageScreen.color = color;

        while (color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            imageScreen.color = color;

            yield return null;
        }
    }
}
