using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Cards.Basic;
using SpaceMercs.SpaceMercsCode.Cards.Common;
using SpaceMercs.SpaceMercsCode.Cards.Uncommon;
using SpaceMercs.SpaceMercsCode.Cards.Unique;
using SpaceMercs.SpaceMercsCode.Relics;

namespace SpaceMercs.SpaceMercsCode.Relics;

public class FocusedLight() : SpaceMercsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Overwhelmed>()
    ];

    public override async Task AfterObtained()
    {
        List<CardModel> cardsToRemove = Owner.Deck.Cards.ToList().FindAll(card => !card.Keywords.Contains(CardKeyword.Eternal) && card is not RememberedVow && card is not BrokenOath && card is not StrikeCosmopaladin && card is not DefendCosmopaladin);

        foreach (var card in cardsToRemove)
        {
            await CardPileCmd.RemoveFromDeck(card, false);
        }


        List<IReadOnlyList<CardModel>> randomBundles = GenerateBundles(Owner);
        List<IReadOnlyList<CardModel>> bundles = new();
        foreach (var source in randomBundles)
        {
            bundles.Add(source);
        }

        foreach (var card in await CardSelectCmd.FromChooseABundleScreen(Owner, bundles))
        {
            await CardPileCmd.Add(card, PileType.Deck);
        }
        await CardPileCmd.Add(Owner.RunState.CreateCard<Overwhelmed>(Owner), PileType.Deck);
        
    }

    private List<IReadOnlyList<CardModel>> GenerateBundles(Player owner)
    {
        IReadOnlyList<CardModel> solarElement = new List<CardModel>([
            owner.RunState.CreateCard<HammerStrike>(owner),
            owner.RunState.CreateCard<ThrowingHammer>(owner),
            owner.RunState.CreateCard<Jetpack>(owner),
            owner.RunState.CreateCard<IncendiaryGrenade>(owner)
        ]);
        IReadOnlyList<CardModel> voidElement = new List<CardModel>([
            owner.RunState.CreateCard<PurgingMaw>(owner),
            owner.RunState.CreateCard<Gnaw>(owner),
            owner.RunState.CreateCard<SmokeScreen>(owner),
            owner.RunState.CreateCard<SuppressionGrenade>(owner)
        ]);
        IReadOnlyList<CardModel> arcElement = new List<CardModel>([
            owner.RunState.CreateCard<Poke>(owner),
            owner.RunState.CreateCard<Boost>(owner),
            owner.RunState.CreateCard<TacticalStrafe>(owner),
            owner.RunState.CreateCard<FlashGrenade>(owner)
        ]);
        IReadOnlyList<CardModel> stasisElement = new List<CardModel>([
            owner.RunState.CreateCard<Icefall>(owner),
            owner.RunState.CreateCard<ColdStare>(owner),
            owner.RunState.CreateCard<HunkerDown>(owner),
            owner.RunState.CreateCard<GlacierGrenade>(owner)
        ]);
        
        return [solarElement, voidElement, arcElement, stasisElement];
    }
}