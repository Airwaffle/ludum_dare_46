using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughBubble : MonoBehaviour
{
    public Animator Start; 
    public Animator Idle;

    public Animator[] Wishes;
    public Animator SpaceWish;

    private float startTime_;
    private int randomWish_;

    private float carriedTime_;
    private bool started_ = false;

    public int RandomWish{get{return randomWish_;}}

    private float timeTilShow = 1.6f;

    void Awake()
    {
        randomWish_ = (int)UnityEngine.Random.Range(0, Wishes.Length);
    }

    internal void UpdateSleep()
    {
        startTime_ += Time.deltaTime;

        if (startTime_ > 1.6 && !started_) 
        {
            Start.gameObject.SetActive(true);
            timeTilShow += 0.4f;
            started_ = true;
        }

        if (Start.GetComponent<SpriteRenderer>().enabled == false)
        {
            Start.gameObject.SetActive(false);
            Idle.gameObject.SetActive(true);
        }

        if (startTime_ > 2f) 
        {
            Wishes[randomWish_].gameObject.SetActive(true);
        }

        if (startTime_ > 5.0f) 
        {
            Wishes[randomWish_].gameObject.SetActive(false);
        }
        
        if (startTime_ > 5.3f) 
        {
            SpaceWish.gameObject.SetActive(true);
        }
    }

    internal void UpdateCarried()
    {
        carriedTime_ += Time.deltaTime;

        if (carriedTime_ < timeTilShow)
        {
            Wishes[randomWish_].gameObject.SetActive(false);
            SpaceWish.gameObject.SetActive(false);
            Idle.gameObject.SetActive(false);
        } 
        else 
        {
            if (carriedTime_ > timeTilShow && !started_ ) 
            {
                timeTilShow += 0.4f;
                started_ = true;
                Start.gameObject.SetActive(true);
                Start.GetComponent<SpriteRenderer>().enabled = true;
                Start.Play("animation", -1, 0f);
            }

            if (Start.GetComponent<SpriteRenderer>().enabled == false)
            {
                Start.gameObject.SetActive(false);
                Idle.gameObject.SetActive(true);
            }

            if (carriedTime_ > 2f) 
            {
                Wishes[randomWish_].gameObject.SetActive(true);
            }
        }
    }

    internal void Hide()
    {
        started_ = false;
        Wishes[randomWish_].gameObject.SetActive(false);
        SpaceWish.gameObject.SetActive(false);
        Idle.gameObject.SetActive(false);
        carriedTime_ = 0;
    }
}
