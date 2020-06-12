using UnityEngine;

public class PlayAnimOnEnable : MonoBehaviour
{
    public Animator anim;

    public void Awake()
    {
        anim.SetTrigger("Start");
    }

    private void OnEnable()
    {
        anim.SetTrigger("Start");
    }

    private void OnDisable()
    {
        anim.SetTrigger("End");
    }
}
