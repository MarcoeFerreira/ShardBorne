using UnityEngine;
using System.Collections;

public class EnemySpawnerOnQuest : MonoBehaviour
{
    public DialogueVariables vars;
    public string questKey = "quest_FarmVeggies";
    public string requiredValue = "return";
    public string spawnedFlag = "enemy.farm.spawned";
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    void Start() => StartCoroutine(Watch());

    IEnumerator Watch()
    {
        while (true)
        {
            if (vars.texts.TryGetValue(questKey, out var s) && s == requiredValue)
            {
                bool spawned = vars.flags.TryGetValue(spawnedFlag, out var f) && f;
                if (!spawned && enemyPrefab)
                {
                    Instantiate(enemyPrefab, spawnPoint ? spawnPoint.position : transform.position, Quaternion.identity);
                    vars.Set(spawnedFlag, "", 0f, true);
                    yield break;
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}
