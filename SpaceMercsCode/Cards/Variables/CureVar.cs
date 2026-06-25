using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SpaceMercs.SpaceMercsCode.Cards.Variables;

public class CureVar : DynamicVar
{
    public const string Key = "CurePower";

    public CureVar(decimal cureAmt) : base(Key, cureAmt)
    {
        this.WithTooltip();
    }
}