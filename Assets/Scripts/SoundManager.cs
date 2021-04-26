using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource bgm;
    [SerializeField] float bpm = 0.0f;
    [SerializeField] float beatOffsetMod = 0.0f;
    [SerializeField] LevelGenerator level;
    [SerializeField] float colourStep = 0.0f;
    [SerializeField] float maxFlash = 0.05f;

    float secondsPerBeat = 0.0f;
    bool playing = false;

    float currentColour = 0.0f;
    float lastBeatPercentage = 0.0f;
    Coroutine flashRoutine = null;

    private void Awake()
    {
        secondsPerBeat = 60.0f / bpm;
        level.OnReady.AddListener(StartAudio);
    }

    public void StartAudio()
    {
        playing = true;
        bgm.Play();
    }

    IEnumerator Beat()
    {
        Color color = Color.HSVToRGB(currentColour / 360.0f, 1.0f, 1.0f);
        level.UpdateColour(color);
        float t = 0.0f;
        while(t <= secondsPerBeat)
        {
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
            float flash = (1.0f - (t / secondsPerBeat)) * maxFlash;
            level.UpdateColour(color + (Color.white * flash));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!playing) return;

        float beatPercentage = (bgm.time - secondsPerBeat * beatOffsetMod) % secondsPerBeat;
        if(beatPercentage < lastBeatPercentage)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);

            currentColour += colourStep;
            currentColour %= 360.0f;
            flashRoutine = StartCoroutine(Beat());
        }
        lastBeatPercentage = beatPercentage;
    }
}
