using UnityEngine;

namespace Enemies
{
    public interface IFireAttack
    {
        public void FireAttack(GameObject fire);
    }
    
    public class LifeFire : TheEnemy, ICold
    {
        public LayerMask noFireLayer;
        public Vector2[] firePosition;
        public GameObject fire;

        public static string[] causeOfDiedFire;

        protected override void Start()
        {
            causeOfDiedFire = causeOfDied;
            base.Start();
            // lifeFire not died for Fire
            attackActions[AttackType.Fire] = () => {};
        }

        public override void Died()
        {
            var newVariantPosition = VariantsPositionsNow(firePosition);
            foreach (var position in newVariantPosition)
            {
                boxCollider2D.enabled = false;
                
                var hit = Physics2D.Linecast(transform.position, position, noFireLayer);
                
                boxCollider2D.enabled = true;
                //луч ни с чем не пересёкся
                if (hit.collider == null)
                {
                    Instantiate(fire, position, Quaternion.identity);
                }
                else if(hit.collider.gameObject.GetComponent<IFireAttack>() is {} fireAttack)
                {
                    fireAttack.FireAttack(fire);
                }
            }
            base.Died();
        }

        public void Cold(GameObject cold)
        {
            Destroy(cold);
            Instantiate(baseAnimationsObj, transform.position, Quaternion.identity)
                .GetComponent<BaseAnimations>().DiedAnimation();
            Destroy(gameObject);
        }
    }
}
