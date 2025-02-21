using SFML.System;

namespace Agario.Game.AnimationSystem;

public class HumanPlayerAnimator : AnimatorBase
{
    public override void Play(string name)
    {
        if (name == "HumanPlayerWalking")
        {
            Vector2f direction = GameObject.GetComponent<PlayerGameObject>().CurrentMoveDirection;
            if (MathF.Abs(direction.Y) >= MathF.Abs(direction.X) && direction.Y < 0)
                Play("HumanPlayerWalkingUp");
            else if (MathF.Abs(direction.Y) >= MathF.Abs(direction.X) && direction.Y >= 0)
                Play("HumanPlayerWalkingDown");
            else if (MathF.Abs(direction.Y) <= MathF.Abs(direction.X) && direction.X >= 0)
                Play("HumanPlayerWalkingRight");
            else if (MathF.Abs(direction.Y) <= MathF.Abs(direction.X) && direction.X < 0)
                Play("HumanPlayerWalkingLeft");
        }
        else
        {
            base.Play(name);
        }
    }
}