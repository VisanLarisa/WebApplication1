using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class EditHelper <T, Q>
    {
        public List<T> fkList { get; set; }
        public Q editableObj { get; set; }
    }
}
