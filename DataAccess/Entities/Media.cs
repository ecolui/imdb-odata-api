using System;

using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    public class Media
    {
        public int MediaId { get; set; }
        public string MediaIMDBId { get; set; }

        public string titleType { get; set; }
        public string primaryTitle { get; set; }
        public string originalTitle { get; set; }
        public bool? isAdult { get; set; }
        public int? startYear { get; set; }
        public int? endYear { get; set; }
        public int? runtimeMinutes { get; set; }

        //navigational properties
        public ICollection<Genre> Genres { get; set; }
        public ICollection<Staff> Staff { get; set; }
        public MediaRating Rating { get; set; }
    }

    //one-to-many relationship between Media and Genre
    public class Genre
    {
        public int GenreId { get; set; }
        public string Description { get; set; }

        [ForeignKey("MediaIMDBId")]
        public string MediaIMDBId { get; set; }
        public Media AssociatedMedia { get; set; }
    }

    //one-to-one relationship
    public class MediaRating
    {
        public int MediaRatingId { get; set; }
        public decimal AverageRating { get; set; }
        public Int64 NumVotes { get; set; }

        [ForeignKey("MediaIMDBId")]
        public string MediaIMDBId { get; set; }
        public Media AssociatedMedia { get; set; }

    }

}
