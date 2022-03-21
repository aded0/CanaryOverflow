namespace CanaryOverflow.Common;

public abstract class Entity<TKey> : IEquatable<Entity<TKey>>
{
    protected Entity()
    {
    }

    protected Entity(TKey id)
    {
        Id = id;
    }

    public TKey? Id { get; protected set; }

    public bool Equals(Entity<TKey>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<TKey?>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Entity<TKey>) obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<TKey?>.Default.GetHashCode(Id);
    }

    public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right)
    {
        return !Equals(left, right);
    }
}
