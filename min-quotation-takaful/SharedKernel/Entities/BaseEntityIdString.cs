using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Entities
{
    public abstract class BaseEntityIdString : BaseEntity
    {
        public new string Id {  get; set; }
    }
}
