namespace DataLayer;
using Npgsql;
using System.Collections;

public class DatabaseService
{
    private readonly string _connectionString = "Host=localhost;Username=postgres;Password=12345;Database=complexit_files";

    // for testing:
    FormDataTesting formDataTesting = new FormDataTesting();

    public async Task saveFile()
    {
        bool isSaved = false;

        MultipartFormDataContent formData;

        formData = formDataTesting.genFormData();

        await SaveFileToPostgres(formData);

      //  return isSaved;
    }


    public MultipartFormDataContent getFile(int id, string savePath) 
    {
        var formData = getFileFromPostgres(id, savePath);

        Console.WriteLine("123");

        return formData;
    }


    public async Task SaveFileToPostgres(MultipartFormDataContent formData)
    {

        var fileName = formData.Headers.GetValues("fileName").First();
        var room = formData.Headers.GetValues("room").First();

        // var data = formData.Headers.GetValues("data").First();

        // var bytes = formData.ReadAsStringAsync();



        //  Console.WriteLine("here");
        //  Console.WriteLine(bytes.Result);

        var extension = formData.Headers.GetValues("extension").First();

        //  byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);

        // TO CONVERT TO STRING AGAIN
        // string result = System.Text.Encoding.UTF8.GetString(byteArray);


        var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand();
        cmd.Connection = conn;


        cmd.CommandText = "INSERT INTO files(name, room, data, extension) VALUES (@name, @room, @data, @extension)";
        cmd.Parameters.AddWithValue("name", fileName);
        cmd.Parameters.AddWithValue("room", room);

        var fileContent = formData.FirstOrDefault(c => c.Headers.ContentDisposition?.FileName != null);
        byte[] bytes;

        if (fileContent != null)
        {
            // file data as a byte array
            bytes = await fileContent.ReadAsByteArrayAsync();
            cmd.Parameters.AddWithValue("data", bytes);
        }
        //  cmd.Parameters.AddWithValue("data", new byte[0]);
        cmd.Parameters.AddWithValue("extension", extension);


        /*
        var stream = await formData.ReadAsStreamAsync();
        byte[] data = new byte[stream.Length];
        await stream.ReadAsync(data, 0, (int)stream.Length);

        cmd.Parameters.AddWithValue("data", NpgsqlDbType.Bytea, data);
        */
        await cmd.ExecuteNonQueryAsync();

        cmd.Dispose();
        // stream.Dispose();

        // also closes the connection
        conn.Dispose();
    }

    public  MultipartFormDataContent getFileFromPostgres(int fileId, string savePath)
    {
        var conn = new NpgsqlConnection(_connectionString);

        var formData = new MultipartFormDataContent();

        conn.Open();

        var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT name, room, data, extension FROM files WHERE id = @id";
        cmd.Parameters.AddWithValue("id", fileId);


        var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            var fileName = reader.GetString(0);
            var fileRoom = reader.GetString(1);
            var fileData = reader.GetFieldValue<byte[]>(2);
            var fileExtension = reader.GetString(3);
            var filePath = Path.Combine(savePath, fileName + fileExtension);

            Console.WriteLine("HERE");
            Console.WriteLine(fileRoom);

            var numberOfBytes = fileData.Length;

            MemoryStream memoryStream = new MemoryStream(fileData);
            StreamContent streamContent = new StreamContent(memoryStream);

            formData.Add(streamContent, "file", "fileName");

            formData.Headers.Add("fileName", fileName);
            formData.Headers.Add("room", fileRoom);

            //formData.Headers.Add("data", "data");
            formData.Headers.Add("extension", fileExtension);

            formData.Headers.Add("numberofbytes", numberOfBytes.ToString());

            /*
            var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            fileStream.Write(fileData, 0, fileData.Length);
            fileStream.Close();
            */
        }

        cmd.Dispose();
        reader.Dispose();
        conn.Dispose();

        return formData;

    }


    public async Task SaveFileFromDb(int fileId, string savePath)
    {

        var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();


        var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT name, data, extension FROM files WHERE id = @id";
        cmd.Parameters.AddWithValue("id", fileId);

        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var fileName = reader.GetString(0);
            var fileData = reader.GetFieldValue<byte[]>(1);
            var fileExtension = reader.GetString(2);
            var filePath = Path.Combine(savePath, fileName + fileExtension);

            var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            fileStream.Write(fileData, 0, fileData.Length);
            fileStream.Close();
        }

        cmd.Dispose();
        reader.Dispose();
        conn.Dispose();
    }

}