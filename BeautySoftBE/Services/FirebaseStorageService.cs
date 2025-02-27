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
        _bucketName = configuration["Firebase:StorageBucket"];
        
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("firebase_credentials/beauty-soft-e3ace-firebase-adminsdk-fbsvc-a0b2127288.json")
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