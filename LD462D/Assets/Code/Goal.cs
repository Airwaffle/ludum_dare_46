using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    
    public Animator[] GoalTypes;

    public GameObject ExplosionPrefab;

    private int randomType_;
    private CameraScript camera_;

    private Main main_;

    public int Score {get; private set;}

    void Awake()
    {
        randomType_ = (int)UnityEngine.Random.Range(0, GoalTypes.Length);

        foreach (var t in GoalTypes)
            t.gameObject.SetActive(false);

        GoalTypes[randomType_].gameObject.SetActive(true);
        camera_ = (FindObjectsOfType(typeof(CameraScript)) as CameraScript[])[0];
        main_ = (FindObjectsOfType(typeof(Main)) as Main[])[0];
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var baby = other.collider.GetComponent<Baby>();
        baby.GoalsHitBeforeLanding++;
        if (baby.bubble.RandomWish == randomType_)
        {
            baby.yVel = GetBabyBump();
            camera_.ZoomObject(1);
            Score += 1000 * baby.GoalsHitBeforeLanding;
        }
        else 
        {
            Score += 200 * baby.GoalsHitBeforeLanding;
        }
        gameObject.SetActive(false);
        Instantiate(ExplosionPrefab, transform.position, transform.rotation);
    }

    private float GetBabyBump()
    {
        return Mathf.Min(main_.GoalsTaken * 5, 60) + 20;
    }
}
