using IOITCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITCore.Models.ViewModels
{
    public class ResUserRoles
    {
    }
    public partial class UserInfoMinDT
    {
        public long? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
    }

    public partial class UserProjectCustomDT
    {
        public int? ProjectId { get; set; }
        public long? UserId { get; set; }
        public string? FullName { get; set; }
    }
    public partial class UserRoleDT : User
    {
        public long UserId { get; set; }
        public int? UserCreateId { get; set; }
        public List<RoleDT>? listRole { get; set; }
        public List<FunctionRoleDT>? listFunction { get; set; }
    }

    public partial class UserMappingDT
    {
        public int? UserMappingId { get; set; }
        public int? UserId { get; set; }
        public int? TargetId { get; set; }
        public byte? TargetType { get; set; }
        public int? UserIdCreatedId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte? Status { get; set; }
    }

    

}
