using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourses.Shared.Messages
{
    public class CourseNameChangeEvent
    {
        public string CourseId { get; set; }
        public string UpdateName { get; set; }
    }
}
