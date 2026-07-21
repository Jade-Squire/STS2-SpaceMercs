using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.RestSiteOptions;

namespace SpaceMercs.SpaceMercsCode.Cards.Unique;

public class Overwhelmed() : SpaceMercsCard(-1,
    CardType.Curse, CardRarity.Curse,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        
    }

    protected override void OnUpgrade()
    {
        
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Unplayable,
        CardKeyword.Innate,
        CardKeyword.Eternal
    ];

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner)
        {
            return false;
        }
        options.Add(new FocusRestSiteOption(player));
        return true;
    }
}