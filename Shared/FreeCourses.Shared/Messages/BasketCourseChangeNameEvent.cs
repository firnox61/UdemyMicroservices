using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourses.Shared.Messages
{
    public class BasketCourseChangeNameEvent
    {
        public string UserId { get; set; }
        public string CourseId { get; set; }
        public string UpdateName { get; set; }
    }
}
