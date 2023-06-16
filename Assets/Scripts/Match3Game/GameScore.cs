using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class GameScore : MonoBehaviour
{
    public static GameScore Instance { get; private set; }

    private int _score;

    public int Score
    {
        get { return _score; }
        set
        {
            if (_score == value) return;
            _score = value;
            scoreText.SetText($"Ñ÷¸ò: {_score}");
        }
    }

    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        Instance = this;
    }
}
