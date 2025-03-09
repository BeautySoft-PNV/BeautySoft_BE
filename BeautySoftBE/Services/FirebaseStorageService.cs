using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace BeautySoftBE.Services;

public class FirebaseStorageService
{
    private readonly string _bucketName;
    private readonly StorageClient _storageClient;

    public FirebaseStorageService(IConfiguration configuration)
    {
        _bucketName = "sinh-826e6.appspot.com";
        
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(@"C:\Users\Sinh\source\repos\BeautySoft_BE\BeautySoftBE\firebase_credentials\sinh-826e6-firebase-adminsdk-eok09-86d202a8f4.json")
            });
        }
        
        _storageClient = StorageClient.Create();
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
    {
        var objectName = $"makeup_images/{Guid.NewGuid()}_{fileName}";
        await _storageClient.UploadObjectAsync(_bucketName, objectName, "image/jpeg", imageStream);
        return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
    }
}