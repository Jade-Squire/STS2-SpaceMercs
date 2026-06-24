using BaseLib.Abstracts;
using SpaceMercs.SpaceMercsCode.Extensions;
using Godot;

namespace SpaceMercs.SpaceMercsCode.Character;

public class SpaceMercsRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => SpaceMercs.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}