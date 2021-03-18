using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using ExamAccess.Shared;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExamAccess.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IMongoDatabase examdb;
        private IMongoCollection<Account> accounts;

        private string Encrypt(string textToEncrypt)
        {
            try
            {
                string ToReturn = "";
                string publickey = "phamleha";
                string secretkey = "hiulebeo";
                byte[] secretkeyByte = { };
                secretkeyByte = Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private string Decrypt(string textToDecrypt)
        {
            try
            {
                string ToReturn = "";
                string publickey = "phamleha";
                string privatekey = "hiulebeo";
                byte[] privatekeyByte = { };
                privatekeyByte = Encoding.UTF8.GetBytes(privatekey);
                byte[] publickeybyte = { };
                publickeybyte = Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }

        public AccountsController(IMongoClient client)
        {
            examdb = client.GetDatabase("examaccess");
            accounts = examdb.GetCollection<Account>("accounts");
        }

        [HttpPost("result/{id}")]
        public void PostNewExamResult([FromBody] ExamResult result, string id)
        {
            var myacc = examdb.GetCollection<ExamResult>(id);
            myacc.InsertOne(result);
        }

        [HttpGet("{id}")]
        public List<ExamResult> GetPersonalRecord(string id)
        {
            var myacc = examdb.GetCollection<ExamResult>(id);
            return myacc.Find(s => true).ToList();
        }

        [HttpGet]
        public List<Account> Get()
        {
            var AccGen = accounts.Find(s => true).ToEnumerable();
            List<Account> res = new();
            foreach (var acc in AccGen)
            {
                acc.password = Decrypt(acc.password);
                res.Add(acc);
            }
            return res;
        }

        [HttpPost("admin")]
        public void PostAdmin([FromBody] Account acc)
        {
            acc.password = Encrypt(acc.password);
            accounts.InsertOne(acc);
        }

        [HttpPost("user")]
        public async Task PostUser([FromBody] Account acc)
        {
            acc.password = Encrypt(acc.password);
            var task = examdb.CreateCollectionAsync(acc.username);
            accounts.InsertOne(acc);
            await task;
        }
    }
}
