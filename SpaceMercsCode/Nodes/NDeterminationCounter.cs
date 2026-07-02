using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using SpaceMercs.SpaceMercsCode.Cards;
using SpaceMercs.SpaceMercsCode.CombatState;

namespace SpaceMercs.SpaceMercsCode.Nodes;

[GlobalClass]
public partial class NDeterminationCounter : NClickableControl
{
    private Player? _player;
    private Label? _determinationText;
    private HoverTip _hoverTip;
    
    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public override void _Ready()
    {
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "SPACEMERCS-DETERMINATION_COUNTER.title"), new LocString("static_hover_tips", "SPACEMERCS-DETERMINATION_COUNTER.description"));
        Connect(Godot.Control.SignalName.MouseEntered, Callable.From(OnHovered));
        Connect(Godot.Control.SignalName.MouseExited, Callable.From(OnUnhovered));
        Connect(NClickableControl.SignalName.MousePressed, Callable.From<InputEvent>(onClicked));
        _determinationText = GetNode<Label>("Label");
        _determinationText.Text = _player.PlayerCombatState.Cosmopaladin().Determination.ToString();
        _player.PlayerCombatState.Cosmopaladin().DeterminationChanged += DeterminationChanged;
        checkVisibility();
        base._Ready();
    }

    public void DeterminationChanged(int oldvalue, int newvalue)
    {
        _determinationText.Text = newvalue.ToString();
        checkVisibility();
    }

    public void OnHovered()
    {
        NHoverTipSet.CreateAndShow(this, _hoverTip)?.SetGlobalPosition(GlobalPosition + new Vector2(-70f, -200f));
    }

    public void OnUnhovered() => NHoverTipSet.Remove(this);

    public void onClicked(InputEvent inputEvent)
    {
        if (!Visible) return;

        PlayerCombatStateExtensions.CosmopaladinCombatState cosmo = _player.PlayerCombatState.Cosmopaladin();
        if (cosmo.Determination >= 2)
        {
            cosmo.LoseDetermination(2);
            _player.PlayerCombatState.GainEnergy(1);
        }
        else
        {
            GD.PrintErr("Not enough determination");
        }
    }

    public void checkVisibility()
    {
        PlayerCombatStateExtensions.CosmopaladinCombatState cosmo = _player.PlayerCombatState.Cosmopaladin();
        foreach (CardModel c in _player.Deck.Cards)
        {
            if (c is SpaceMercsCard card && card.HasDeterminationAbility)
            {
                Visible = true;
                return;
            }
        }

        Visible = cosmo.Determination > 0;
    }
}