using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public ScoreRow scoreRowPrefab;
    public Player player;
    public ScoreManager scoreManager;
    public GameObject gameUI;
    public GameObject gameOverUI;
    public GameObject nameUI;
    public GameObject scoresUI;
    public TMPro.TextMeshProUGUI positionText;
    public TMPro.TextMeshProUGUI scoreText;
    public Transform scoresList;
    public float fadeTime = 3.0f;
    public TMPro.TMP_InputField nameInput;

    private void Awake()
    {
        player.OnDeath.AddListener(EnableGameOverUI);
        gameUI.SetActive(true);
        gameOverUI.SetActive(false);
        nameUI.SetActive(true);
        scoresUI.SetActive(false);
    }

    string ordinal(int i)
    {
        int ones = i % 10;
        int tens = Mathf.FloorToInt(i / 10) % 10;
        string suff = "";
        if (tens == 1) {
        suff = "th";
        } else {
            switch (ones) {
            case 1 : suff = "st"; break;
            case 2 : suff = "nd"; break;
            case 3 : suff = "rd"; break;
            default : suff = "th"; break;
            }
        }
        return i.ToString() + suff;
    }

    void UpdatePosition(int position)
    {
        positionText.text = ordinal(position);
    }

    void SetScores(ScoreManager.ScoresData data)
    {
        List<GameObject> toDestroy = new List<GameObject>();
        for (int i = 0; i < scoresList.childCount; i++)
            toDestroy.Add(scoresList.GetChild(i).gameObject);
        foreach (var go in toDestroy) Destroy(go);

        foreach(var score in data.scores)
        {
            var row = Instantiate(scoreRowPrefab, scoresList);
            row.Set(score);
        }

        UpdatePosition(data.position);
    }

    IEnumerator FadeGameOver(bool submit)
    {
        scoreManager.GetScores(SetScores);
        scoreText.text = "Score: " + scoreManager.Score;
        gameUI.SetActive(false);
        gameOverUI.SetActive(true);
        nameUI.SetActive(submit);
        scoresUI.SetActive(!submit);
        positionText.gameObject.SetActive(!submit);
        if(submit)
            nameInput.Select();
        CanvasGroup bg = gameOverUI.GetComponent<CanvasGroup>();
        bg.alpha = 0.0f;
        while (bg.alpha < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            bg.alpha += Time.deltaTime * (1.0f / fadeTime);
        }
    }

    public void EnableGameOverUI()
    {
        scoreManager.GetHighScore(score =>
        {
            StartCoroutine(FadeGameOver(scoreManager.Score > score));
        });
    }

    public void LoadMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void Replay()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void ValidateNameInput()
    {
        nameInput.text = nameInput.text.ToUpper();
        if (nameInput.text.Length > 3)
            nameInput.text = nameInput.text.Substring(0, 3);
    }

    public void SubmitScore()
    {
        scoreManager.PostScore(nameInput.text, i => 
        {
            nameUI.SetActive(false);
            scoresUI.SetActive(true);
            positionText.gameObject.SetActive(true);
            scoreManager.GetScores(SetScores);
        });
    }
}
