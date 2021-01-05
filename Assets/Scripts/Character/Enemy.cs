using UnityEngine;

public sealed class Enemy : MonoBehaviour
{
    [SerializeField]
    private float maxX = 250.0f;

    [SerializeField]
    private float minX = -250.0f;

    [SerializeField]
    private float xSpeed = 50.0f;

    [SerializeField]
    private float ySpeed = 1.0f;

    [SerializeField]
    private float yAmplitude = 30.0f;

    private bool movingRight = true;

    private bool waiting = true;
    private float waitingTime = 14.0f;
    private float timeAcc;

    private float baseY;

    private bool gameOver = false;

    public void OnGameOver()
    {
        gameOver = true;
    }

    private void Start()
    {
        FindObjectOfType<Board>().AddPersistentObject(GetComponent<BoardObjectWithOffsets>());
        baseY = transform.position.y;
    }

    private void Update()
    {
        if (!gameOver)
        {
            UpdateState();
        }
    }

    private void UpdateState()
    {
        if (waiting)
        {
            if (timeAcc >= waitingTime)
            {
                timeAcc = 0.0f;
                waiting = false;
                waitingTime = Mathf.Clamp(waitingTime - 1.0f, 0.0f, waitingTime);
            }
            else
            {
                timeAcc += Time.deltaTime;
            }
        }
        else
        {
            if (movingRight)
            {
                if (transform.position.x >= maxX)
                {
                    FlipOrientation();
                }

                transform.position += xSpeed * Time.deltaTime * Vector3.right;
            }
            else
            {
                if (transform.position.x <= minX)
                {
                    FlipOrientation();
                }

                transform.position += xSpeed * Time.deltaTime * Vector3.left;
            }

            transform.position = new Vector3(transform.position.x, baseY + Mathf.Sin(Time.time * ySpeed) * yAmplitude, transform.position.z);
        }
    }

    private void FlipOrientation()
    {
        movingRight = !movingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        waiting = true;
        timeAcc = 0.0f;
    }
}
