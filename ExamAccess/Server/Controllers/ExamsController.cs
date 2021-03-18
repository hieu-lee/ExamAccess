using ExamAccess.Shared;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExamAccess.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private IMongoDatabase examdb;
        private IMongoCollection<Exam> exams;

        public ExamsController(IMongoClient client)
        {
            examdb = client.GetDatabase("examlist");
            exams = examdb.GetCollection<Exam>("exams");
        }

        [HttpGet]
        public IEnumerable<Exam> Get()
        {
            return exams.Find(s => true).ToEnumerable();
        }

        [HttpGet("{id}")]
        public async Task<Tuple<List<QuestionInfo>, int, bool, string>> GetQuestionsAndExamInfo(string id)
        {
            var task = exams.Find(s => s.code == id).FirstOrDefaultAsync();
            var myExamCollection = examdb.GetCollection<QuestionInfo>(id);
            var questions = myExamCollection.Find(s => true).ToList();
            var exam = await task;
            return new Tuple<List<QuestionInfo>, int, bool, string>(questions, exam.time, exam.privacy, exam.label);
            
        }

        [HttpPost]
        public async Task PostNewExam([FromBody] Exam exam)
        {
            var task = examdb.CreateCollectionAsync(exam.code);
            exams.InsertOne(exam);
            await task;
        }

        [HttpPost("{id}")]
        public void PostNewQuestion([FromBody] QuestionInfo question, string id)
        {
            var myexam = examdb.GetCollection<QuestionInfo>(id);
            myexam.InsertOne(question);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            var filter = Builders<Exam>.Filter.Eq("_id", id);
            var task = examdb.DropCollectionAsync(id);
            exams.DeleteOne(filter);
            await task;
        }

        [HttpDelete("{id}/{qnum:int}")]
        public void PopQuestion(string id, int qnum)
        {
            var myexam = examdb.GetCollection<QuestionInfo>(id);
            var filter = Builders<QuestionInfo>.Filter.Eq("_id", qnum);
            myexam.DeleteOne(filter);
        }

        [HttpPut("{id}/time")]
        public void PutTime([FromBody] int time, string id)
        {
            var filter = Builders<Exam>.Filter.Eq("_id", id);
            var update = Builders<Exam>.Update.Set("time", time);
            exams.UpdateOne(filter, update);
        }

        [HttpPut("{id}/privacy")]
        public void PutPrivacy([FromBody] bool privacy, string id)
        {
            var filter = Builders<Exam>.Filter.Eq("_id", id);
            var update = Builders<Exam>.Update.Set("privacy", privacy);
            exams.UpdateOne(filter, update);
        }

        [HttpPut("{id}/label")]
        public void PutLabel([FromBody] string label, string id)
        {
            var filter = Builders<Exam>.Filter.Eq("_id", id);
            var update = Builders<Exam>.Update.Set("label", label);
            exams.UpdateOne(filter, update);
        }
    }
}
