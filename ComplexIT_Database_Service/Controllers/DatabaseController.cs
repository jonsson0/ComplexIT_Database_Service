using Microsoft.AspNetCore.Mvc;
using DataLayer;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web;

namespace ComplexIT_Database_Service.Controllers
{
    [Route("api/databaseapi")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
       // private IDatabaseService _databaseService;

        private DatabaseService databaseService = new DatabaseService();


        public DatabaseController(/*IDatabaseService databaseService*/)
        {
         //   _databaseService = databaseService;
        }

        [HttpGet("{id}", Name = nameof(getFile))]
        [RequestSizeLimit(100_000_000)]
        public IActionResult getFile(string id)
        {

            //  string fileSaveToPath = @"C:\Users\madsj\OneDrive\Skrivebord\from db";


            Console.WriteLine("GET REQUEST");
            Console.WriteLine("This is the string id that is being requested:");
            Console.WriteLine(id);

            var jObject = databaseService.getFile(id);


            if (jObject.Count > 0)
            {
                Console.WriteLine("Returning ok to the get request with the jObject as body");
                Console.WriteLine("-----------------------------------");
                return Ok(jObject.ToString());
            }
            else
            {
                Console.WriteLine("Returning 404 not found to the get request");
                Console.WriteLine("-----------------------------------");
                return NotFound();
            }
        }

        [HttpGet("allfiles/{roomId}", Name = nameof(getFilesFromRoom))]
        [RequestSizeLimit(100_000_000)]
        public IActionResult getFilesFromRoom(string roomId)
        {

            //  string fileSaveToPath = @"C:\Users\madsj\OneDrive\Skrivebord\from db";


            Console.WriteLine("GET REQUEST");
            Console.WriteLine("This is the string roomId, where all the files are being requested from:");
            Console.WriteLine(roomId);

            var jObject = databaseService.getFilesFromRoom(roomId);


            if (jObject.Count > 0)
            {
                Console.WriteLine("Returning ok to the get request with the jObject as body");
                Console.WriteLine("-----------------------------------");
                return Ok(jObject.ToString());
            }
            else
            {
                Console.WriteLine("Returning 404 not found to the get request");
                Console.WriteLine("-----------------------------------");
                return NotFound();
            }
        }


        [HttpPost("upload", Name = nameof(saveFile))]
        [RequestSizeLimit(100_000_000)]
        public IActionResult saveFile()
        {

            Console.WriteLine("POST UPLOAD REQUEST");


            Stream inputStream = Request.Body;

            /*
             var formData = Request.Form;

             foreach (var VARIABLE in formData)
             {
                 Console.WriteLine(VARIABLE);
             }
            */

            JObject jsonObject = new JObject();

            using (StreamReader stream = new StreamReader(HttpContext.Request.Body))
            {
                string body = stream.ReadToEnd();


               // Console.WriteLine(body);

               // Console.WriteLine("This is the body after converting to JObject:");
                // Parse the JSON string into a JsonObject
                jsonObject = JObject.Parse(body);
              //  Console.WriteLine(jsonObject);
            }


            var id = databaseService.SaveFileToPostgres(jsonObject);
            // var id = databaseService.SaveFileToPostgres(body);

            Console.WriteLine("Returning ok with the id as body");
            Console.WriteLine("-----------------------------------");

            return Ok(id);
        }

        [HttpDelete("files/room/{room}" , Name = nameof(deleteFilesFromRoom))]
        [RequestSizeLimit(100_000_000)]
        public IActionResult deleteFilesFromRoom(string room)
        {
            Console.WriteLine("deleting all files from room:");
            Console.WriteLine(room);
            databaseService.DeletefilesFromRoom(room);

            Console.WriteLine("Returning with OK");
            return Ok("All files from the following room deleted: " + room);
        }
        
        [HttpDelete("delete/file/{id}", Name = nameof(deleteFile))]
        [RequestSizeLimit(100_000_000)]
        public IActionResult deleteFile(string id)
        {
            Console.WriteLine("deleting file with id:");
            Console.WriteLine(id);
            databaseService.Deletefile(id);

            Console.WriteLine("Returning with OK");
            return Ok("File deleted");
        }
        
    }
}