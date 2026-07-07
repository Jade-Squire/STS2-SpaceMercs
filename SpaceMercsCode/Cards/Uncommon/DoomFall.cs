using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class DoomFall() : SpaceMercsCard(3,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Void>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
        
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(2, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int count = 0;
        List<CardModel> voids = new();
        foreach (var card in PileType.Draw.GetPile(Owner).Cards)
        {
            if (card is Void)
            {
                voids.Add(card);
                count++;
            }
        }
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Targeting(play.Target)
            .WithHitCount(count)
            .Execute(choiceContext);
        
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
        DynamicVars.Damage.UpgradeValueBy(1);
    }
}