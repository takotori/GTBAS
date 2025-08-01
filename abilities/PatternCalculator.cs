using System;
using System.Collections.Generic;
using Godot;

namespace ProjectD.addons.gas.abilities;

public static class PatternCalculator
{
    public static HashSet<Vector2I> GetAbilityRange(
        Vector2I Target,
        Pattern Pattern,
        Vector2I MinMaxRange
    )
    {
        var TilesInRange = Pattern switch
        {
            Pattern.Line => GetLinePattern(MinMaxRange),
            Pattern.Diagonal => GetDiagonalPattern(MinMaxRange),
            Pattern.Star => GetStarPattern(MinMaxRange),
            Pattern.Diamond => GetDiamondPattern(MinMaxRange),
            Pattern.Square => GetSquarePattern(MinMaxRange),
            _ => throw new ArgumentOutOfRangeException(nameof(Pattern), Pattern, null),
        };

        return OffsetIndexArray(TilesInRange, Target);
    }

    public static HashSet<Vector2I> GetAoeAbilityRange(
        Vector2I Origin,
        Vector2I Target,
        AoePattern Pattern,
        Vector2I MinMaxRange
    )
    {
        var Direction = FindRelativeDirection(Origin, Target);
        var TilesInRange = Pattern switch
        {
            AoePattern.Single => [new Vector2I(0, 0)],
            AoePattern.Line => GetAoeLinePattern(MinMaxRange, Direction),
            AoePattern.HorizontalLine => GetAoeHorizontalLinePattern(MinMaxRange, Direction),
            AoePattern.Diamond => GetDiamondPattern(MinMaxRange),
            AoePattern.Square => GetSquarePattern(MinMaxRange),
            AoePattern.Cone => GetAoeConePattern(MinMaxRange, Direction),
            _ => throw new ArgumentOutOfRangeException(nameof(Pattern), Pattern, null),
        };

        return OffsetIndexArray(TilesInRange, Target);
    }

    private static HashSet<Vector2I> GetAoeLinePattern(
        Vector2I MinMaxRange,
        RelativeDirection Direction
    )
    {
        HashSet<Vector2I> TilesInRange = [];
        for (var i = MinMaxRange.X; i < MinMaxRange.Y; i++)
            switch (Direction)
            {
                case RelativeDirection.Up:
                    TilesInRange.Add(new Vector2I(0, i));
                    break;
                case RelativeDirection.Down:
                    TilesInRange.Add(new Vector2I(0, -i));
                    break;
                case RelativeDirection.Left:
                    TilesInRange.Add(new Vector2I(-i, 0));
                    break;
                case RelativeDirection.Right:
                    TilesInRange.Add(new Vector2I(i, 0));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Direction), Direction, null);
            }

        return TilesInRange;
    }

    private static HashSet<Vector2I> GetAoeHorizontalLinePattern(
        Vector2I MinMaxRange,
        RelativeDirection Direction
    )
    {
        HashSet<Vector2I> TilesInRange = [];
        for (var i = MinMaxRange.X; i < MinMaxRange.Y; i++)
            switch (Direction)
            {
                case RelativeDirection.Up:
                    TilesInRange.Add(new Vector2I(0, i));
                    TilesInRange.Add(new Vector2I(1, i));
                    TilesInRange.Add(new Vector2I(-1, i));
                    break;
                case RelativeDirection.Down:
                    TilesInRange.Add(new Vector2I(0, -i));
                    TilesInRange.Add(new Vector2I(1, -i));
                    TilesInRange.Add(new Vector2I(-1, -i));
                    break;
                case RelativeDirection.Left:
                    TilesInRange.Add(new Vector2I(-i, 0));
                    TilesInRange.Add(new Vector2I(-i, 1));
                    TilesInRange.Add(new Vector2I(-i, -1));
                    break;
                case RelativeDirection.Right:
                    TilesInRange.Add(new Vector2I(i, 0));
                    TilesInRange.Add(new Vector2I(i, 1));
                    TilesInRange.Add(new Vector2I(i, -1));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Direction), Direction, null);
            }

        return TilesInRange;
    }

    private static HashSet<Vector2I> GetAoeConePattern(
        Vector2I MinMaxRange,
        RelativeDirection Direction
    )
    {
        HashSet<Vector2I> TilesInRange = [];
        var Width = 1;

        for (var i = MinMaxRange.X; i < MinMaxRange.Y; i++)
        {
            switch (Direction)
            {
                case RelativeDirection.Up:

                    for (var j = -Width; j <= Width; j++)
                        TilesInRange.Add(new Vector2I(j, i));

                    break;
                case RelativeDirection.Down:
                    for (var j = -Width; j <= Width; j++)
                        TilesInRange.Add(new Vector2I(j, -i));

                    break;
                case RelativeDirection.Left:
                    for (var j = -Width; j <= Width; j++)
                        TilesInRange.Add(new Vector2I(-i, j));

                    break;
                case RelativeDirection.Right:
                    for (var j = -Width; j <= Width; j++)
                        TilesInRange.Add(new Vector2I(i, j));

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Direction), Direction, null);
            }

            Width++;
        }

        return TilesInRange;
    }

    private static HashSet<Vector2I> GetLinePattern(Vector2I MinMaxRange)
    {
        HashSet<Vector2I> TilesInRange = [];
        for (var i = MinMaxRange.X; i <= MinMaxRange.Y; i++)
        {
            TilesInRange.Add(new Vector2I(0, i));
            TilesInRange.Add(new Vector2I(0, -i));
            TilesInRange.Add(new Vector2I(i, 0));
            TilesInRange.Add(new Vector2I(-i, 0));
        }

        return TilesInRange;
    }

    private static HashSet<Vector2I> GetDiagonalPattern(Vector2I MinMaxRange)
    {
        HashSet<Vector2I> TilesInRange = [];
        for (var i = MinMaxRange.X; i <= MinMaxRange.Y; i++)
        {
            TilesInRange.Add(new Vector2I(i, i));
            TilesInRange.Add(new Vector2I(-i, -i));
            TilesInRange.Add(new Vector2I(i, -i));
            TilesInRange.Add(new Vector2I(-i, i));
        }

        return TilesInRange;
    }

    private static HashSet<Vector2I> GetStarPattern(Vector2I MinMaxRange)
    {
        var Pattern = GetLinePattern(MinMaxRange);
        Pattern.UnionWith(GetDiagonalPattern(MinMaxRange));
        return Pattern;
    }

    private static HashSet<Vector2I> GetDiamondPattern(Vector2I MinMaxRange)
    {
        HashSet<Vector2I> TilesInRange = [];
        for (var i = MinMaxRange.X; i <= MinMaxRange.Y; i++)
        for (var j = 0; j <= i; j++)
        {
            TilesInRange.Add(new Vector2I(j, -(i - j)));
            TilesInRange.Add(new Vector2I(i - j, j));
            TilesInRange.Add(new Vector2I(-j, i - j));
            TilesInRange.Add(new Vector2I(-(i - j), -j));
        }

        return TilesInRange;
    }

    private static HashSet<Vector2I> GetSquarePattern(Vector2I MinMaxRange)
    {
        HashSet<Vector2I> TilesInRange = [];
        for (var i = MinMaxRange.X; i <= MinMaxRange.Y; i++)
        for (var j = -i; j <= i; j++)
        {
            TilesInRange.Add(new Vector2I(j, -i));
            TilesInRange.Add(new Vector2I(i, j));
            TilesInRange.Add(new Vector2I(-j, i));
            TilesInRange.Add(new Vector2I(-i, -j));
        }

        return TilesInRange;
    }

    private static HashSet<Vector2I> OffsetIndexArray(
        HashSet<Vector2I> TilesInRange,
        Vector2I Offset
    )
    {
        HashSet<Vector2I> OffsetTiles = [];
        foreach (var TileInRange in TilesInRange)
            OffsetTiles.Add(new Vector2I(TileInRange.X + Offset.X, TileInRange.Y + Offset.Y));

        return OffsetTiles;
    }

    private static RelativeDirection FindRelativeDirection(Vector2 Origin, Vector2 Target)
    {
        var Direction = new Vector2(Target.X - Origin.X, Target.Y - Origin.Y);
        if (Mathf.Abs(Direction.X) > Mathf.Abs(Direction.Y))
            return Direction.X > 0 ? RelativeDirection.Right : RelativeDirection.Left;

        return Direction.Y > 0 ? RelativeDirection.Up : RelativeDirection.Down;
    }
}

public enum RelativeDirection
{
    Up,
    Down,
    Left,
    Right,
}
