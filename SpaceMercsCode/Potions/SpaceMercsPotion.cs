using BaseLib.Abstracts;
using BaseLib.Utils;
using SpaceMercs.SpaceMercsCode.Character;

namespace SpaceMercs.SpaceMercsCode.Potions;

[Pool(typeof(SpaceMercsPotionPool))]
public abstract class SpaceMercsPotion : CustomPotionModel;