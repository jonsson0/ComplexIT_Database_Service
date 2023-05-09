using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using DataLayer;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ComplexIT_Database_Service.Controllers
{
    [Route("api/databaseapi")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
       // private IDatabaseService _databaseService;

        private DatabaseService test = new DatabaseService();


        public DatabaseController(/*IDatabaseService databaseService*/)
        {
         //   _databaseService = databaseService;
        }

        [HttpGet("{id}", Name = nameof(getFile))]
        public IActionResult getFile(int id)
        {

            Console.WriteLine("123");

            string fileSaveToPath = @"C:\Users\madsj\OneDrive\Skrivebord\from db";

            var formData = test.getFile(id, fileSaveToPath);

            var fileContent = formData.FirstOrDefault(c => c.Headers.ContentDisposition?.FileName != null);

            var jsonObject = new
            {
                name = formData.Headers.GetValues("fileName").First(),
                room = formData.Headers.GetValues("room").First(),
                extension = formData.Headers.GetValues("extension").First(),
                data = fileContent.ReadAsStringAsync().Result,
            };

            // Serialize the object to a JSON string
            string jsonString = JsonSerializer.Serialize(jsonObject);

            if (formData != null)
            {
                var extension = formData.Headers.GetValues("extension").First();
                Console.WriteLine("33");
                Console.WriteLine(extension);
                return Ok(jsonString);
            }

            return NotFound();
        }

        [HttpPost("upload", Name = nameof(saveFile))]
        [RequestSizeLimit(100_000_000)]
        public IActionResult saveFile()
        {
            /*
            if (str.Equals("123"))
            {
                Console.WriteLine("123");
            }s

            if (str.Equals("12345"))
            {
                Console.WriteLine("12345");
            }
            */
          

            Console.WriteLine("printing body");

            using (StreamReader stream = new StreamReader(HttpContext.Request.Body))
            {
                string body = stream.ReadToEnd();

                Console.WriteLine(body);
              //  JObject json = JObject.Parse(body);

                // file og name

              //  json.

                // body = "param=somevalue&param2=someothervalue"
                Console.WriteLine("This is the first 1000 body:");
                //Console.WriteLine(body.Substring(0, 1000));
            }




             // test.SaveFileToPostgres(body);

            return Ok("Hej Tobias");
        }
    }
}