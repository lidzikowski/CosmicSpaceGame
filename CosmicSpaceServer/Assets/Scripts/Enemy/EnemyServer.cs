using UnityEngine;

public class EnemyServer : Opponent
{
    public override ulong Id => throw new System.NotImplementedException();

    public override Vector2 Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public override ulong Hitpoints { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public override ulong MaxHitpoints => throw new System.NotImplementedException();

    public override ulong Shields { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public override ulong MaxShields => throw new System.NotImplementedException();

    public override int Speed => throw new System.NotImplementedException();

    public override ulong Damage => throw new System.NotImplementedException();

    public override string Name => throw new System.NotImplementedException();
}