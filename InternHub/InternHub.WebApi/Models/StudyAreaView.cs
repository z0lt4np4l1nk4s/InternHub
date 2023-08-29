using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternHub.Model
{
    public class StudyAreaView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public StudyAreaView() { }

        public StudyAreaView(StudyArea studyArea)
        {
            Id = studyArea.Id;
            Name = studyArea.Name;
        }
    }
}
