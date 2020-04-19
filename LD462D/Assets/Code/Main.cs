using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public Baby baby;
    public SpriteRenderer fadeToBlack;
    public SpriteRenderer[] accusations;
    public AudioSource normalMusic;

    public GameObject HighScoreTextPrefab;
    public GameObject YourScoreTextPrefab;
    public GameObject ScorePrefab;
    public GameObject GoalPrefab;
    public GameObject BirdPrefab;
    private float babyDeadTime_ = 0;
    private float fadeToBlackTime_ = 2;
    private float fadeToBlackSpeed_ = 1;
    private int randomAccusation_ = -1;
    private float nextBirdCounter_ = 0;

    private List<GameObject> goals_ = new List<GameObject>();
    private List<GameObject> birds_ = new List<GameObject>();

    private int goalsTaken_ = 0;

    private ScoreDisplay scoreInstance_ = null;
    private ScoreDisplay highscoreInstance_ = null;

    public int Score = 0;
    public int HighScore = 0;

    void Awake()
    {
        SpawnGoals_();

        if (!File.Exists("save.dat"))
            randomAccusation_ = 0;
        else 
            randomAccusation_= int.Parse(File.ReadAllText("save.dat"));

        randomAccusation_ = Mathf.Min(randomAccusation_, accusations.Length - 1);

        File.WriteAllText("save.dat", (randomAccusation_ + 1).ToString());


        if (!File.Exists("high.dat"))
            HighScore = 0;
        else 
            HighScore = int.Parse(File.ReadAllText("high.dat"));
    }

    public int GoalsTaken 
    {
        get
        {
            return goalsTaken_;
        }
    }

    int CurrentGoalCount_
    {
        get
        {
            if (goalsTaken_ < 5)
                return 3;
            else if (goalsTaken_ < 10)
                return 4;
            else if (goalsTaken_ < 15)
                return 5;
            else 
                return 6;
        }
    }

     int CurrentBirdCount_
    {
        get
        {
            if (goalsTaken_ < 2)
                return 1;
            else if (goalsTaken_ < 8)
                return 2;
            else if (goalsTaken_ < 16)
                return 3;
            else
                return 4;
        }
    }

    private void SpawnGoals_()
    {
        while(goals_.Count < CurrentGoalCount_)
        {
            float xPos = UnityEngine.Random.Range(-15.0f, 15.0f);
            float yPos = UnityEngine.Random.Range(15, 70.0f);

            float lowestExisting = int.MaxValue;
            float secondLowestExisting = int.MaxValue;

            foreach(var existingGoal in goals_)
            {
                if (existingGoal.transform.position.y < lowestExisting)
                    secondLowestExisting = lowestExisting;
                    lowestExisting = existingGoal.transform.position.y;
            }

            // Ensures there is always at least one goal that is reachable.
            if (yPos > 30 && (lowestExisting > 30 || secondLowestExisting > 30))
                yPos = UnityEngine.Random.Range(20, 30);

            goals_.Add(Instantiate(GoalPrefab, new Vector3(xPos, yPos, 0), transform.rotation));
        }
    }

    private void SpawBird_()
    {
        float xPos = UnityEngine.Random.Range(-15.0f, 15.0f);
        if (xPos > 0)
            xPos = 15;
        else    
            xPos = -15;

        float yPos = UnityEngine.Random.Range(5, 35.0f);

        birds_.Add(Instantiate(BirdPrefab, new Vector3(xPos, yPos, 0), transform.rotation));
    }

    void Update()
    {
        if (birds_.Count < CurrentBirdCount_)
        {
            nextBirdCounter_ -= Time.deltaTime;
            if(nextBirdCounter_ <= 0)
            {
                SpawBird_();
                nextBirdCounter_ = 20;
            }
        }

        foreach(var bird in birds_)
        {
            if (bird.activeSelf == false)
            {
                birds_.Remove(bird);
                nextBirdCounter_ = 10;
                break;
            }
        }

        foreach(var goal in goals_)
        {
            if (goal.activeSelf == false)
            {
                Score += goal.GetComponent<Goal>().Score;
                goalsTaken_++;
                goals_.Remove(goal);
                SpawnGoals_();
                break;
            }
        }

        if (baby.GetState() == Baby.State.Dead)
        {
            babyDeadTime_ += Time.deltaTime;

            if (normalMusic.volume > 0)
                normalMusic.volume -= Time.deltaTime * 0.5f;

            if (babyDeadTime_ > fadeToBlackTime_)
            {
                var color = fadeToBlack.color;
                color.a += Time.deltaTime * fadeToBlackSpeed_;
                fadeToBlack.color = color;
            }

            if (babyDeadTime_ > fadeToBlackTime_ + (1 / fadeToBlackSpeed_))
            {
                var color = accusations[randomAccusation_].color;
                color.a += Time.deltaTime;

                if (scoreInstance_ == null)
                {
                    if (Score < 0)
                        Score = 0;
                        
                    if (Score > HighScore)
                    {
                        HighScore = Score;
                        File.WriteAllText("high.dat", HighScore.ToString());
                    }
                    Instantiate(HighScoreTextPrefab, new Vector3(-3.8f, 12, 0), Quaternion.identity).GetComponent<ScoreDisplay>();
                    Instantiate(YourScoreTextPrefab, new Vector3(-3.4f, 9.5f, 0), Quaternion.identity).GetComponent<ScoreDisplay>();

                    scoreInstance_ = Instantiate(ScorePrefab, new Vector3(0, 9.5f, 0), Quaternion.identity).GetComponent<ScoreDisplay>();
                    highscoreInstance_ = Instantiate(ScorePrefab, new Vector3(0, 12, 0), Quaternion.identity).GetComponent<ScoreDisplay>();
                    scoreInstance_.CreateNumber(Score, HighScore.ToString().Length);
                    highscoreInstance_.CreateNumber(HighScore, HighScore.ToString().Length);
                }

                accusations[randomAccusation_].color = color;

                
                if (Input.GetKeyDown(KeyCode.Space))
                    Reset();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                Reset();
        }
    }

    private void Reset()
    {
        fadeToBlackTime_ = 0.5f; 
        fadeToBlackSpeed_ = 3;
        SceneManager.LoadScene("SampleScene");
    }
}
