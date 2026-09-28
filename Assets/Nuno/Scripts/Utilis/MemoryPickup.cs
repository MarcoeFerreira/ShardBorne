using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class MemoryPickup : MonoBehaviour
{
    [Header("Configuração da Cutscene")]
    [Tooltip("As imagens para a cutscene (Arraste as 3 imagens aqui)")]
    public Sprite[] cutsceneImages;

    [Header("Configuração do Diálogo")]
    public string dialogueNodeId = "B3_Drop_Shard";
    public DialogueVariables vars;
    public string questKey = "quest_FarmVeggies";
    public string questValue = "done";
    public string shardFlag = "hasFirstShard"; // Para evitar repetir a cutscene se já apanhaste

    [Header("Atração (Igual ao ItemPickup)")]
    public string playerTag = "Player";
    public float attractRadius = 3f;
    public float attractSpeed = 10f;
    public float pickupDelay = 0.5f;
    public float spawnKick = 2f;

    private Rigidbody2D rb;
    private float bornTime;
    private Transform targetPlayer;
    private bool isCollected = false;

    void Awake()
    {
        bornTime = Time.time;
        rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;

        // Pequeno "salto" ao nascer
        if (rb) rb.linearVelocity = new Vector2(Random.Range(-0.5f, 0.5f), 1f) * spawnKick;
    }

    void Update()
    {
        if (isCollected) return; // Se já foi apanhado, para de processar
        if (Time.time - bornTime < pickupDelay) return;

        // Lógica de atração (igual ao ItemPickup)
        if (!targetPlayer)
        {
            var player = GameObject.FindGameObjectWithTag(playerTag);
            if (player && Vector2.Distance(transform.position, player.transform.position) <= attractRadius)
                targetPlayer = player.transform;
        }

        if (targetPlayer)
        {
            if (rb) rb.linearVelocity = Vector2.zero;
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, attractSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
        if (!other.CompareTag(playerTag)) return;
        if (Time.time - bornTime < pickupDelay) return;

        CollectMemory();
    }

    void CollectMemory()
    {
        isCollected = true;

        // 1. Esconder o objeto visualmente (mas não destruir ainda)
        GetComponent<Collider2D>().enabled = false;
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr) sr.enabled = false;

        // 2. Atualizar variáveis da Quest imediatamente
        if (vars)
        {
            vars.Set(questKey, questValue, 0f, false);
            vars.Set(shardFlag, "", 0f, true);
        }

        // 3. Iniciar a sequência Cutscene -> Diálogo
        if (ImageCutsceneManager.Instance != null && cutsceneImages != null && cutsceneImages.Length > 0)
        {
            // Toca a cutscene e, no fim, chama o diálogo
            ImageCutsceneManager.Instance.PlaySequence(cutsceneImages, StartDialogue);
        }
        else
        {
            // Se não houver imagens ou manager, salta para o diálogo
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        // NOVO: Aciona o escurecimento do background assim que a cutscene termina
        if (BackgroundDimmer.Instance != null)
        {
            BackgroundDimmer.Instance.DimBackground();
        }

        // Encontra o manager e inicia a fala
        var dm = FindFirstObjectByType<DialogueManager>();
        if (dm)
        {
            dm.StartDialogue(dialogueNodeId);
        }

        // Agora sim, podemos destruir o objeto da memória
        Destroy(gameObject);
    }
}