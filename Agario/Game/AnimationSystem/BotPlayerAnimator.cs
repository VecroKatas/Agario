using SFML.System;

namespace Agario.Game.AnimationSystem;

public class BotPlayerAnimator : AnimatorBase
{
    public override void Play(string name)
    {
        if (name == "BotPlayerWalking")
        {
            Vector2f direction = GameObject.GetComponent<PlayerGameObject>().CurrentMoveDirection;
            if (MathF.Abs(direction.Y) >= MathF.Abs(direction.X) && direction.Y < 0)
                base.Play("HumanPlayerWalkingUp");
            else if (MathF.Abs(direction.Y) >= MathF.Abs(direction.X) && direction.Y >= 0)
                base.Play("HumanPlayerWalkingDown");
            else if (MathF.Abs(direction.Y) <= MathF.Abs(direction.X) && direction.X >= 0)
                base.Play("HumanPlayerWalkingRight");
            else if (MathF.Abs(direction.Y) <= MathF.Abs(direction.X) && direction.X < 0)
                base.Play("HumanPlayerWalkingLeft");
        }
        else if (name == "BotPlayerEating")
        {
            Vector2f direction = GameObject.GetComponent<PlayerGameObject>().CurrentMoveDirection;
            if (MathF.Abs(direction.Y) >= MathF.Abs(direction.X) && direction.Y < 0)
                base.Play("HumanPlayerEatingUp");
            else if (MathF.Abs(direction.Y) >= MathF.Abs(direction.X) && direction.Y >= 0)
                base.Play("HumanPlayerEatingDown");
            else if (MathF.Abs(direction.Y) <= MathF.Abs(direction.X) && direction.X >= 0)
                base.Play("HumanPlayerEatingRight");
            else if (MathF.Abs(direction.Y) <= MathF.Abs(direction.X) && direction.X < 0)
                base.Play("HumanPlayerEatingLeft");
        }
        else
        {
            base.Play(name);
        }
    }
}