﻿using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace webAPI.Models
{
    public partial class Course
    {
        public Course()
        {
            CourseInstructor = new HashSet<CourseInstructor>();
            Enrollment = new HashSet<Enrollment>();
        }

        public int CourseId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public int DepartmentId { get; set; }
        public DateTime DateModified { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<CourseInstructor> CourseInstructor { get; set; }
        public virtual ICollection<Enrollment> Enrollment { get; set; }
    }
}
