using UnityEngine;

namespace Enemies
{
    public class Orge : TheEnemy
    {
        public Vector2[] variantsPositions1;
        public Vector2[] variantsPositions2;

        protected override void Start()
        {
            base.Start();
            variantsPositions = variantsPositions1;
        }

        protected override void TurnOver()
        {
            switch (moveAnimation.name)
            {
                case "Run2":
                    animator.Play("OrgeIdle1");
                    moveAnimation = new AnimationType("Run1", moveAnimation.speed);
                    variantsPositions = variantsPositions1;
                    break;
                case "Run1":   
                    animator.Play("OrgeIdle2");
                    moveAnimation = new AnimationType("Run2", moveAnimation.speed);
                    variantsPositions = variantsPositions2;
                    break;
            }
            base.TurnOver();
        }
    }
}
