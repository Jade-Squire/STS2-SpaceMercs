using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;

namespace SpaceMercs.SpaceMercsCode.Relics;

public class TestRelic() : SpaceMercsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override ActMap ModifyGeneratedMap(IRunState runState, ActMap map, int actIndex)
    {
        if (actIndex == 0)
        {
            map.StartingMapPoint.PointType = MapPointType.Shop;
        }        
        return map;
    }
}