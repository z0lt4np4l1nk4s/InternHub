using InternHub.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model
{
    public class StudyArea : BaseModel, IStudyArea
    {
       
        public string Name { get; set; }
    }
}
