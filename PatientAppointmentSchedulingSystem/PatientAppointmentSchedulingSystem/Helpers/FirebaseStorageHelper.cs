using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace PatientAppointmentSchedulingSystem.Helpers
{
    public class FirebaseStorageHelper
    {
        private readonly StorageClient _storageClient; 
        private readonly string _bucketName = "fyp1-uwp.appspot.com";

        public FirebaseStorageHelper()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("wwwroot/firebase-adminsdk.json")
                });
            }
            _storageClient = StorageClient.Create(GoogleCredential.FromFile("wwwroot/firebase-adminsdk.json"));
        }

        //Upload image & return its public url
        public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string folderName, string contentType="image/jpeg")
        {
            //upload into "DoctorImage" folder
            var objectName = $"{folderName}/{fileName}";
            await _storageClient.UploadObjectAsync(_bucketName, objectName, contentType, fileStream);
            return GetImageUrl (fileName, folderName); 
        }

        //Delete image by filename
        public async Task DeleteImageAsync(string fileName, string folderName)
        {
            var objectName = $"{folderName}/{fileName}";
            await _storageClient.DeleteObjectAsync(_bucketName, objectName);
        }

        //generate the public download url
        public string GetImageUrl(string fileName, string folderName) 
        { 
            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(folderName + "/" + fileName)}?alt=media";
        }
    }
}
