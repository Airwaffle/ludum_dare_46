using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private int dir_;
    private float speed_;

    private float scale_; 

    void Awake()
    {
        speed_ = UnityEngine.Random.Range(2, 3); 
        dir_ = UnityEngine.Random.Range(0,1) > 0.5 ? 1 : -1;
        scale_ = transform.localScale.x;
        transform.localScale = new Vector3(scale_ * dir_, transform.localScale.y, transform.localScale.z);
    }

    void Update()
    {
        var xMove = speed_ * Time.deltaTime * dir_;
        transform.Translate(new Vector3(xMove, 0, 0));


        if (dir_ == 1 && transform.position.x > 15)
        {
            dir_ = -1;
            transform.localScale = new Vector3(scale_ * dir_, transform.localScale.y, transform.localScale.z);
        }
        else if (dir_ == -1 && transform.position.x < - 10)
        {
            dir_ = 1;
            transform.localScale = new Vector3(scale_ * dir_, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var baby = other.collider.GetComponent<Baby>();
        if (baby.GetState() != Baby.State.Dead)
        {
            if (baby.yVel > 0)
                baby.yVel *= 0.2f;

            Debug.Log("Hit bird");
        }
    }
}
