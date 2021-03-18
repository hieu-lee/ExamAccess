using ExamAccess.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamAccess.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RecordsController : ControllerBase
    {
        private IMongoCollection<Record> history;

        public RecordsController(IMongoClient client)
        {
            var database = client.GetDatabase("examaccess");
            history = database.GetCollection<Record>("history");
        }

        [HttpPost]
        public void Post([FromBody] Record record)
        {
            history.InsertOne(record);
        }

        [HttpGet]
        public List<Record> Get()
        {
            return history.Find(s => true).ToList();
        }
    }
}
