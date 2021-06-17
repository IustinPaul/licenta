using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Rigidbody2D m_rbody2D;
    public float LifeSpan;

    public float Dmg;
    public float Speed;
    public float DirectionX;
    public float DirectionY;

    void Awake()
    {
        m_rbody2D = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        LifeSpan -= Time.deltaTime;
        if(LifeSpan <= 0)
        {
            Destroy(gameObject);
        }
        m_rbody2D.velocity = new Vector2(DirectionX, DirectionY) * Speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Speed > 0)
        {
            Destroy(gameObject);
        }
    }

}
