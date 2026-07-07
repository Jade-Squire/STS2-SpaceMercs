using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Keywords;
using SpaceMercs.SpaceMercsCode.Powers;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace SpaceMercs.SpaceMercsCode.Cards.Uncommon;

public class Satiation() : SpaceMercsCard(3,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(3, ValueProp.Move),
        new PowerVar<HungerPower>(0)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<HungerPower>(choiceContext, Owner.Creature, DynamicVars[nameof(HungerPower)].BaseValue,
            Owner.Creature, this);
        
        List<CardModel> voids = new();
        foreach (var card in PileType.Draw.GetPile(Owner).Cards)
        {
            if (card is Void)
            {
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
                voids.Add(card);
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
        DynamicVars[nameof(HungerPower)].UpgradeValueBy(2);
        AddKeyword(SpaceMercsKeywords.GainsHunger);
    }
}