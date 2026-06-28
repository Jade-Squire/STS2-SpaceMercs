using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class PercussiveFlames() : SpaceMercsCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Burn>(),
        HoverTipFactory.FromPower<ScorchPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("Burns", 3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<PercussiveFlamesPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        await CardPileCmd.AddToCombatAndPreview<Burn>(Owner.Creature, PileType.Hand, DynamicVars["Burns"].IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Burns"].UpgradeValueBy(-2);
    }
}