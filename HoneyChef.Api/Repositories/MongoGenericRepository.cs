using System.Linq.Expressions;
using HoneyChef.Api.Persistence;
using IOITCore.Models.ViewModels.Bases;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HoneyChef.Api.Repositories
{
    public class MongoGenericRepository<TEntity> where TEntity : class
    {
        private readonly IMongoCollection<TEntity> _mongoService;

        public MongoGenericRepository(IDatabaseSettings settings)
        {
            try
            {
                var client = new MongoClient(settings.ConnectionString);
                var db = client.GetDatabase(settings.DatabaseName);
                string collection = GetCollectionName();
                _mongoService = db.GetCollection<TEntity>(collection);
            }
            catch (MongoException ex)
            {
                // handle exception ex for mongoException
            }
            catch (Exception ex)
            {
                // handl another exeption 
            }
        }
        
        private string GetCollectionName() 
        {
            return typeof(TEntity).Name;
        }

        public async Task<List<TEntity>> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await _mongoService.Find(predicate).ToListAsync();
            }
            catch (MongoException ex)
            {
                // Xử lý ngoại lệ MongoDBException ở đây
                // Ví dụ: Ghi log, thông báo cho người dùng, hoặc thực hiện hành động phục hồi khác
                Console.WriteLine($"Lỗi khi kết nối đến MongoDB: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ khác (nếu có) ở đây
                Console.WriteLine($"Lỗi không xác định: {ex.Message}");
                return null;
            }
        }

        public async Task<List<TEntity>> GetAllRecon(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await _mongoService.Find(predicate).ToListAsync();
            }
            catch (MongoException ex)
            {
                // Xử lý ngoại lệ MongoDBException ở đây
                // Ví dụ: Ghi log, thông báo cho người dùng, hoặc thực hiện hành động phục hồi khác
                Console.WriteLine($"Lỗi khi kết nối đến MongoDB: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ khác (nếu có) ở đây
                Console.WriteLine($"Lỗi không xác định: {ex.Message}");
                return null;
            }
        }

        public async Task<List<TEntity>> GetAllWithLimit(Expression<Func<TEntity, bool>> predicate, int limit)
        {
            try
            {
                return await _mongoService.Find(predicate).Limit(limit).ToListAsync();
            }
            catch (MongoException ex)
            {
                // Xử lý ngoại lệ MongoDBException ở đây
                // Ví dụ: Ghi log, thông báo cho người dùng, hoặc thực hiện hành động phục hồi khác
                Console.WriteLine($"Lỗi khi kết nối đến MongoDB: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ khác (nếu có) ở đây
                Console.WriteLine($"Lỗi không xác định: {ex.Message}");
                return null;
            }
        }
        public async Task<List<TEntity>> GetAllWithLimitOrderField(Expression<Func<TEntity, bool>> predicate, int limit, string orderField = null)
        {
            try
            {
                if (orderField == null)
                {
                    orderField = "created_date_origin";
                }
                return await _mongoService.Find(predicate).Sort(Builders<TEntity>.Sort.Descending(orderField)).Limit(limit).ToListAsync();
            }
            catch (MongoException ex)
            {
                // Xử lý ngoại lệ MongoDBException ở đây
                // Ví dụ: Ghi log, thông báo cho người dùng, hoặc thực hiện hành động phục hồi khác
                Console.WriteLine($"Lỗi khi kết nối đến MongoDB: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ khác (nếu có) ở đây
                Console.WriteLine($"Lỗi không xác định: {ex.Message}");
                return null;
            }
        }
        public async Task<TResponse> GetPagedData<TResponse>(FilterDefinition<TEntity> filter = null, int pageNo = -1, int pageSize = 10, string orderField = null)
            where TResponse : PagedResult<TEntity>, new()
        {
            // Nếu filter là null, khởi tạo filter rỗng
            if (filter == null)
            {
                filter = Builders<TEntity>.Filter.Empty;
            }
            if (orderField == null)
            {
                orderField = "created_date";
            }
            //if (pageSize > 20) pageSize = 20;
            // Tính toán số lượng bản ghi để bỏ qua dựa trên pageNo và pageSize
            int skip = (pageNo - 1) * pageSize;
            try
            {
                // Lấy dữ liệu từ cơ sở dữ liệu với phân trang
                var data = pageNo > 0 ? await _mongoService.Find(filter)
                             .Sort(Builders<TEntity>.Sort.Descending(orderField))
                             .Skip(skip)
                             .Limit(pageSize)
                             .ToListAsync() : await _mongoService.Find(filter)
                             .Sort(Builders<TEntity>.Sort.Descending(orderField))
                             .Limit(pageSize)
                             .ToListAsync();

                // Đếm tổng số lượng bản ghi
                var totals = await _mongoService.Find(filter).CountDocumentsAsync();
                //var totals = await _mongoService.CountAsync(filter);

                int totalPage = totals > 0
                   ? (int)Math.Ceiling(totals / (double)pageSize)
                   : 0;
                // Tạo đối tượng TResponse và đặt giá trị cho các thuộc tính
                TResponse response = new TResponse
                {
                    //data = data,
                    //total_elements = totals,
                    //total_page = totalPage,
                    //page_no = pageNo,
                    //page_size = pageSize
                };

                return response;
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ khác (nếu có) ở đây
                Console.WriteLine($"Lỗi không xác định: {ex.Message}");
                return null;
            }
        }

        // Phương thức tạo (Create)
        public async Task<(bool, string)> CreateAsync(TEntity entity)
        {
            try
            {
                await _mongoService.InsertOneAsync(entity);
                return (true, "OK");

            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }

        }
        public async Task<(bool, string)> CreateMultiAsync(List<TEntity> entity)
        {
            try
            {
                await _mongoService.InsertManyAsync(entity);
                return (true, "OK");

            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }

        }
        public async Task<TEntity> GetLastOneAsync(Expression<Func<TEntity, bool>> predicate, string orderFieldDesc = null)
        {
            try
            {
                return string.IsNullOrEmpty(orderFieldDesc) ? await _mongoService.Find(predicate).FirstOrDefaultAsync() : await _mongoService.Find(predicate).Sort(Builders<TEntity>.Sort.Descending(orderFieldDesc)).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Phương thức đọc (Read)
        public async Task<TEntity> GetByIdAsync(string id)
        {
            try
            {

                return await _mongoService.Find(Builders<TEntity>.Filter.Eq("_id", new ObjectId(id))).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Phương thức cập nhật (Update)
        public async Task<bool> UpdateAsync(string id, TEntity entity)
        {
            try
            {
                var filter = Builders<TEntity>.Filter.Eq("_id", new ObjectId(id));
                var replaceResult = await _mongoService.ReplaceOneAsync(filter, entity);
                if (replaceResult.IsAcknowledged && replaceResult.ModifiedCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Phương thức xóa (Delete)
        public async Task DeleteAsync(string id)
        {
            await _mongoService.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", new ObjectId(id)));
        }
        //xoas list
        public async Task DeleteListAsync(List<ObjectId> ids)
        {
            await _mongoService.DeleteManyAsync(Builders<TEntity>.Filter.In("_id", ids));
        }
        // Phương thức lấy danh sách (List) với điều kiện lọc (filter) tùy chọn
        public async Task<List<TEntity>> ListAsync(FilterDefinition<TEntity> filter = null)
        {
            try
            {
                return await _mongoService.Find(filter ?? Builders<TEntity>.Filter.Empty).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<TEntity>();
            }
        }

        public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                await _mongoService.InsertManyAsync(entities);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<UpdateResult> UpdateManyAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update)
        {
            // Sử dụng phương thức UpdateManyAsync của IMongoCollection<TEntity>
            // để cập nhật nhiều tài liệu trong cơ sở dữ liệu
            try
            {
                return await _mongoService.UpdateManyAsync(filter, update);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<BulkWriteResult<TEntity>> UpdateManyAsync(IEnumerable<(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update)> updates)
        {
            // Tạo một danh sách các hoạt động (operations) để thực hiện cập nhật
            var bulkOperations = new List<WriteModel<TEntity>>();

            // Lặp qua danh sách các cập nhật và thêm chúng vào danh sách các hoạt động
            foreach (var (filter, update) in updates)
            {
                var updateOneModel = new UpdateManyModel<TEntity>(filter, update);
                bulkOperations.Add(updateOneModel);
            }
            try
            {
                // Thực hiện hoạt động bulk write (cập nhật hàng loạt)
                return await _mongoService.BulkWriteAsync(bulkOperations);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<BulkWriteResult<TEntity>> UpdateManyAsync(List<WriteModel<TEntity>> writeModels)
        {
            try
            {
                // Thực hiện hoạt động bulk write (cập nhật hàng loạt)
                return await _mongoService.BulkWriteAsync(writeModels);
            }
            catch (Exception)
            {
                return null;
            }

        }
        // đếm
        public async Task<long> CountDataFilter(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await _mongoService.CountAsync(predicate);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }
}
