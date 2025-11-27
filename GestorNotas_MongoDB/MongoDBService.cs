using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;


namespace NotasApp
{
    public class MongoDBServices
    {
        private readonly IMongoCollection<Nota> _notas;

        public MongoDBServices()
        {
            var config = JObject.Parse(File.ReadAllText("appsettings.json"));

            string connectionString = config["MongoDB"]["ConnectionString"].ToString();
            string databaseName = config["MongoDB"]["DatabaseName"].ToString();
            string collectionName = config["MongoDB"]["CollectionName"].ToString();


            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _notas = database.GetCollection<Nota>(collectionName);
        }

        public async Task<List<Nota>> GetNotasAsync()
        {
            return await _notas.Find(nota => true).ToListAsync();
        }

        public async Task<Nota> GetNotaByIdAsync(string id)
        {
            return await _notas.Find(nota => nota.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateNotaAsync(Nota nota)
        {
            await _notas.InsertOneAsync(nota);
        }

        public async Task UpdateNotaAsync(string id, Nota nota)
        {
            await _notas.ReplaceOneAsync(n => n.Id == id, nota);
        }

        public async Task DeleteNotaAsync(string id)
        {
            await _notas.DeleteOneAsync(nota => nota.Id == id);
        }

        public async Task<List<Nota>> BuscarPorTituloAsync(string titulo)
        {
            var filter = Builders<Nota>.Filter.Regex(
                n => n.Titulo,
                new MongoDB.Bson.BsonRegularExpression(titulo, "i") 
            );

            return await _notas.Find(filter).ToListAsync();
        }

        public async Task<List<Nota>> BuscarPorTagAsync(string tag)
        {
            var filter = Builders<Nota>.Filter.Where(n =>
                n.Tags.Any(t =>
                    System.Text.RegularExpressions.Regex.IsMatch(t, tag, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                )
            );

            return await _notas.Find(filter).ToListAsync();
        }
    }
}