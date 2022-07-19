using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface IClickToAvailablePosition: IPointerClickHandler
{
    public void ClickEvent();
} 

public class Player : TheEssence, IPointerClickHandler, IStuckInSlime
{
    public LayerMask enemyLayer;
    public GameObject canvasDialogFrame;
    public GameObject moveToPlace;
    public Animator[] canvasShieldAnimators;
    public Text causeOfDiedText;
    
    internal int shieldsCount;
    internal bool playerHasRiched;

    private List<GameObject> _moveToPlaces;
    private GameManager _gameManager;

    protected override void Start()
    {
        base.Start();
        shieldsCount = canvasShieldAnimators.Length;
    }

    public void Stuck()
    {
        StartCoroutine(StuckWhile());
    }

    private IEnumerator StuckWhile()
    { 
        var baseVariantsPositions = variantsPositions;
        var newVariantsPositions = baseVariantsPositions.Where(variantPosition => Vector2.Distance(variantPosition, new Vector2(0, 0)) < 2).ToArray();
        variantsPositions = newVariantsPositions;
        var slimeCannon = Instantiate(baseAnimationsObj, transform.position, Quaternion.identity)
                    .GetComponent<BaseAnimations>();
        slimeCannon.animator.SetBool(BaseAnimations.IsSlimeCannon, true);
        yield return new WaitUntil(() => isMove);
        slimeCannon.animator.SetBool(BaseAnimations.IsSlimeCannon, false);
        slimeCannon.SlimeBumAnimation();
        variantsPositions = baseVariantsPositions;
    }

    public override void Awake()
    {
        base.Awake();
        _gameManager = FindObjectOfType<GameManager>();
        _moveToPlaces = new List<GameObject>();
    }

    public override void Active()
    {
        base.Active();
        // проверяем врагов на пути и не даем пройти за ними
        foreach (var newVariantPosition in MoveCalculation(VariantsPositionsNow(variantsPositions)))
        {
			boxCollider2D.enabled = false;
			var hit = Physics2D.Linecast(transform.position, newVariantPosition, enemyLayer);
			boxCollider2D.enabled = true;
            if (hit.collider == null)
            {
                _moveToPlaces.Add(Instantiate(moveToPlace, newVariantPosition, Quaternion.identity));
            }
            else if(hit.collider != null)
            {
                if (hit.collider.gameObject.transform.position != (Vector3)newVariantPosition) continue;
                _moveToPlaces.Add(Instantiate(moveToPlace, newVariantPosition, Quaternion.identity));
            }
        }
    }

    protected override Vector2[] MoveCalculation(Vector2[] theVariantsPositions)
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
            else if(hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<IClickToAvailablePosition>() is {} thisObject)
                {
                    thisObject.ClickEvent();
                }
            }
        }
        return nowVariantsPositions.ToArray();
    }

    public override IEnumerator Move(Vector3 @where)
    {
        DeleteAllMoveToPlaces();
        var x = @where.x - transform.position.x;
        if(x + 2 == 0 && !turnedRight || x - 2 == 0 && turnedRight)
        {
            Flip();
        }
        else if(x != 0 && (turnedRight && x >= 1 || !turnedRight && x <= -1))
        {
            boxCollider2D.enabled = false;
            var hit = Physics2D.Linecast (transform.position, @where, enemyLayer);
            boxCollider2D.enabled = true;
            if (hit.collider != null)
            {
                Flip();
            }
        }
        StartCoroutine(base.Move(@where));
        yield return null;
    }

    public void LossOfShieldEvent()
    {
        StartAnimation("PlayerProtection");
        shieldsCount--;
        canvasShieldAnimators[shieldsCount].Play("LossOfShield");
        if (shieldsCount == 0)
        {
            animator.SetBool("IdleWithoutShield", true);
        }
    }

    public void RestorationOfShields()
    {
        shieldsCount = canvasShieldAnimators.Length;
        foreach (var canvasShieldAnimator in canvasShieldAnimators)
        {
            canvasShieldAnimator.Play("CanvasShieldIdle");
        }
        animator.SetBool("IdleWithoutShield", false);
    }

    private void DeleteAllMoveToPlaces()
    {
        foreach(var i in _moveToPlaces)
        {
            Destroy(i);
        }
    }

    protected override void TurnOver()
    {
        base.TurnOver();
        StartCoroutine(_gameManager.TurnEnded());
    }

    public void OnPointerClick(PointerEventData @event)
    {
        if(isTurnOver || @event.rawPointerPress.name != gameObject.name) return;
        Flip();
        DeleteAllMoveToPlaces();
        TurnOver();
    }

    public void Died(string causeOfDied)
    {
        canvasDialogFrame.SetActive(true);
        causeOfDiedText.text = causeOfDied;
        base.Died();
    }
    
    public override void Died()
    {
        if (causeOfDiedThisEnemy != null) causeOfDiedText.text = causeOfDiedThisEnemy.CauseOfDied();
        else if(essenceHeDied != null)
        {
            if(essenceHeDied is ICauseOfDied icauseOfDied)
            {
                causeOfDiedText.text = icauseOfDied.CauseOfDied();
            }
        }
        canvasDialogFrame.SetActive(true);
        base.Died();
    }
}
