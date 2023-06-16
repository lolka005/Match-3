using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreWrite : MonoBehaviour
{
    public ScoreManager scoreManager;
    void Start()
    {
        Score score = new Score(Environment.UserName, GameScore.Instance.Score);
        scoreManager.AddScore(score);
        scoreManager.SaveScore();
    }
}
