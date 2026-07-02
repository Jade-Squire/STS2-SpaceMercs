using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SpaceMercs.SpaceMercsCode.Nodes;

namespace SpaceMercs.SpaceMercsCode.AddedNodes;

public class AddedNodes
{
    public static AddedNode<NEnergyCounter, NDeterminationCounter> NDeterminationCounter = new(
        "res://SpaceMercs/scenes/DeterminationCounter.tscn/",
        (energyCounter, display) =>
        {
            var energyContainer = energyCounter.GetChild(0);
            
            energyContainer.AddChild(display);
        });
}