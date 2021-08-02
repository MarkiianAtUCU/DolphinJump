using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class LevelPart
{
    public GameObject prefab;
    public int probability;
}

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<LevelPart> levelPrefabs;
    [SerializeField] private float startOffset;
    [SerializeField] private float generationStep;
    [SerializeField] private int numbersOfPregenerated;
    [SerializeField] private int destroyDistance;

    private List<int> _roundRobin = new List<int>();
    private List<Transform> _levelPartInstances = new List<Transform>();
    private int _stepsPassed;

    void Start()
    {
        for (int i = 0; i < levelPrefabs.Count; i++)
        {
            for (int j = 0; j < levelPrefabs[i].probability; j++)
            {
                _roundRobin.Add(i);
            }
        }

        for (int i = 0; i < numbersOfPregenerated; i++)
        {
            _levelPartInstances.Add(
                SpawnLevelPart(new Vector3(startOffset + i * generationStep, 0, 0)).transform
            );
        }

        _stepsPassed = numbersOfPregenerated;
    }

    private void Update()
    {
        GenerateLevelWhilePlayerMoves();
    }

    private void GenerateLevelWhilePlayerMoves()
    {
        int requiredStepCount = (int) Mathf.Floor(GameManager.Instance.player.transform.position.x / generationStep) +
                                numbersOfPregenerated;
        if (requiredStepCount > _stepsPassed)
        {
            // Spawn level parts, according to steps passed
            for (int i = 0; i < requiredStepCount - _stepsPassed; i++)
            {
                _levelPartInstances.Add(
                    SpawnLevelPart(new Vector3(startOffset + (_stepsPassed + i) * generationStep, 0, 0)).transform
                );
            }

            // Destroy level parts which are too far from player
            for (int i = 0; i < _levelPartInstances.Count; i++)
            {
                if (GameManager.Instance.player.transform.position.x - _levelPartInstances[i].transform.position.x >
                    destroyDistance)
                {
                    Destroy(_levelPartInstances[i].gameObject);
                    _levelPartInstances.RemoveAt(i);
                }
            }

            _stepsPassed = requiredStepCount;
        }
    }

    private GameObject SpawnLevelPart(Vector3 pose)
    {
        return Instantiate(levelPrefabs[
            _roundRobin[
                Random.Range(0, _roundRobin.Count)
            ]
        ].prefab, pose, Quaternion.identity);
    }
}