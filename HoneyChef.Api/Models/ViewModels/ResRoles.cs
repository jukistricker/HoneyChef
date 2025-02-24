using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOITCore.Models.ViewModels
{
    public class ResRoles
    {
    }
    public partial class RoleDT
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }

    public partial class RoleDTO
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }
        public int? UserEditId { get; set; }
        public int? Status { get; set; }
        public byte? LevelRole { get; set; }
        public List<FunctionRoleDT>? listFunction { get; set; }
    }

    public partial class FunctionRoleDT
    {
        public int Id { get; set; }
        public int TargetId { get; set; }
        public int FunctionId { get; set; }
        public string? ActiveKey { get; set; }
        public int? Status { get; set; }
    }


}
