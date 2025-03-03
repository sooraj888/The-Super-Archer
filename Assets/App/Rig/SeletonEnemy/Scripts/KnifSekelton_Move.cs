
public class KnifSekelton_Move : ISekeltonKnif
{
    public void EnterState(SekeltonKnifStateMachine character)
    {
        character.PlayAnimation("Walk");
    }

    public void UpdateState(SekeltonKnifStateMachine character)
    {
        if (character.IsNearPlayer)
        {
            character.ChangeState(character.killState);
            return;
        }
        if (!character.IsMove)
        {
            character.ChangeState(character.idleState);
        }
    }

    public void ExitState(SekeltonKnifStateMachine character)
    {
        // Optional: Logic when stopping movement
    }

    
}
