using System.Threading.Tasks;
using System.Web;

namespace MyFixIt.Persistence
{
    public interface IPhotoService
    {
        void CreateAndConfigureAsync();
        Task<string> UploadPhotoAsync(HttpPostedFileBase photoToUpload);
    }
}