using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Entities
{
    public partial class Roles : DefaultBaseEntity
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
