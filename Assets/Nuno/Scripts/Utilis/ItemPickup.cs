using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Header("Contagem")]
    public DialogueVariables vars;
    public string countKey = "count.potato";
    public int amount = 1;

    [Header("Atra��o")]
    public string playerTag = "Player";
    public float attractRadius = 3f;
    public float attractSpeed = 10f;
    public float pickupDelay = 0.25f; // espera antes de poder ser apanhado

    [Header("Efeito Spawn")]
    public float spawnKick = 2f; // pequeno impulso visual

    Rigidbody2D rb;
    float born;
    Transform target;

    void Awake()
    {
        born = Time.time;
        rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;

        if (rb) rb.linearVelocity = new Vector2(Random.Range(-0.5f, 0.5f), 1f) * spawnKick;
    }

    void Update()
    {
        if (Time.time - born < pickupDelay) return;

        if (!target)
        {
            var player = GameObject.FindGameObjectWithTag(playerTag);
            if (player && Vector2.Distance(transform.position, player.transform.position) <= attractRadius)
                target = player.transform;
        }

        if (target)
        {
            // atrai e desacelera a velocidade inicial
            if (rb) rb.linearVelocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, target.position, attractSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!target || !other.CompareTag(playerTag)) return;
        if (Time.time - born < pickupDelay) return;

        vars.Add(countKey, amount);
        Destroy(gameObject);
    }
}
