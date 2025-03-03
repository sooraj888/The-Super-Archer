
public class KnifSekelton_Kill : ISekeltonKnif
{
    public void EnterState(SekeltonKnifStateMachine character)
    {
        character.SetVeleocityX(0);
        character.PlayAnimation("Kill");
    }

    public void UpdateState(SekeltonKnifStateMachine character)
    {
        //// After a short delay, return to idle (or move if key still pressed)
        if (!character.IsNearPlayer)
        {
            if (character.IsMove)
            {
                character.ChangeState(character.moveState);
            }
            else
            {
                character.ChangeState(character.idleState);
            }
        }
    }

    public void ExitState(SekeltonKnifStateMachine character)
    {
        // Optional: Logic when stopping the attack
    }

}
