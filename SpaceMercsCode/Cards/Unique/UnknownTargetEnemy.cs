using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SpaceMercs.SpaceMercsCode.Cards;

namespace SpaceMercs.SpaceMercsCode.Cards.Unique;

public class UnknownTargetEnemy() : UnknownBase(-1, CardType.Status, CardRarity.Token, TargetType.AnyEnemy)
{
    
}