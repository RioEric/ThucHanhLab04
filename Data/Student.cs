namespace ThucHanhLab4.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Student")]
    public partial class Student
    {
        [StringLength(10)]
        public string StudentID { get; set; }

        [Required]
        [StringLength(50)]
        public string StudentName { get; set; }

        public double AverageScore { get; set; }

        [Required]
        [StringLength(10)]
        public string FacultyID { get; set; }

        public virtual Faculty Faculty { get; set; }
    }
}
