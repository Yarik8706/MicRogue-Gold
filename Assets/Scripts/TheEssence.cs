using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
	Null, // Player attack
	Cold,
	Melee,
	Fire
}

public struct AnimationType
{
	public readonly string name;
	public float speed;

	public AnimationType(string name = "", float speed = 1)
	{
		this.name = name;
		this.speed = speed;
	}
}

public interface ICauseOfDied
{
	public string CauseOfDied();
}

public abstract class TheEssence : MonoBehaviour
{
	[Header("Movement Parameters")]
	public LayerMask blockingLayer;
	public float moveTime = .1f;
	public Vector2[] variantsPositions;
	[Header("Animations Parameters")] 
    public string moveAnimationName;
    public string diedAnimationName;
    [Header("Other")] 
    public GameObject baseAnimationsObj;
    
    private protected bool turnedRight;
    private protected Collider2D boxCollider2D;
    private protected float inverseMoveTime;
    private protected Dictionary<AttackType, Action> attackActions;
    private protected AnimationType diedAnimation;
    private protected TheEssence essenceHeDied;
    private protected ICauseOfDied causeOfDiedThisEnemy;
    private protected AnimationType moveAnimation;
    private Rigidbody2D _rigidbody2d;
    
    internal Animator animator;
    internal bool isTurnOver;
    internal bool isMove;
    internal bool isActive;

    protected virtual void Start()
    {
    }
    
    public virtual void Awake()
    {
	    isActive = true;
	    moveAnimation = new AnimationType(moveAnimationName);
	    diedAnimation = new AnimationType(diedAnimationName);
	    turnedRight = !(transform.localScale.x > 0);
	    inverseMoveTime = 1f / moveTime;
	    attackActions = new Dictionary<AttackType, Action>
	    {
		    {AttackType.Null, Died},
		    {AttackType.Fire, Died},
		    {AttackType.Melee, Died}
	    };
	    animator = GetComponent<Animator>();
	    boxCollider2D = GetComponent<BoxCollider2D>();
	    _rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public virtual void Active()
    {
	    isTurnOver = false;
    }

    protected virtual Vector2[] MoveCalculation(Vector2[] theVariantsPositions)
    {
	    var nowVariantsPositions = new List<Vector2> {Capacity = 0};
	    foreach (var newVariantPosition in theVariantsPositions)
	    {
		    boxCollider2D.enabled = false;
		    var hit = Physics2D.Linecast (transform.position, newVariantPosition, blockingLayer);
		    boxCollider2D.enabled = true;
		    if (hit.collider == null)
		    {
			    nowVariantsPositions.Add(newVariantPosition);
		    }
	    }
	    return nowVariantsPositions.ToArray();
    }

    protected void SetAnimationMoveSpeed(float animationSpeed, float moveTimeSpeed)
    {
	    moveAnimation.speed = animationSpeed;
	    moveTime = moveTimeSpeed;
	    inverseMoveTime = 1f / moveTime;
    }

    protected void Flip()
    {
        turnedRight = !turnedRight;
        var scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    
    protected void Flip(bool turnedRight)
    {
	    this.turnedRight = turnedRight;
	    var scaler = transform.localScale;
	    scaler.x = turnedRight ? 1 : -1;
	    transform.localScale = scaler;
    }
    
    public virtual IEnumerator Move(Vector3 @where)
    {
	    StartAnimationTrigger(moveAnimation);
	    StartCoroutine(SmoothMovement(@where));
	    yield return new WaitUntil(() => transform.position == @where);
	    TurnOver();
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
	    isMove = true;
	    yield return new WaitForSeconds(0.1f);
	    while(transform.position != end)
	    {
		    var newPosition = Vector3.MoveTowards(
			    _rigidbody2d.position, 
			    end, 
			    inverseMoveTime * Time.deltaTime);
		    transform.position = newPosition;
		    yield return null;
	    }
		_rigidbody2d.MovePosition(end);
		isMove = false;
    }

    protected Vector2[] VariantsPositionsNow(Vector2[] positions)
    {
        var newVariantsPositions = new List<Vector2>{Capacity = 0};
        for(var i = 0; i < positions.Length; i++)
        {
	        var position = transform.position;
	        newVariantsPositions.Add(new Vector2(position.x + positions[i].x, position.y + positions[i].y));
        }
        return newVariantsPositions.ToArray();
    }

    protected void StartAnimationTrigger(AnimationType animationType)
    {
	    if (animationType.name == "")
	    {
		    return;
	    }
	    animator.speed = animationType.speed;
	    animator.SetTrigger(animationType.name);
    }
    
    protected void StartAnimationTrigger(string trigger)
    {
	    if (trigger == "")
	    {
		    return;
	    }
	    animator.speed = 1;
	    animator.SetTrigger(trigger);
    }
    
    public void StartAnimation(string nameAnimation)
    {
	    if (nameAnimation == "")
	    {
		    return;
	    }
	    animator.speed = 1;
	    animator.Play(nameAnimation);
    }

    protected virtual void TurnOver()
    {
	    isMove = false;
	    isTurnOver = true;
    }

    public void GetDamage(AttackType type = AttackType.Null)
    {
	    attackActions[type].Invoke();
    }

    public void GetDamage(TheEssence attackEnemy, AttackType type = AttackType.Null)
    {
	    essenceHeDied = attackEnemy;
	    attackActions[type].Invoke();
    }
    
    public void GetDamage(ICauseOfDied attackEnemy, AttackType type = AttackType.Null)
    {
	    causeOfDiedThisEnemy = attackEnemy;
	    attackActions[type].Invoke();
    }

    public virtual void Died()
    {
	    StartAnimationTrigger(diedAnimation);
	    Instantiate(baseAnimationsObj, transform.position, Quaternion.identity)
		    .GetComponent<BaseAnimations>().DiedAnimation();
	    TurnOver();
	    Destroy(gameObject);
    }
}
