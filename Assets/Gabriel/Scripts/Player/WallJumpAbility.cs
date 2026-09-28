using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Habilidade de pulo na parede do jogador.
/// Permite saltar a partir de uma parede.
/// </summary>
public class WallJumpAbility : BaseAbility
{
    public InputActionReference wallJumpActionRef; // Referência à ação de pulo na parede (Input System)
    [SerializeField] private Vector2 wallJumpForce; // Força aplicada ao pular da parede
    [SerializeField] private float wallJumpMaxTime; // Tempo máximo do estado de wall jump
    private float wallJumpMinimumTime;              // Tempo mínimo antes de poder sair do wall jump
    private float wallJumpTimer;                    // Timer do estado de wall jump

    /// <summary>
    /// Ativa o evento de input ao habilitar o componente.
    /// </summary>
    private void OnEnable()
    {
        wallJumpActionRef.action.performed += TryToWallJump;
    }

    /// <summary>
    /// Remove o evento de input ao desabilitar o componente.
    /// </summary>
    private void OnDisable()
    {
        wallJumpActionRef.action.performed -= TryToWallJump;
    }

    /// <summary>
    /// Inicializa variáveis da habilidade.
    /// </summary>
    protected override void Inicialization()
    {
        base.Inicialization();
        wallJumpTimer = wallJumpMaxTime;
    }

    /// <summary>
    /// Tenta executar o pulo na parede quando o input é acionado.
    /// </summary>
    private void TryToWallJump(InputAction.CallbackContext value)
    {
        if (!isPermitted || linkedStateMachine.currentState == PlayerStates.State.KnockBack)
        {
            return;
        }

        if (EvaluateWallJumpConditions())
        {
            linkedStateMachine.ChangeState(PlayerStates.State.WallJump);
            wallJumpTimer = wallJumpMaxTime;
            wallJumpMinimumTime = 0.15f;
            // Inverte o lado do jogador e aplica a força do pulo
            player.ForceFlip();
            if (player.facingRight)
            {
                linkedPhysics.rb.linearVelocity = new Vector2(wallJumpForce.x, wallJumpForce.y);
            }
            else
            {
                linkedPhysics.rb.linearVelocity = new Vector2(-wallJumpForce.x, wallJumpForce.y);
            }
        }
    }

    /// <summary>
    /// Processa a lógica do wall jump a cada frame.
    /// </summary>
    public override void ProcessAbility()
    {
        wallJumpTimer -= Time.deltaTime;
        wallJumpMinimumTime -= Time.deltaTime;

        if(wallJumpMinimumTime < 0 && linkedPhysics.grounded)
        {
            if(linkedInput.horizontalInput != 0)
            {
                linkedStateMachine.ChangeState(PlayerStates.State.Run);
            }
            else
            {
                linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            }
            return;
        }

        // Se o tempo acabou, muda para Idle ou Jump dependendo se está no chão
        if (wallJumpTimer <= 0)
        {
            if (linkedPhysics.grounded)
            {
                linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            }
            else
            {
                linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            }
            return;
        }

        // Se já pode sair do wall jump e ainda está na parede, volta para WallSlide
        if (wallJumpMinimumTime <= 0 && linkedPhysics.wallDetected)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.WallSlide);
            wallJumpTimer = -1;
        }
    }

    /// <summary>
    /// Verifica se as condições para pular da parede estão corretas.
    /// </summary>
    private bool EvaluateWallJumpConditions()
    {
        // Não pode pular se está no chão ou não está encostado na parede
        if (linkedPhysics.grounded || !linkedPhysics.wallDetected)
        {
            return false;
        }
        return true;
    }
}
