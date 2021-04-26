using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] Segment[] segmentPrefabs;
    [SerializeField] float segmentHeight = 5.0f;
    [SerializeField] float minDistAhead = 0.0f;
    [SerializeField] float maxDistBehind = 0.0f;
    [SerializeField] float dynamicWeightReduction = 5.0f;
    [SerializeField] float dynamicWeightRecharge = 2.0f;

    [SerializeField] Transform player;

    public UnityEvent OnReady = new UnityEvent();

    Dictionary<Segment, float> dynamicWeights = new Dictionary<Segment, float>();
    Dictionary<Segment, Coroutine> dynamicWeightChangers = new Dictionary<Segment, Coroutine>();
    List<Segment> segments = new List<Segment>();

    float tunnelHeight = 0.0f;

    Color currentColour = Color.red;

    private void Awake()
    {
        foreach(var segment in segmentPrefabs)
        {
            dynamicWeights[segment] = segment.spawnWeight;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 20; i++)
        {
            SpawnSegment();
        }

        OnReady.Invoke();
    }

    Vector3 GetEnd()
    {
        if (segments.Count <= 0) return Vector3.zero;
        return segments[segments.Count - 1].GetWorldEnd();
    }

    Segment GetPrefabWeighted()
    {
        float totalWeight = 0.0f;
        foreach (float weight in dynamicWeights.Values)
            totalWeight += weight;

        float target = Random.value * totalWeight;
        float current = 0.0f;
        foreach(var segment in segmentPrefabs)
        {
            current += segment.spawnWeight;
            if (current >= target)
                return segment;
        }

        return segmentPrefabs[segmentPrefabs.Length - 1];
    }

    void UpdateMaterials()
    {
        foreach(var segment in segments)
        {
            segment.UpdateTunnelData(player.position.y, 100.0f);
        }
    }

    public void UpdateColour(Color colour)
    {
        currentColour = colour;
        foreach (var segment in segments)
        {
            segment.UpdateColourData(colour);
        }
    }

    IEnumerator DoDyanamicWeightStuff(Segment segmentPrefab)
    {
        dynamicWeights[segmentPrefab] -= dynamicWeightReduction;
        while (dynamicWeights[segmentPrefab] < segmentPrefab.spawnWeight)
        {
            yield return new WaitForEndOfFrame();
            dynamicWeights[segmentPrefab] += Time.deltaTime * dynamicWeightRecharge;
        }

        dynamicWeights[segmentPrefab] = segmentPrefab.spawnWeight;
    }

    Segment SpawnSegment()
    {
        Segment prefab = GetPrefabWeighted();
        Segment segment = Instantiate(prefab, transform, true);

        segment.transform.position = GetEnd();

        segments.Add(segment);

        segment.UpdateColourData(currentColour);
        segment.UpdateTunnelData(player.position.y, 100.0f);

        if(prefab.dynamicWeight)
        {
            if (dynamicWeightChangers.ContainsKey(prefab)) StopCoroutine(dynamicWeightChangers[prefab]);
            dynamicWeightChangers[prefab] = StartCoroutine(DoDyanamicWeightStuff(prefab));
        }

        tunnelHeight += segment.length;

        return segment;
    }

    // Update is called once per frame
    void Update()
    {
        float distAhead = player.position.y - GetEnd().y;
        if (distAhead < minDistAhead)
        {
            SpawnSegment();

            Vector3 firstSegPos = segments[0].transform.position;
            float distBehind = firstSegPos.y - player.position.y;
            if(distBehind > maxDistBehind)
            {
                tunnelHeight -= segments[0].length;
                Destroy(segments[0].gameObject);
                segments.RemoveAt(0);
            }
        }

        UpdateMaterials();
    }
}
