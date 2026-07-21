using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using SpaceMercs.SpaceMercsCode.Character;
using SpaceMercs.SpaceMercsCode.Relics;

namespace SpaceMercs.SpaceMercsCode.Ancients;

public class Traveler : CustomAncientModel
{
    protected override OptionPools MakeOptionPools => new OptionPools(
        generateNeowCurseOptions(), generateNeowPositiveOptions(), [AncientOption<FocusedLight>()]
    );

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> list1 = generateNeowCurseOptions().Select<AncientOption, EventOption>(option => RelicOption(option.ModelForOption)).ToList();
        list1.RemoveAll((Predicate<EventOption>) (r =>
        {
            RelicModel relic = r.Relic;
            return relic != null && !relic.IsAllowedAtNeow(Owner);
        }));
        EventOption eventOption = Rng.NextItem(list1);
        List<EventOption> list2 = generateNeowPositiveOptions().Select<AncientOption, EventOption>(option => RelicOption(option.ModelForOption)).ToList();
        if (eventOption.Relic is CursedPearl)
            list2.RemoveAll((Predicate<EventOption>) (o => o.Relic is GoldenPearl));
        if (eventOption.Relic is HeftyTablet)
            list2.RemoveAll((Predicate<EventOption>) (o => o.Relic is ArcaneScroll));
        if (eventOption.Relic is LeafyPoultice)
            list2.RemoveAll((Predicate<EventOption>) (o => o.Relic is NewLeaf));
        if (eventOption.Relic is PrecariousShears)
            list2.RemoveAll((Predicate<EventOption>) (o => o.Relic is PreciseScissors));
        if (!(eventOption.Relic is LargeCapsule))
        {
            if (Rng.NextBool())
                list2.Add(RelicOption<LavaRock>());
            else
                list2.Add(RelicOption<SmallCapsule>());
        }
        if (Rng.NextBool())
            list2.Add(RelicOption<NutritiousOyster>());
        else
            list2.Add(RelicOption<StoneHumidifier>());
        if (Rng.NextBool())
            list2.Add(RelicOption<NeowsTalisman>());
        else
            list2.Add(RelicOption<Pomander>());
        list2.RemoveAll((Predicate<EventOption>) (r =>
        {
            RelicModel relic = r.Relic;
            return relic != null && !relic.IsAllowedAtNeow(Owner);
        }));
        List<EventOption> items = new();
        items.AddRange(list2.ToList().UnstableShuffle(Rng).Take(Owner.Character is Cosmopaladin? 1 : 2));
        items.Add(eventOption);
        if(Owner.Character is Cosmopaladin)
            items.Add(RelicOption<FocusedLight>());
        return items;
    }

    public override string? CustomMapIconPath =>
        "res://SpaceMercs/images/packed/map/ancients/ancient_node_spacemercs-traveler.png";

    public override string? CustomMapIconOutlinePath => "res://SpaceMercs/images/packed/map/ancients/ancient_node_spacemercs-traveler_outline.png";

    public override string? CustomRunHistoryIconPath =>
        "res://SpaceMercs/images/ui/run_history/spacemercs-traveler.png";

    public override string? CustomRunHistoryIconOutlinePath => "res://SpaceMercs/images/ui/run_history/spacemercs-traveler_outline.png";

    public override string? CustomScenePath =>
        "res://SpaceMercs/scenes/events/background_scenes/spacemercs-traveler.tscn";

    private WeightedList<AncientOption> generateNeowCurseOptions()
    {
        WeightedList<AncientOption> options = new();
        options.Add(AncientOption<CursedPearl>());
        options.Add(AncientOption<HeftyTablet>());
        options.Add(AncientOption<LargeCapsule>());
        options.Add(AncientOption<LeafyPoultice>());
        options.Add(AncientOption<NeowsBones>());
        options.Add(AncientOption<PrecariousShears>());
        options.Add(AncientOption<SilkenTress>());
        options.Add(AncientOption<SilverCrucible>());
        return options;
    }
    
    private WeightedList<AncientOption> generateNeowPositiveOptions()
    {
        WeightedList<AncientOption> options = new();
        options.Add(AncientOption<ArcaneScroll>());
        options.Add(AncientOption<BoomingConch>());
        options.Add(AncientOption<FishingRod>());
        options.Add(AncientOption<GoldenPearl>());
        options.Add(AncientOption<Kaleidoscope>());
        options.Add(AncientOption<LeadPaperweight>());
        options.Add(AncientOption<LostCoffer>());
        options.Add(AncientOption<MassiveScroll>());
        options.Add(AncientOption<NeowsTorment>());
        options.Add(AncientOption<NewLeaf>());
        options.Add(AncientOption<PhialHolster>());
        options.Add(AncientOption<PreciseScissors>());
        options.Add(AncientOption<ScrollBoxes>());
        options.Add(AncientOption<WingedBoots>());
        return options;
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 1;
    }

    public override bool ShouldForceSpawn(ActModel act, AncientEventModel? rngChosenAncient)
    {
        foreach (var player in RunManager.Instance.State.Players)
        {
            if (player.Character is Cosmopaladin)
            {
                return IsValidForAct(act);
            }
        }

        return false;
    }
}