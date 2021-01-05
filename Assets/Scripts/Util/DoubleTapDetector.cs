using UnityEngine;
using UnityEngine.Events;

public sealed class DoubleTapDetector : MonoBehaviour
{
    [SerializeField]
    private float maxDoubleTapTime = 0.1f;

    [SerializeField]
    private UnityEvent OnDoubleTap;

    private int tapCount = 0;
    private float newTime = 0.0f;

    private void Awake()
    {
        if (OnDoubleTap == null)
        {
            OnDoubleTap = new UnityEvent();
        }
    }

    /// <summary>
    /// https://answers.unity.com/questions/369230/how-to-detect-double-tap-in-android.html
    /// </summary>
    private void Update()
    {
        CheckDoubleTap();
    }

    private void CheckDoubleTap()
    {
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                tapCount += 1;
            }

            if (tapCount == 1)
            {
                newTime = Time.time + maxDoubleTapTime;
            }
            else if (tapCount == 2 && Time.time <= newTime)
            {
                OnDoubleTap.Invoke();
                tapCount = 0;
            }
        }

        if (Time.time > newTime)
        {
            tapCount = 0;
        }
    }
}
