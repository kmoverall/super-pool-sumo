using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoalSpawner : MonoBehaviour 
{
    [System.NonSerialized]
    public int[] scores = { 0, 0 };

    [SerializeField]
    Transform goalPrefab;

    List<Transform> spawnedGoals = new List<Transform>();
    
    [SerializeField]
    float minSpacing = 60;
    
    [SerializeField]
    float spawnDelay = 2;

    void Start()
    {
        Spawn(0);
        Spawn(1);
    }

    bool Spawn(int player) 
    {

        bool validSpawn = false;
        float rot = 0;
        int iter = 0;
        while (!validSpawn)
        {
            validSpawn = true;
            rot = Random.Range(0f, 359f);
            foreach (Transform g in spawnedGoals)
            {
                if (Mathf.Abs(g.transform.localRotation.eulerAngles.z - rot) < minSpacing
                    || Mathf.Abs(g.transform.localRotation.eulerAngles.z + 360 - rot) < minSpacing)
                {
                    validSpawn = false;
                    break;
                }
            }

            iter++;
            if (iter > 100)
                break;
        }

        if (!validSpawn)
            return false;

        Transform created = Instantiate(goalPrefab);
        created.GetComponentInChildren<Goal>().player = player;

        created.transform.SetParent(transform, false);
        created.transform.localPosition = Vector3.zero;
        created.transform.localRotation = Quaternion.AngleAxis(rot, new Vector3(0,0,1));

        spawnedGoals.Add(created);

        return true;
	}

    public void Score(Goal scored)
    {
        scores[scored.player] += 1;
        scored.GetComponent<Animator>().SetTrigger("Score");
    }

    IEnumerator DelayedSpawn(int player)
    {
        bool spawned = false;
        while (!spawned)
        {
            yield return new WaitForSeconds(spawnDelay);
            spawned = Spawn(player);
        }
    }
}
