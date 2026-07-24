namespace SpaceMercs.SpaceMercsCode.Hooks;

public interface IShouldLoseHunger
{
    public bool ShouldLoseHungerAtTurnStart() => true;

    public void AfterHungerLossPrevented()
    {
        
    }
}