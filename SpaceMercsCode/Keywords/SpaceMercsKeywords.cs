using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SpaceMercs.SpaceMercsCode.Keywords;

public static class SpaceMercsKeywords
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Exert;
    
}