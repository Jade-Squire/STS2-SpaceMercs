using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;
using SpaceMercs.SpaceMercsCode.Relics;

namespace SpaceMercs.SpaceMercsCode.Relics;

public class TestRelic() : SpaceMercsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override ActMap ModifyGeneratedMap(IRunState runState, ActMap map, int actIndex)
    {
        if (actIndex == 0)
        {
            map.StartingMapPoint.PointType = MapPointType.RestSite;
        }        
        return map;
    }
}