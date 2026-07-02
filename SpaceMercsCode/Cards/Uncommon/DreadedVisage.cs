using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class DreadedVisage() : SpaceMercsCard(3,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<SuppressPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SuppressPower>(),
        HoverTipFactory.FromCard<Void>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        List<CardModel> voids = new();
        
        foreach (CardModel card in PileType.Draw.GetPile(Owner).Cards)
        {
            if (card is Void)
            {
                voids.Add(card);
                await PowerCmd.Apply<SuppressPower>(choiceContext, CombatState.HittableEnemies,
                    DynamicVars[nameof(SuppressPower)].BaseValue, Owner.Creature, this);
            }
        }

        foreach (CardModel card in voids)
        {
            if (voids.Count > 3)
            {
                CardCmd.Exhaust(choiceContext, card);
                await Cmd.CustomScaledWait(0.1f, 0.2f);
            }
            else
            {
                await CardCmd.Exhaust(choiceContext, card);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(SuppressPower)].UpgradeValueBy(1);
    }
}