using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Models.Roles
{
    public class RoleModel
    {
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
    }

    public class UserRoleModel {
        public int RoleId { get; set; }
        public Guid UserId { get; set; }
    }
}
