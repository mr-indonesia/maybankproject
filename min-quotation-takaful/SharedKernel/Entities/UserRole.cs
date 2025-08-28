using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Entities
{
    public partial class UserRole
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public Guid UserId { get; set; }
    }
}
