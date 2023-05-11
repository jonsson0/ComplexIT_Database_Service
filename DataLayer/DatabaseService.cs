using Newtonsoft.Json.Linq;

namespace DataLayer;

using Newtonsoft.Json;
using Npgsql;
using System.Collections;
using System.Text;

public class DatabaseService
{
    private readonly string _connectionString = "Host=localhost;Username=postgres;Password=12345;Database=complexit_files";

    // for testing:
   // FormDataTesting formDataTesting = new FormDataTesting();


   public JObject getFile(int id) 
    {
        var json = getFileFromPostgres(id);

        Console.WriteLine("123");

        return json;
    }


    public JObject SaveFileToPostgres(JObject json)
    {
      
        dynamic jsonObject = JsonConvert.DeserializeObject(json.ToString());


        var fileName = jsonObject.fileName;
        var fileRoom = jsonObject.fileRoom;
        var iv = jsonObject.iv;
        var fileData = jsonObject.fileData;
        var fileExtension = jsonObject.fileExtension;

        Console.WriteLine("type:");
        Console.WriteLine(fileData.ToString().GetType());

       // byte[] dataBytes =  Encoding.Default.GetBytes(fileData.ToString());

        Console.WriteLine("INSIDE INSIDE");
        Console.WriteLine(fileName);
        Console.WriteLine(fileRoom);
        Console.WriteLine(iv);
        Console.WriteLine(fileData.ToString());
        Console.WriteLine(fileExtension);

        Console.WriteLine("---");

      //  Console.WriteLine(dataBytes);

        Console.WriteLine("---");

     //   Console.WriteLine(dataBytes.ToString());


        //  var room = formData.Headers.GetValues("room").First();

        // var data = formData.Headers.GetValues("data").First();

        // var bytes = formData.ReadAsStringAsync();


        //  Console.WriteLine("here");
        //  Console.WriteLine(bytes.Result);

        //  var extension = formData.Headers.GetValues("extension").First();

        //  byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);

        // TO CONVERT TO STRING AGAIN
        // string result = System.Text.Encoding.UTF8.GetString(byteArray);


        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var cmd = new NpgsqlCommand();
        cmd.Connection = conn;


        cmd.CommandText = "INSERT INTO files(name, room, iv, data, extension) VALUES (@name, @room, @iv, @data, @extension)";
        cmd.Parameters.AddWithValue("name", fileName.ToString());
        cmd.Parameters.AddWithValue("room", fileRoom.ToString());
        cmd.Parameters.AddWithValue("iv", iv.ToString());
        cmd.Parameters.AddWithValue("data", fileData.ToString());
        cmd.Parameters.AddWithValue("extension", fileExtension.ToString());

        //  var fileContent = formData.FirstOrDefault(c => c.Headers.ContentDisposition?.FileName != null);


        /*
        var stream = await formData.ReadAsStreamAsync();
        byte[] data = new byte[stream.Length];
        await stream.ReadAsync(data, 0, (int)stream.Length);

        cmd.Parameters.AddWithValue("data", NpgsqlDbType.Bytea, data);
        */

        Console.WriteLine("sending");

        cmd.ExecuteNonQueryAsync();

        cmd.Dispose();
        // stream.Dispose();

        // also closes the connection
        conn.Dispose();
        return new JObject();
    }

    public  JObject getFileFromPostgres(int fileId)
    {
        var conn = new NpgsqlConnection(_connectionString);

      //  var formData = new MultipartFormDataContent();

        JObject json = new JObject();

        conn.Open();

        var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT id, name, room, iv, data, extension FROM files WHERE id = @id";
        cmd.Parameters.AddWithValue("id", fileId);


        var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            var id = reader.GetInt32(0);

            var fileName = reader.GetString(1);
            var fileRoom = reader.GetString(2);
            var iv = reader.GetString(3);
            var fileData = reader.GetFieldValue<byte[]>(4);
            var fileExtension = reader.GetString(5);

            // var filePath = Path.Combine(savePath, fileName + fileExtension);

            Console.WriteLine("HERE");
            Console.WriteLine(fileRoom);


            json.Add("id", id);
            json.Add("fileName", fileName);
            json.Add("fileRoom", fileRoom);
            json.Add("iv", iv);
            json.Add("fileData", Encoding.Default.GetString(fileData));
            json.Add("fileExtension", fileExtension);

            //  var numberOfBytes = fileData.Length;

            // MemoryStream memoryStream = new MemoryStream(fileData);
            //  StreamContent streamContent = new StreamContent(memoryStream);

            // formData.Add(streamContent, "file", "fileName");

            // formData.Headers.Add("fileName", fileName);
            // formData.Headers.Add("room", fileRoom);

            //formData.Headers.Add("data", "data");
            // formData.Headers.Add("extension", fileExtension);

            //  formData.Headers.Add("numberofbytes", numberOfBytes.ToString());

            /*
            var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            fileStream.Write(fileData, 0, fileData.Length);
            fileStream.Close();
            */
        }

        cmd.Dispose();
        reader.Dispose();
        conn.Dispose();

        Console.WriteLine("172 172 172");
        Console.WriteLine(json);

        return json;
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