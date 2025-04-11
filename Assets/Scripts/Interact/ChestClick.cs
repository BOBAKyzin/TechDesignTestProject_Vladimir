using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ChestClick : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private int clicksToOpen = 4;
    private int clickCount = 0;
    private bool isOpened = false;

    [Header("Effects")]
    public AudioSource shakeAudio;
    public AudioSource openAudio;
    public ParticleSystem openVFX;

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        if (isOpened) return;

        clickCount++;

        if (clickCount >= clicksToOpen)
        {
            isOpened = true;
            anim.SetTrigger("Open");

            if (openAudio != null)
                openAudio.Play();

            if (openVFX != null)
                openVFX.Play();
        }
        else
        {
            if (shakeAudio != null)
                shakeAudio.Play();
            anim.SetTrigger("Shake");
        }
    }
}
