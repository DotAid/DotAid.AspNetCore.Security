namespace DotAid.AspNetCore.Security;

public class HashedId<T>
{
    public static implicit operator HashedId<T>(T id) => new(id);

    public static implicit operator T(HashedId<T> hashedId) => hashedId.Id;

    public HashedId(T id)
    {
        Id = id;
    }

    public T Id { get; }
}