using CloudinaryDotNet.Actions;

namespace FlightBooking.Interface
{
    public interface IPhotoServices
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);        
    }
}