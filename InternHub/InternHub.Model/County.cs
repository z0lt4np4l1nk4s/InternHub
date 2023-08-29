using InternHub.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model
{
    public class County : BaseModel, ICounty
    {
        public string Name { get; set; }
    }
}
