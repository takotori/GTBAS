using Godot;

namespace ProjectD.addons.gas.abilities;

[Tool]
[GlobalClass]
public partial class AbilityPattern : Resource
{
    [Export] public AoePattern AoePattern { get; set; }
    [Export] public Vector2I MinMaxAoeRange { get; set; }
    [Export] public Pattern RangePattern { get; set; }
    [Export] public Vector2I MinMaxRange { get; set; }
    [Export] public CenterPoint CenterPoint { get; set; }
}

public enum Pattern
{
    Line,
    Diagonal,
    Star,
    Diamond,
    Square
}

public enum AoePattern
{
    Single,
    Line,
    HorizontalLine,
    Diamond,
    Square,
    Cone
}

public enum CenterPoint
{
    Target, // Must be centered on enemy
    Self, // Must be centered on self
    Ally, // Must be centered on ally
    Area // Can be centered anywhere   
}