
public class KnifSekelton_Idle : ISekeltonKnif
{
    public void EnterState(SekeltonKnifStateMachine character)
    {
        character.SetVeleocityX(0);
        character.PlayAnimation("Idle");
    }

    public void UpdateState(SekeltonKnifStateMachine character)
    {
        if (character.IsNearPlayer) {
            character.ChangeState(character.killState);
            return;
        }

        if (character.IsMove)
        {
            character.ChangeState(character.moveState);
        }
    }

    public void ExitState(SekeltonKnifStateMachine character)
    {
        // Optional: Logic when leaving idle
    }

   
}
