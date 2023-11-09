using CatShopApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace CatShopService
{
    public interface ICatService
    {
        Task<List<CatShopApi.Models.Cat>> GetAsync();
        Task<CatShopApi.Models.Cat> GetAsync(string id);
        Task CreateAsync(CatShopApi.Models.Cat newCat);
        Task UpdateAsync(string id, CatShopApi.Models.Cat updatedCat);
        Task<bool> DeleteCatAndAssociatedComments(string catId);
        Task RemoveAsync(string id);
    }


    public class CatService : ICatService
    {
        private readonly IMongoCollection<CatShopApi.Models.Cat> _catsCollection;
        private readonly IMongoCollection<Comment> _commentsCollection;

        public CatService(IMongoDatabase database)
        {
            _catsCollection = database.GetCollection<CatShopApi.Models.Cat>("Cats");
            _commentsCollection = database.GetCollection<Comment>("comments");
        }

        public async Task<List<CatShopApi.Models.Cat>> GetAsync()
        {
            return await _catsCollection.Find(cat => true).ToListAsync();
        }

        public async Task<CatShopApi.Models.Cat> GetAsync(string id)
        {
            return await _catsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(CatShopApi.Models.Cat newCat)
        {
            await _catsCollection.InsertOneAsync(newCat);
        }

        public async Task UpdateAsync(string id, CatShopApi.Models.Cat updatedCat)
        {
            var filter = Builders<CatShopApi.Models.Cat>.Filter.Eq(x => x.Id, id);
            var update = Builders<CatShopApi.Models.Cat>.Update
                .Set(x => x.name, updatedCat.name)
                .Set(x => x.breed, updatedCat.breed);

            await _catsCollection.UpdateOneAsync(filter, update);
        }

        public async Task<bool> DeleteCatAndAssociatedComments(string catId)
        {
            // Шаг 1: Удаление комментариев, связанных с котом
            await _commentsCollection.DeleteManyAsync(comment => comment.CatId == catId);

            // Шаг 2: Удаление самого кота
            var deleteResult = await _catsCollection.DeleteOneAsync(cat => cat.Id == catId);

            // Возвращаем результат удаления (true, если удалено успешно)
            return deleteResult.DeletedCount > 0;
        }


        public async Task RemoveAsync(string id)
        {
            var filter = Builders<CatShopApi.Models.Cat>.Filter.Eq("Id", id);
            await _catsCollection.DeleteOneAsync(filter);
        }
    }

}

