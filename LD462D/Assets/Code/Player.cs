using UnityEngine;
using System.Collections;
using System;

public class Player: MonoBehaviour
{
    public float xPos = 0;

    public float xspeed = 3;
    public float xVel = 0;

    public Animator[] OneTimeAnimations;
    public Animator[] LoopingAnimations;

    public SpriteRenderer PukeRender;

    public enum State
    {
        Idle, 
        Walking, 
        Throwing,
        Crying
    }

    private State state_ = State.Idle;

    private Baby baby_;
    private Baby carriedBaby_;
    private Renderer currentAnimation_;
    private CircleCollider2D currentGrabCollider_;

    private Baby thrownBaby_;
    private Main main_;
    private float timeSpaceDown_ = 0;
    private float timeBabyDead_ = 0;
    private bool firstTime_ = true;
    private bool firstSpace_ = true;
    private bool gotPuked_ = false;

    private float timeTilCry_ = 1;

    public void Awake()
    {
        currentGrabCollider_ = GetComponent<CircleCollider2D>();
        main_ = (FindObjectsOfType(typeof(Main)) as Main[])[0];
        baby_ = (FindObjectsOfType(typeof(Baby)) as Baby[])[0];

        xPos = transform.position.x;
    }

    public void Update()
    {
        if (state_ == State.Crying)
        {
            return;
        }

        if (thrownBaby_?.GetState() == Baby.State.Dead)
        {
            timeBabyDead_ += Time.deltaTime;

            if (timeBabyDead_ > timeTilCry_)
            {
                timeTilCry_ = 0.2f;
                state_ = State.Crying;
                SetLoopingAnimation(1);
                return;    
            }
        }

        if (Input.GetKey(KeyCode.W))
        {
           
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            xVel -= xspeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            
        }
        if (Input.GetKey(KeyCode.D)|| Input.GetKey(KeyCode.RightArrow))
        {
            xVel += xspeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if (carriedBaby_ != null)
            {
                timeSpaceDown_ += Time.deltaTime;
                if (state_ == State.Idle || state_ == State.Walking || state_ == State.Throwing)
                {   
                    state_ = State.Throwing;
                    PlayAnimationOnce(0);
                }
            }
            else if (firstSpace_)
            {
                if (Vector3.Distance(currentGrabCollider_.bounds.center, baby_.BabyCollider.bounds.center) < 4.6)
                {
                    TryStartCarryBaby_(baby_);
                }
            }
            firstSpace_ = false;
        }
        else 
        {
            firstSpace_ = true;
        }

        xVel *= 0.9f;
        xPos += xVel;

        if (xVel > 0)
            transform.localScale = new Vector3(transform.localScale.y, transform.localScale.y, transform.localScale.z);
        else if (xVel < 0)
            transform.localScale = new Vector3(transform.localScale.y *-1, transform.localScale.y, transform.localScale.z);

            if (xPos < -15)
                xPos = -15;
            else if (xPos > 15)
                xPos = 15;


        Vector3 newPos = new Vector3(xPos, transform.position.y, 1); 

        transform.position = newPos;

        var graPos = transform.TransformPoint(currentGrabCollider_.offset);

        if (carriedBaby_ != null)
        {
            carriedBaby_.xPos = graPos.x;
            carriedBaby_.yPos = graPos.y;
            var babyScale = carriedBaby_.transform.localScale;
            babyScale.x = transform.localScale.x;
            carriedBaby_.transform.localScale =  babyScale;
        }

        if (carriedBaby_?.GetState() == Baby.State.PukeHold)
        {
            var pukeColor = PukeRender.color;
            pukeColor.a += Time.deltaTime * 0.3f;
            PukeRender.color = pukeColor;
            if (gotPuked_ == false)
            {
                main_.Score -= 500;
                gotPuked_ = true;
            }
        }
        else 
        {
            if (PukeRender.color.a > 0)
            {
                var pukeColor = PukeRender.color;
                pukeColor.a -= Time.deltaTime * 0.05f;
                PukeRender.color = pukeColor;
            }
        }

        if (currentAnimation_ != null && currentAnimation_.enabled == false)
        {
            currentAnimation_.enabled = true;
            if (state_ == State.Throwing)
            {
                carriedBaby_.yVel = 20 + 160 * Mathf.Min(timeSpaceDown_, 0.25f);
                timeSpaceDown_ = 0;

                carriedBaby_.xVel = xVel * 100;
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    carriedBaby_.xVel *= 3;

                carriedBaby_.groundedY = transform.position.y - 1.7f;

                carriedBaby_.SetState(Baby.State.Airborne);
                currentGrabCollider_ = GetComponent<CircleCollider2D>();
                thrownBaby_ = carriedBaby_;
                carriedBaby_ = null;
                gotPuked_ = false;
            }
            state_ = State.Idle;
        }

        if (state_ == State.Idle)
        {
            if (Math.Abs(xVel) < 0.1f)
                SetLoopingAnimation(0);
            else
                SetLoopingAnimation(2);
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
                currentGrabCollider_ = anim.GetComponent<CircleCollider2D>();

            }
        }

        foreach(var anim in LoopingAnimations)
        { 
            anim.gameObject.SetActive(false);
        }

        currentAnimation_ = animToActivate.GetComponent<Renderer>();
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

        foreach(var anim in OneTimeAnimations)
            anim.gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var baby = other.collider.GetComponent<Baby>();
        TryStartCarryBaby_(baby);
    }

    private void TryStartCarryBaby_(Baby baby)
    {
        if (baby.GetState() != Baby.State.Dead)
        {
            if (baby.GetState() == Baby.State.Sleep)
                if (!Input.GetKey(KeyCode.Space))
                    return;

            carriedBaby_ = baby;
            carriedBaby_.SetState(Baby.State.Hold);
        }
    }
}