using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace SpaceMercs.SpaceMercsCode.Combat;

public class SpacemercsCustomPile() : CustomPile(Aether)
{
    
    [CustomEnum] public static PileType Aether;
    
    public override bool CardShouldBeVisible(CardModel card)
    {
        return false;
    }

    public override Vector2 GetTargetPosition(CardModel model, Vector2 size)
    {
        return new Vector2(size.X / 2, size.Y);
    }
}