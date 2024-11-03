using UnityEngine;

public class Slow : MonoBehaviour
{
    private TowerWeapon towerWeapon;

    private void Awake()
    {
        towerWeapon = GetComponentInParent<TowerWeapon>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
        {
            return;
        }

        Movement2D movement2D = collision.GetComponent<Movement2D>();
        movement2D.SlowCount++;

        movement2D.MoveSpeed -= movement2D.MoveSpeed * towerWeapon.Slow;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!collision.CompareTag("Enemy"))
        {
            return;
        }

        Movement2D movement2D = collision.GetComponent<Movement2D>();
        movement2D.SlowCount--;

        if(movement2D.SlowCount == 0)
            collision.GetComponent<Movement2D>().ResetMoveSpeed();
    }
}
