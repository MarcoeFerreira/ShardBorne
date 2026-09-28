using UnityEngine;

public class DropOnDeathSimple : MonoBehaviour
{
    [System.Serializable]
    public class Drop
    {
        public GameObject prefab;
        [Range(0, 1)] public float chance = 1f;
        public int min = 1, max = 1;
    }

    public Drop[] drops;
    public Transform dropPoint;

    // Removemos a lógica de diálogo/cutscene daqui porque agora é o Item que trata disso!

    bool used;
    public bool alsoOnDestroy = true;
    static bool quitting;

    void OnApplicationQuit() { quitting = true; }

    public void OnDeath()
    {
        if (used) return;
        used = true;

        DoDrop();

        // Destrói o inimigo
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (!alsoOnDestroy) return;
        if (quitting) return;
        if (!gameObject.scene.isLoaded) return;

        if (!used) DoDrop();
    }

    void DoDrop()
    {
        if (drops == null) return;
        Vector3 pos = dropPoint ? dropPoint.position : transform.position;
        foreach (var d in drops)
        {
            if (!d.prefab) continue;
            if (Random.value > d.chance) continue;
            int n = Random.Range(d.min, d.max + 1);
            for (int i = 0; i < n; i++) Instantiate(d.prefab, pos, Quaternion.identity);
        }
    }
}