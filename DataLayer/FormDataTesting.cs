namespace DataLayer;

public class FormDataTesting
{

    public FormDataTesting()
    {
        
    }

    public MultipartFormDataContent genFormData()
    {
        string filePath = @"C:\Users\madsj\OneDrive\Skrivebord\intro.pdf";

        string fileSaveToPath = @"C:\Users\madsj\OneDrive\Skrivebord\from db";

        string fileExtension = Path.GetExtension(filePath);

        var fileStream = File.OpenRead(filePath);
        var streamContent = new StreamContent(fileStream);
        var formData = new MultipartFormDataContent();

        // name and fileName can be w/e. The name dont matter from what i understand, its just for identifying it. The first is field and 2nd is the file name
        formData.Add(streamContent, "file", "fileName");

        formData.Headers.Add("fileName", "ThisIsTheFileNameInHeader");
        formData.Headers.Add("room", "ThisIsTheRoomInHeader");

        //formData.Headers.Add("data", "data");
        formData.Headers.Add("extension", fileExtension);

        return formData;
    }

}