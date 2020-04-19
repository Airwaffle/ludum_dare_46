using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public Player player;
    public Baby baby;
    private Renderer renderer_;

    void Awake()
    {
        renderer_ = GetComponent<Renderer>();
    }

    void Update()
    {
        if (baby != null)
        {
            transform.position = new Vector3(baby.transform.position.x +1, baby.groundedY, 0);
            var state = baby.GetState();
            
            //if (state == Baby.State.Dead || state == Baby.State.Sleep)
            if (state != Baby.State.Airborne)
                renderer_.enabled = false;
            else 
                renderer_.enabled = true;
        }
        else if (player != null)
        {
            transform.position = new Vector3(player.transform.position.x - 0.5f, player.transform.position.y - 1.95f, 0);
            var state = baby.GetState();
        }
    }
}
