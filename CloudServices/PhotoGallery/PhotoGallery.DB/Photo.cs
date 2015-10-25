using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PhotoGallery.DB
{
    public class Photo
    {
        public int PhotoId { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [StringLength(2083)]
        [DisplayName("Full-size Image")]
        public string ImageURL { get; set; }

        [StringLength(2083)]
        [DisplayName("Thumbnail")]
        public string ThumbnailURL { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PostedDate { get; set; }
    }
}

