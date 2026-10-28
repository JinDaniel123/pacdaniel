using System.Collections;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;
    public float spawnDelay = 5f;
    public float moveSpeed = 3f;

    public float minX = -9f;
    public float maxX = 9f;
    public float minY = -7f;
    public float maxY = 7f;

    private GameObject currentCherry;
    private Vector2 targetPos;
    private bool isCherryActive = false;

    void Start()
    {
        StartCoroutine(SpawnCherryRoutine());
    }

    IEnumerator SpawnCherryRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            float randomY = Random.Range(minY, maxY);
            bool spawnLeft = Random.value > 0.5f;

            Vector2 spawnPos;
            if (spawnLeft)
            {
                spawnPos = new Vector2(minX - 2f, randomY);
                targetPos = new Vector2(maxX + 2f, randomY);
            }
            else
            {
                spawnPos = new Vector2(maxX + 2f, randomY);
                targetPos = new Vector2(minX - 2f, randomY);
            }

            currentCherry = Instantiate(cherryPrefab, spawnPos, Quaternion.identity);
            isCherryActive = true;

            while (isCherryActive && currentCherry != null)
            {
                currentCherry.transform.position = Vector2.MoveTowards(currentCherry.transform.position, targetPos, moveSpeed * Time.deltaTime);
                if (Vector2.Distance(currentCherry.transform.position, targetPos) < 0.1f)
                {
                    Destroy(currentCherry);
                    isCherryActive = false;
                }
                yield return null;
            }
        }
    }

    public void CherryEaten()
    {
        if (currentCherry != null) Destroy(currentCherry);
        isCherryActive = false;
    }
}