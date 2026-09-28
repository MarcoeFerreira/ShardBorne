using UnityEngine;
using UnityEngine.InputSystem; // Novo Input
// Remove qualquer #if ENABLE_INPUT_SYSTEM que tivesses aqui

[RequireComponent(typeof(Collider2D))]
public class FarmPlot : MonoBehaviour
{
    [Header("UI/Refs")]
    public GameObject promptE;
    public HarvestMinigameBar minigame;
    public DialogueVariables vars;
    public string countKey = "count.potato";

    [Header("Input (Novo Input System)")]
    public InputActionReference interactAction; // arrasta a action "Player/Interact" aqui

    [Header("Drop opcional")]
    public GameObject pickupPrefab;
    public Transform dropPoint;

    bool inside;

    void Reset() { GetComponent<Collider2D>().isTrigger = true; }

    void OnEnable()
    {
        if (interactAction && interactAction.action != null)
        {
            interactAction.action.performed += OnInteract;
            interactAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (interactAction && interactAction.action != null)
        {
            interactAction.action.performed -= OnInteract;
            interactAction.action.Disable();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        inside = true;
        if (promptE) promptE.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        inside = false;
        if (promptE) promptE.SetActive(false);
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!inside || ctx.control == null) return; // só dentro do trigger
        if (promptE) promptE.SetActive(false);
        if (minigame) minigame.Open(OnResult);
    }

    void OnResult(bool success)
    {
        if (!success) return;

        if (pickupPrefab)
        {
            Instantiate(pickupPrefab, dropPoint ? dropPoint.position : transform.position, Quaternion.identity);
        }
        else
        {
            vars.Add(countKey, 1f); // incrementa diretamente
        }
    }
}
