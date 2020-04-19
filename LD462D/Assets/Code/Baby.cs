using UnityEngine;
using System.Collections;
using System;

public class Baby: MonoBehaviour
{
    public float xPos;
    public float yPos;

    public Animator Idle;
    public Animator Death;

    public float xVel;
    public float yVel;

    public float groundedY;

    public ThoughBubble bubble;

    private CircleCollider2D babyCollider;
    private State state_;

    public Animator[] OneTimeAnimations;
    public Animator[] LoopingAnimations;

    private float timeHold_ = 0;
    private float timeIll_ = 0;

    private float randomTimeUntilIll_ = 4;

    public int GoalsHitBeforeLanding {get; set;} = 0;
    public CircleCollider2D BabyCollider {get{return babyCollider;}}

    public enum State
    {
        Sleep, 
        Idle, 
        Hold, 
        IllHold, 
        PukeHold,
        Airborne, 
        Dead, 
    }

    public void Awake()
    {
        babyCollider = GetComponent<CircleCollider2D>(); 
    }

    public void Update()
    {
        if (state_ == State.Dead)
            return;

        bool hold = state_ == State.Hold || state_ == State.IllHold || state_ == State.PukeHold;

        if (hold)
        {
            var collisionPointOffset = babyCollider.offset * transform.localScale;
            Vector3 newPos = new Vector3(xPos - collisionPointOffset.x, yPos - collisionPointOffset.y, 1); 
            transform.position = newPos;
            timeHold_ += Time.deltaTime;
        }

        if (state_ == State.Sleep)
        {
            bubble.gameObject.SetActive(true);
            bubble.UpdateSleep();
        }

        if (state_ == State.Hold)
        {
            bubble.UpdateCarried();
            SetLoopingAnimation(0);
            if (timeHold_ > randomTimeUntilIll_)
            {
                state_ = State.IllHold;
            }
        }
        else if (state_ == State.IllHold)
        {
            bubble.Hide();
            SetLoopingAnimation(3);
            timeIll_ += Time.deltaTime;

            if (timeIll_ > 0.8f)
            {
                state_ = State.PukeHold;
            }
        }
        else if (state_ == State.PukeHold)
        {
            SetLoopingAnimation(6);
        }
        else if (state_ == State.Airborne)
        {
            bubble.Hide();
            yVel -= 50 * Time.deltaTime;
            xVel *= 0.98f;

            var xMove = xVel * Time.deltaTime;

            if (transform.position.x + xMove < -15)
                xVel *= -1;
            else if (transform.position.x + xMove > 15)
                xVel *= -1;

            transform.Translate(new Vector3(xVel * Time.deltaTime, yVel * Time.deltaTime, 0));

            if (yVel < 0 && transform.position.y < groundedY)
            {
                state_ = State.Dead;
                transform.position = new Vector3(transform.position.x, groundedY, 0);
                PlayAnimationOnce(0);
            }
            else 
            {
                if (yVel < 0)
                    SetLoopingAnimation(2);
                else if (Mathf.Abs(xVel) > 0.05f)
                    SetLoopingAnimation(5);
                else 
                    SetLoopingAnimation(1);
            }
        }
    }

     private void PlayAnimationOnce(int index)
    {
        var animToActivate = OneTimeAnimations[index];

        foreach(var anim in OneTimeAnimations)
        { 
            if (animToActivate != anim)
            {
                anim.gameObject.SetActive(false);
            }
            else 
            {
                anim.gameObject.SetActive(true);
                animToActivate.Play("animation", -1, 0f);

            }
        }

        foreach(var anim in LoopingAnimations)
        { 
            anim.gameObject.SetActive(false);
        }
    }

    private void SetLoopingAnimation(int index)
    {
        var animToActivate = LoopingAnimations[index];

        foreach(var anim in LoopingAnimations)
        { 
            if (animToActivate == anim)
                anim.gameObject.SetActive(true);
            else
                anim.gameObject.SetActive(false);
        }
    }

    internal void SetState(State state)
    {
        state_ = state;
        if (state == State.Airborne)
        {
            GoalsHitBeforeLanding = 0;
            randomTimeUntilIll_ = UnityEngine.Random.Range(0, 3) + 3;
            timeHold_ = 0;
            timeIll_ = 0;
        }
    }

    internal State GetState()
    {
        return state_;
    }
}