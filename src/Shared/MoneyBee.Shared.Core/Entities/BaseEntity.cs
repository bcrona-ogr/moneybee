namespace MoneyBee.Shared.Core.Entities
{
    public abstract class BaseEntity
    {
        public bool Active { get; protected set; } = true;
        public DateTime? DeletedAtUtc { get; protected set; }

        public virtual void SoftDelete(DateTime deletedAtUtc)
        {
            if (!Active)
                return;

            Active = false;
            DeletedAtUtc = deletedAtUtc;
        }
    }
}