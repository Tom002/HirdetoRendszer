using System;
using System.Collections.Generic;
using System.Text;

namespace HirdetoRendszer.Dal.Interfaces
{
    public interface ISoftDelete
    {
        public bool SoftDeleted { get; set; }
    }
}
