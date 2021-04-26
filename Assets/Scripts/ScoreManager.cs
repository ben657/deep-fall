using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] TMPro.TextMeshProUGUI ui;
    [SerializeField] Animator uiAnimator;
    [SerializeField] float scorePerSecond = 1.0f;
    [SerializeField] float uiUpdateThreshold = 10.0f;
    [SerializeField] TMPro.TextMeshProUGUI multiplierUI;
    [SerializeField] Image multiplierImage;
    [SerializeField] Color[] multiplierColors;

    public int Score => uiScore;
    float score = 0;
    int uiScore = 0;

    float lastPlayerY = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreUI(false);
    }

    void UpdateScoreUI(bool animate)
    {
        uiScore = Mathf.FloorToInt(score);
        ui.text = uiScore.ToString();
        uiAnimator.SetTrigger("Update");
    }

    Color multFromCol;
    Color multToCol;
    float multColS = 0.0f;

    void UpdateMultiplierUI(float multiplier)
    {
        multiplierUI.text = "x" + Mathf.FloorToInt(multiplier);
        int colorIndex = Mathf.Clamp(Mathf.FloorToInt(multiplier) - 1, 0, multiplierColors.Length - 1);

        multFromCol = multiplierImage.color;
        multToCol = multiplierColors[colorIndex];
        multColS = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float multiplier = player.Multiplier;
        float dist = Mathf.Abs(player.transform.position.y - lastPlayerY);
        score += multiplier * dist;
        if (score - uiScore >= uiUpdateThreshold)
            UpdateScoreUI(true);

        UpdateMultiplierUI(multiplier);

        lastPlayerY = player.transform.position.y;

        if (multColS < 1.0f)
            multColS += Time.deltaTime * 5.0f;
        multiplierImage.color = Color.Lerp(multFromCol, multToCol, multColS);
    }

    [System.Serializable]
    struct ScorePostData
    {
        public string guid;
        public string name;
        public int score;
    }

    [System.Serializable]
    struct ScorePostResponse
    {
        public int position;
    }

    IEnumerator DoPostScore(string name, Action<int> callback)
    {
        ScorePostData data = new ScorePostData()
        {
            guid = SystemInfo.deviceUniqueIdentifier,
            name = name,
            score = Score
        };

        string body = JsonUtility.ToJson(data);
        byte[] rawBody = Encoding.UTF8.GetBytes(body);

        UnityWebRequest request = new UnityWebRequest("http://213.171.210.192:8005/scores", "POST", new DownloadHandlerBuffer(), new UploadHandlerRaw(rawBody));
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Post Score Request error");
            Debug.LogError(request.error);
        }
        else
        {
            var response = JsonUtility.FromJson<ScorePostResponse>(request.downloadHandler.text);
            callback(response.position);
        }
    }

    public void PostScore(string name, Action<int> callback)
    {
        StartCoroutine(DoPostScore(name, callback));
    }

    [System.Serializable]
    public struct ScoresData
    {
        public ScoreData[] scores;
        public int position;
    }

    [System.Serializable]
    public struct ScoreData
    {
        public string guid;
        public string name;
        public int score;
        public string position;
    }

    IEnumerator DoGetScores(Action<ScoresData> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get("http://213.171.210.192:8005/scores?guid=" + SystemInfo.deviceUniqueIdentifier);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Get Scores Request error");
            Debug.LogError(request.error);
        }
        else
        {
            var response = JsonUtility.FromJson<ScoresData>(request.downloadHandler.text);
            callback(response);
        }
    }

    public void GetScores(Action<ScoresData> callback)
    {
        StartCoroutine(DoGetScores(callback));
    }

    struct HighScoreResponse
    {
        public int score;
    }

    IEnumerator DoGetHighScore(Action<int> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get("http://213.171.210.192:8005/scores/high?guid=" + SystemInfo.deviceUniqueIdentifier);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Get HighScore Request error");
            Debug.LogError(request.error);
        }
        else
        {
            var response = JsonUtility.FromJson<HighScoreResponse>(request.downloadHandler.text);
            callback(response.score);
        }
    }

    public void GetHighScore(Action<int> callback)
    {
        StartCoroutine(DoGetHighScore(callback));
    }
}
