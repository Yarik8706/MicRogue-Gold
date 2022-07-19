using UnityEngine;

public class BaseAnimations : MonoBehaviour // объект который есть у каждой сущности для выполнения разного рода анимации
{
    // будет ли объект удаляться если выполнит анимацию?
    [HideInInspector] public bool isDied;
    [HideInInspector] public Animator animator;
    
    private static readonly int Died = Animator.StringToHash("Died");
    private static readonly int Freezing = Animator.StringToHash("Freezing");
    private static readonly int Frostbite = Animator.StringToHash("Frostbite");
    private static readonly int SlimeBum = Animator.StringToHash("SlimeBum");
    public static readonly int IsSlimeCannon = Animator.StringToHash("isSlimeCannon");

    private void Awake()
    {
        isDied = true;
        animator = GetComponent<Animator>();
    }

    public void DiedAnimation()
    {
        animator.SetTrigger(Died);
    }

    public void FreezingAnimation()
    {
        animator.SetTrigger(Freezing);
    }

    public void FrostbiteAnimation()
    {
        animator.SetTrigger(Frostbite);
    }

    public void SlimeBumAnimation()
    {
        animator.SetTrigger(SlimeBum);
    }
    
    public void DiedEvent()
    {
        if (!isDied) return;
        Destroy(gameObject);
    }
}
