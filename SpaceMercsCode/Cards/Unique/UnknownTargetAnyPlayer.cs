using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.Character;

namespace SpaceMercs.SpaceMercsCode.Cards.Unique;

[Pool(typeof(CosmopaladinUniqueCardPool))]
public class UnknownTargetAnyPlayer() : UnknownBase(-1, CardType.Status, CardRarity.Token, TargetType.AnyPlayer)
{
    
}