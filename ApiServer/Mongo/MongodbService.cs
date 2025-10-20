using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServer.Mongo;

public class MongodbService {

    private readonly IMongoCollection<Playlist> _playlistCollection;

    public MongodbService(IOptions<MongoDBSettings> mongoDBSettings) {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _playlistCollection = database.GetCollection<Playlist>(mongoDBSettings.Value.CollectionName);
    }

    public async Task<List<Playlist>> GetAsync() =>
        await _playlistCollection.Find(_ => true).ToListAsync();

    public async Task<Playlist?> GetByIdAsync(string id) =>
        await _playlistCollection.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Playlist playlist) =>
        await _playlistCollection.InsertOneAsync(playlist);

    public async Task UpdateAsync(string id, Playlist updated) =>
        await _playlistCollection.ReplaceOneAsync(p => p.Id == id, updated);

    public async Task AddToPlaylistAsync(string id, string movieId) {
        var filter = Builders<Playlist>.Filter.Eq(p => p.Id, id);
        var update = Builders<Playlist>.Update.Push(p => p.MovieIds, movieId);
        await _playlistCollection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteAsync(string id) =>
        await _playlistCollection.DeleteOneAsync(p => p.Id == id);
}
