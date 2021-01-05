using UnityEngine;

public sealed class Shaker : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Shake()
    {
        var rand = Random.Range(0, 3);

        switch (rand)
        {
            case 0: animator.SetTrigger("shake"); break;
            case 1: animator.SetTrigger("shake2"); break;
            case 2: animator.SetTrigger("shake3"); break;
        }
    }
}
