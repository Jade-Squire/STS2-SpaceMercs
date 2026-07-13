using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Common;

public class Alternate() : SpaceMercsCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (PileType.Hand.GetPile(Owner).IsEmpty)
        {
            return;
        }

        List<CardModel> list = (await CardSelectCmd.FromHandForDiscard(choiceContext, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this)).ToList();
        if (!list.Any())
        {
            return;
        }

        CardModel card = list.First();
        int cost = card.EnergyCost.CostsX ? 0 : card.EnergyCost.GetResolved();
        await CardCmd.Discard(choiceContext, card);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingRandomOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .WithHitCount(cost)
            .Execute(choiceContext);
        if (IsUpgraded)
        {
            await CardPileCmd.Draw(choiceContext, cost, Owner);
        }
        
    }

    protected override void OnUpgrade()
    {

    }
}