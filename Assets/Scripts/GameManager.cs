using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [System.NonSerialized]
    public int[] scores = { 0, 0 };

    [SerializeField]
    Transform goalPrefab;

    [SerializeField]
    Beachball beachballPrefab;

    List<Transform> spawnedGoals = new List<Transform>();
    List<Beachball> balls = new List<Beachball>();

    [SerializeField]
    float minSpacing = 60;

    [SerializeField]
    float goalSpawnDelay = 2;

    [SerializeField]
    float ballSpawnDelay = 1;
    
    [SerializeField]
    int maxBalls = 2;


    void Start()
    {
        SpawnGoal(0);
        SpawnGoal(1);
        for (int i = 0; i < maxBalls; i++)
        {
            SpawnBall();
        }
    }

    public void IncreaseBallCount()
    {
        maxBalls++;
        StartCoroutine(DelayedBallSpawn());
    }

    bool SpawnGoal(int player)
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
        created.transform.localRotation = Quaternion.AngleAxis(rot, new Vector3(0, 0, 1));

        spawnedGoals.Add(created);

        return true;
    }

    public bool SpawnBall()
    {
        Beachball ball = Instantiate(beachballPrefab);

        bool validSpawn = false;
        Vector2 pos = Vector2.zero;
        int iter = 0;
        while (!validSpawn)
        {
            validSpawn = true;
            pos = Random.insideUnitCircle * 3;
            foreach (Beachball b in balls)
            {
                Vector2 oPos = new Vector2(b.transform.position.x, b.transform.position.y);
                if (Vector2.Distance(pos, oPos) < b.GetComponent<CircleCollider2D>().radius + ball.GetComponent<CircleCollider2D>().radius)
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

        ball.transform.position = Random.insideUnitCircle * 3;
        balls.Add(ball);
        return true;
    }

    public void Score(Goal scored, Beachball ball)
    {
        scores[scored.player] += 1;

        spawnedGoals.Remove(scored.transform.parent);
        balls.Remove(ball);

        StartCoroutine(DelayedGoalSpawn(scored.player));
        StartCoroutine(DelayedBallSpawn());

        StartCoroutine(DelayedDestroy(scored.transform.parent.gameObject));
        StartCoroutine(DelayedDestroy(ball.gameObject));
    }

    IEnumerator DelayedBallSpawn()
    {
        bool spawned = false;
        while (!spawned)
        {
            yield return new WaitForSeconds(ballSpawnDelay);
            spawned = SpawnBall();
        }
    }

    IEnumerator DelayedGoalSpawn(int player)
    {
        bool spawned = false;
        while (!spawned)
        {
            yield return new WaitForSeconds(goalSpawnDelay);
            spawned = SpawnGoal(player);
        }
    }

    IEnumerator DelayedDestroy(GameObject toDestroy)
    {
        yield return new WaitForSeconds(1);
        Destroy(toDestroy.gameObject);
    }
}
