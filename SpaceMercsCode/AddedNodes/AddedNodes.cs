using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SpaceMercs.SpaceMercsCode.Nodes;

namespace SpaceMercs.SpaceMercsCode.AddedNodes;

public class AddedNodes
{
    public static AddedNode<NCombatUi, NDeterminationCounter> NDeterminationCounter = new(ui =>
    {
        var determinationCounter = PreloadManager.Cache.GetScene("res://SpaceMercs/scenes/DeterminationCounter.tscn")
            .Instantiate<NDeterminationCounter>();
        ui.AddChild(determinationCounter);
        return determinationCounter;
    });
}