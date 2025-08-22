using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectD.addons.gas.attributes;
using ProjectD.scripts.units;

namespace ProjectD.addons.gas.abilities;

[GlobalClass]
public partial class SubAbility : Ability
{
    protected SubAbilityData subAbilityData;
    
    public void Init(SubAbilityData subAbilityData, AbilitySystem casterAbilitySystem)
    {
        this.subAbilityData = subAbilityData;
        this.casterAbilitySystem = casterAbilitySystem;
    }

    protected List<AttributeSet> GetTargetsInRange(SubAbilityData ability, Vector2I targetTile)
    {
        var opponentUnits = GetUnitOfTeam(
            GetOwnerActor().Team == Team.Player ? Team.Enemy : Team.Player
        );

        var abilityAoeReachablePositions = PatternCalculator.GetAoeAbilityRange(
            GetOwnerActor().currentTileIndex,
            targetTile,
            ability.pattern.AoePattern,
            ability.pattern.MinMaxAoeRange
        );

        var abilityReachableTiles = navigation
            .GetAbilityReachableTiles(abilityAoeReachablePositions)
            .Select(a => a.GetTileIndex())
            .ToList();

        return opponentUnits
            .Where(u => abilityReachableTiles.Contains(u.currentTileIndex))
            .Select(u => u.abilitySystem.attributeSet)
            .ToList();
    }
    
    public virtual bool CanActivateSubAbility()
    {
        return false;
    }
}