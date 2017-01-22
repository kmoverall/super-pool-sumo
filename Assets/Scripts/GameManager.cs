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

    [SerializeField]
    float gameLength = 180;
    [System.NonSerialized]
    public float timeRemaining;

    public RectTransform HUD;
    public RectTransform Results;

    public void Awake()
    {
        timeRemaining = gameLength;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void StartGame()
    {
        StopAllCoroutines();

        SpawnGoal(0);
        SpawnGoal(1);
        for (int i = 0; i < maxBalls; i++)
        {
            SpawnBall();
        }

        PlayerInput[] inputs = GetComponents<PlayerInput>();
        foreach (PlayerInput i in inputs)
        {
            i.enabled = true;
        }

        StartCoroutine(Timer());
    }

    public void IncreaseBallCount()
    {
        maxBalls++;
        StartCoroutine(DelayedBallSpawn());
    }

    bool SpawnGoal(int player)
    {
        if (timeRemaining <= 0)
            return false;

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
        if (timeRemaining <= 0)
            return false;

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

    public void StopPlay()
    {
        for (int i = 0; i < spawnedGoals.Count; i++)
        {
            spawnedGoals[i].GetComponentInChildren<Goal>().enabled = false;
        }

        PlayerInput[] inputs = GetComponents<PlayerInput>();
        foreach (PlayerInput i in inputs)
        {
            i.enabled = false;
        }

        StartCoroutine(ShowResults(3));
    }

    IEnumerator ShowResults(float delay)
    {
        yield return new WaitForSeconds(delay);
        Results.GetComponent<Animator>().SetTrigger("Reset");
        HUD.GetComponent<Animator>().SetTrigger("Close");
    }

    public void RestartGame()
    {
        for (int i = 0; i < spawnedGoals.Count; i++)
        {
            Destroy(spawnedGoals[i].gameObject);
        }
        spawnedGoals.Clear();

        for (int i = 0; i < balls.Count; i++)
        {
            Destroy(balls[i].gameObject);
        }
        balls.Clear();

        scores = new int[] { 0, 0 };
        timeRemaining = gameLength;

        PlayerInput[] players = GetComponents<PlayerInput>();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].Reset();
        }

        FindObjectOfType<Water>().ResetTextures();

        HUD.GetComponent<Animator>().SetTrigger("Reset");
    }

    IEnumerator DelayedBallSpawn()
    {
        bool spawned = false;
        while (!spawned)
        {
            yield return new WaitForSeconds(ballSpawnDelay);
            if (timeRemaining <= 0)
                yield break;
            spawned = SpawnBall();
        }
    }

    IEnumerator DelayedGoalSpawn(int player)
    {
        bool spawned = false;
        while (!spawned)
        {
            yield return new WaitForSeconds(goalSpawnDelay);
            if (timeRemaining <= 0)
                yield break;
            spawned = SpawnGoal(player);
        }
    }

    IEnumerator DelayedDestroy(GameObject toDestroy)
    {
        yield return new WaitForSeconds(1);
        Destroy(toDestroy.gameObject);
    }

    IEnumerator Timer()
    {
        timeRemaining = gameLength;
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        StopPlay();
    }
}
