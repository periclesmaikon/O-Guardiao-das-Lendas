using UnityEngine;

public class Player : MonoBehaviour {

    CharacterController controller;

    [Header("Configurações de Áudio")]
    private AudioSource footstepsAudio;

    Vector3 forward;
    Vector3 strafe;
    Vector3 vertical;

    float forwardSpeed = 2.5f;
    //float strafeSpeed = 2.5f;

    float gravity;
    float jumpSpeed;
    float maxJumpHeight = 0.5f;
    float timeToMaxHeight = 0.3f;

    void Start() {
        controller = GetComponent<CharacterController>();
        footstepsAudio = GetComponent<AudioSource>(); // Inicializa o áudio

        gravity = (-2 * maxJumpHeight) / (timeToMaxHeight * timeToMaxHeight);
        jumpSpeed = (2 * maxJumpHeight) / timeToMaxHeight;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        float forwardInput = Input.GetAxisRaw("Vertical");
        float strafeInput = Input.GetAxisRaw("Horizontal");

        // vetor de direção
        Vector3 moveDirection = new Vector3(strafeInput, 0, forwardInput);

        // normaliza a velocidade caso esteja na diagonal (soma valores vertical e horizontal)
        if (moveDirection.magnitude > 1) {
            moveDirection.Normalize();
        }

        // transforma essa direção para o espaço do mundo (baseado na rotação do player)
        Vector3 horizontalVelocity = transform.TransformDirection(moveDirection) * forwardSpeed;

        //gravidade e pulo
        vertical += gravity * Time.deltaTime * Vector3.up;

        if(controller.isGrounded) {
            vertical = Vector3.down;
        }

        if(Input.GetKeyDown(KeyCode.Space) && controller.isGrounded) {
            vertical = jumpSpeed * Vector3.up;
        }

        if (vertical.y > 0 && (controller.collisionFlags & CollisionFlags.Above) != 0) {
            vertical = Vector3.zero;
        }

        Vector3 finalVelocity = horizontalVelocity + vertical;
        controller.Move(finalVelocity * Time.deltaTime);

        ControlarPassos(forwardInput, strafeInput);
    }

    void ControlarPassos(float fInput, float sInput) {
        // Verifica se há input de movimento E se o jogador está no chão
        bool estaMovendo = (fInput != 0 || sInput != 0) && controller.isGrounded;

        if (estaMovendo) {
            if (!footstepsAudio.isPlaying) {
                footstepsAudio.Play();
            }
        } else {
            if (footstepsAudio.isPlaying) {
                footstepsAudio.Stop();
            }
        }
    }
}