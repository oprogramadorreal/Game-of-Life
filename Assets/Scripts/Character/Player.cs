using UnityEngine;
using UnityEngine.UI;

public sealed class Player : MonoBehaviour
{
    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private GameObject coinDestructionParticlesPrefab;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private CharacterController2D controller;

    [SerializeField]
    private float runSpeed = 1000.0f;

    [SerializeField]
    private Text uiScore;

    private float horizontalMove = 0.0f;
    private bool needsToJump = false;

    private bool canJump = true;

    private int score = 0;

    private bool gameOver = false;

    public int Score => score;

    public void OnGameOver()
    {
        gameOver = true;
    }

    public void Jump()
    {
        if (canJump && !gameOver)
        {
            animator.SetBool("IsJumping", true);
            audioManager.CreateTemporaryAudioSource("Jump");
            needsToJump = true;
            canJump = false;
        }
    }

    public void SetHorizontalMove(float moveDirection)
    {
        if (moveDirection == 0.0f)
        {
            horizontalMove = 0.0f;
        }
        else
        {
            horizontalMove = Mathf.Sign(moveDirection) * runSpeed;
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Coin"))
        {
            ++score;
            uiScore.text = score.ToString();

            audioManager.CreateTemporaryAudioSource("Coin");

            var particles = Instantiate(coinDestructionParticlesPrefab, collider.transform.position, Quaternion.identity);
            Destroy(particles, 0.5f);

            collider.transform.parent.gameObject.SetActive(false); // assuming we've collided with a dummy child trigger
        }
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
        canJump = true;
    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("IsCrouching", isCrouching);
    }

    private void FixedUpdate()
    {
        if (!gameOver)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, false, needsToJump);
            needsToJump = false;
        }
    }
}
