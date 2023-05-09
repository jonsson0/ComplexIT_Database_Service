namespace DataLayer;

public interface IDatabaseService
{
    public bool saveFile();

    public MultipartFormDataContent getFile(int id);
}