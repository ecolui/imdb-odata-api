using System;

using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    public class Staff
    {
        public int StaffId { get; set; }
        public string imdbStaffId { get; set; }
        public string PrimaryName { get; set; }
        public int? BirthYear { get; set; }
        public int? DeathYear { get; set; }

        //navigaitional properties
        public ICollection<Profession> primaryProfession {get; set;}   
        public ICollection<StaffMediaLink> KnownForTitles { get; set; }
    }

    //one-to-many relationship
    public class Profession
    {
        public int ProfessionId { get; set; }
        public string Description { get; set; }

        [ForeignKey("imdbStaffId")]
        public string imdbStaffId { get; set; }
        public Staff AssociatedStaff { get; set; }

    }

    //Many-to-Many Relationship
    public class StaffMediaLink
    {
        public int StaffMediaLinkId { get; set; }

        [ForeignKey("imdbStaffId")]
        public string imdbStaffId { get; set; }
        public Staff AssociatedStaff { get; set; }

        [ForeignKey("MediaIMDBId")]
        public string MediaIMDBId { get; set; }
        public Media AssociatedMedia { get; set; }
    }

    public class Principal
    {

        public int PrincipalId { get; set; }

        [ForeignKey("MediaIMDBId")]
        public string MediaIMDBId { get; set; }
        public Media AssociatedMedia { get; set; }       

        public int? Ordering { get; set; }

        [ForeignKey("imdbStaffId")]
        public string imdbStaffId { get; set; }
        public Staff Staff { get; set; }

        public string Category { get; set; }

        public string Job { get; set; }

        //TODO: Consider parsing this out into a one-to-many relationship
        public string Characters { get; set; }

    }

}
