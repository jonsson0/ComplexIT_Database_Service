using Newtonsoft.Json.Linq;

namespace DataLayer;

using Newtonsoft.Json;
using Npgsql;
using System.Collections;
using System.Text;

public class DatabaseService
{
    private readonly string _connectionString = "Host=localhost;Username=postgres;Password=12345;Database=testdb";

    // for testing:
   // FormDataTesting formDataTesting = new FormDataTesting();


   public JObject getFile(string id) 
    {
        var json = getFileFromPostgres(id);

        return json;
    }


    public string SaveFileToPostgres(JObject json)
    {

        dynamic jsonObject = JsonConvert.DeserializeObject(json.ToString());

        var fileId = jsonObject.id;
        var fileName = jsonObject.name;
        var fileRoom = jsonObject.room;
        var iv = jsonObject.iv;
        var fileData = jsonObject.file;
        var fileExtension = jsonObject.extention;
        

        Console.WriteLine("This is the individual values:");
        Console.WriteLine("id: " + fileId.ToString());
        Console.WriteLine("fileName: " + fileName.ToString());
        Console.WriteLine("fileRoom: " + fileRoom.ToString());
        Console.WriteLine("iv: " + iv.ToString());
        Console.WriteLine("fileData (first 10 chars): " + fileData.ToString().Substring(0,10));
        Console.WriteLine("fileExtension: " + fileExtension.ToString());

        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var cmd = new NpgsqlCommand();
        cmd.Connection = conn;

        cmd.CommandText = "INSERT INTO files(id, name, room, iv, data, extension) VALUES ( @id, @name, @room, @iv, @data, @extension)";
        cmd.Parameters.AddWithValue("id", fileId.ToString());
        cmd.Parameters.AddWithValue("name", fileName.ToString());
        cmd.Parameters.AddWithValue("room", fileRoom.ToString());
        cmd.Parameters.AddWithValue("iv", iv.ToString());
        cmd.Parameters.AddWithValue("data", fileData.ToString());
        cmd.Parameters.AddWithValue("extension", fileExtension.ToString());


        Console.WriteLine("sending query to database");

        cmd.ExecuteNonQuery();

        cmd.Dispose();
        // stream.Dispose();

        // also closes the connection
        conn.Dispose();
        return fileId;
    }

    public  JObject getFileFromPostgres(string fileId)
    {
        var conn = new NpgsqlConnection(_connectionString);

        JObject json = new JObject();

        conn.Open();

        var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT id, name, room, iv, data, extension FROM files WHERE id = @id";
        cmd.Parameters.AddWithValue("id", fileId);


        var reader = cmd.ExecuteReader();


        if (reader.Read())
        {
            var id = reader.GetString(0);
            var fileName = reader.GetString(1);
            var fileRoom = reader.GetString(2);
            var iv = reader.GetString(3);
           // var fileData = reader.GetFieldValue<byte[]>(4);
            var fileData = reader.GetString(4);
            var fileExtension = reader.GetString(5);

            Console.WriteLine("This is the individual values in get:");
            Console.WriteLine("id: " + id.ToString());
            Console.WriteLine("fileName: " + fileName.ToString());
            Console.WriteLine("fileRoom: " + fileRoom.ToString());
            Console.WriteLine("iv: " + iv.ToString());
            Console.WriteLine("fileData (first 10 chars): " + fileData.ToString().Substring(0, 10));
            Console.WriteLine("fileExtension: " + fileExtension.ToString());


            json.Add("id", id);
            json.Add("fileName", fileName);
            json.Add("fileRoom", fileRoom);
            json.Add("iv", iv);
            //  json.Add("fileData", Encoding.Default.GetString(fileData));
              json.Add("fileData", fileData);

            json.Add("fileExtension", fileExtension);

        }

        cmd.Dispose();
        reader.Dispose();
        conn.Dispose();

        return json;
    }

    public JObject getFilesFromRoom(string roomId)
    {
        var conn = new NpgsqlConnection(_connectionString);

        JObject json = new JObject();

        conn.Open();

        var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT id, name, room, iv, data, extension FROM files WHERE room = @room";
        cmd.Parameters.AddWithValue("room", roomId);


        var reader = cmd.ExecuteReader();

        int count = -1;

        while (reader.Read())
        {
            count++;
            var id = reader.GetString(0);
            var fileName = reader.GetString(1);
            var fileRoom = reader.GetString(2);
            var iv = reader.GetString(3);
            // var fileData = reader.GetFieldValue<byte[]>(4);
            var fileData = reader.GetString(4);
            var fileExtension = reader.GetString(5);

            /*
            Console.WriteLine("This is the individual values in get:");
            Console.WriteLine("id: " + id.ToString());
            Console.WriteLine("fileName: " + fileName.ToString());
            Console.WriteLine("fileRoom: " + fileRoom.ToString());
            Console.WriteLine("iv: " + iv.ToString());
            Console.WriteLine("fileData (first 10 chars): " + fileData.ToString().Substring(0, 10));
            Console.WriteLine("fileExtension: " + fileExtension.ToString());
            */

            JObject innerJson = new JObject();


            innerJson.Add("id", id);
            innerJson.Add("fileName", fileName);
            innerJson.Add("fileRoom", fileRoom);
            innerJson.Add("iv", iv);
            //  json.Add("fileData", Encoding.Default.GetString(fileData));
            innerJson.Add("fileData", fileData);

            innerJson.Add("fileExtension", fileExtension);

            json.Add("file "+ count, innerJson);
        }

        Console.WriteLine("This is the json:");
        Console.WriteLine(json);

        cmd.Dispose();
        reader.Dispose();
        conn.Dispose();

        return json;
    }


    public void DeletefilesFromRoom(string room)
    {

        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var cmd = new NpgsqlCommand();
        cmd.Connection = conn;

        cmd.CommandText = "DELETE FROM files WHERE room = @room";
        cmd.Parameters.AddWithValue("room", room);
        
        Console.WriteLine("sending query to database");

        cmd.ExecuteNonQuery();

        cmd.Dispose();
        // stream.Dispose();

        // also closes the connection
        conn.Dispose();
    }

    public void Deletefile(string id)
    {

        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        var cmd = new NpgsqlCommand();
        cmd.Connection = conn;


        cmd.CommandText = "DELETE FROM files WHERE id = @id";
        cmd.Parameters.AddWithValue("id", id);

        Console.WriteLine("sending query to database to delete file with id: " + id);

        cmd.ExecuteNonQuery();

        cmd.Dispose();
        // stream.Dispose();

        // also closes the connection
        conn.Dispose();
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