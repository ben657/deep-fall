using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRow : MonoBehaviour
{
    public TMPro.TextMeshProUGUI positionText;
    public TMPro.TextMeshProUGUI nameText;
    public TMPro.TextMeshProUGUI scoreText;

    public void Set(ScoreManager.ScoreData data)
    {
        positionText.text = data.position.ToString();
        nameText.text = data.name;
        scoreText.text = data.score.ToString();
    }
}
