using static IOITCore.Enums.ApiEnums;

namespace IOITCore.Entities.Bases
{
    public class BaseEntity<TId>
    {
        public virtual TId Id { get; set; }
        public virtual EntityStatus Status { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }
        public virtual int? CompanyId { get; set; }

    }

    public class BaseEntity
    {
        public virtual int Id { get; set; }
        public virtual EntityStatus Status { get; set; }
 
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }
        public virtual int? CompanyId { get; set; }

    }
}
