using System.Data.Entity;

namespace PhotoGallery.DB
{
    public class PhotoGalleryContext : DbContext
    {
        public PhotoGalleryContext() : base("name=PhotoGalleryContext")
        {
        }

        public PhotoGalleryContext(string connString) : base(connString)
        {
        }

        public System.Data.Entity.DbSet<Photo> Photos { get; set; }
    
    }
}
