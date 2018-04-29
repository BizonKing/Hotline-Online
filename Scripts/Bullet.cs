using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public int damage;
    public GameObject playerWhoFiredBullet;

    public void SetPlayerWhoFiredBullet(GameObject player)
    {
        playerWhoFiredBullet = player;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        var hit = collision.gameObject;
        // var health = hit.GetComponent<PlayerHealth>();
        var health = hit.GetComponent<Player>();
        if (health != null && hit != playerWhoFiredBullet)
        {
            health.TakeDamage(damage, playerWhoFiredBullet);
            Destroy(gameObject);
        }
        if (hit.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
