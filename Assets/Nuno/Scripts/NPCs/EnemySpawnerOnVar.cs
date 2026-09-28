using System.Collections;
using UnityEngine;

public class EnemySpawnerOnVar : MonoBehaviour
{
    public DialogueVariables vars;
    public string stageKey = "quest.farm.stage";
    public int requiredStage = 2;

    public string spawnedFlag = "enemy.farm.spawned";
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    void Start() => StartCoroutine(Watch());

    IEnumerator Watch()
    {
        while (true)
        {
            int stage = Mathf.RoundToInt(vars.numbers.TryGetValue(stageKey, out var st) ? st : 0);
            bool spawned = vars.flags.TryGetValue(spawnedFlag, out var f) && f;

            if (stage >= requiredStage && !spawned && enemyPrefab)
            {
                Instantiate(enemyPrefab, spawnPoint ? spawnPoint.position : transform.position, Quaternion.identity);
                vars.Set(spawnedFlag, "", 0f, true);
                yield break;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}
