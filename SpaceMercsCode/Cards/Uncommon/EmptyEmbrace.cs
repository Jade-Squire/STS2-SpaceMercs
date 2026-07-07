using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Powers;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class EmptyEmbrace() : SpaceMercsCard(2,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Void>(),
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<EmptyEmbracePower>(3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<EmptyEmbracePower>(choiceContext, Owner.Creature, DynamicVars[nameof(EmptyEmbracePower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(EmptyEmbracePower)].UpgradeValueBy(2);
    }
}