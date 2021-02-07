using Grpc.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

 namespace GrpcServiceProjectsMongoDbServer.Services
 {
    public class BloggerService : BlogService.BlogServiceBase
    {
        private static MongoClient _mongoClient = new MongoClient("mongodb://localhost:27017");
        private static IMongoDatabase _mongoDatabase = _mongoClient.GetDatabase("BlogStoreDb");
        private static IMongoCollection<BsonDocument> _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>("blog");

        public override Task<CreateBlogResponse> CreateBlog(CreateBlogRequest request, ServerCallContext context)
        {
            var blog = request.Blog;
            BsonDocument doc = new BsonDocument("author_id", blog.AuthorId)
                                                .Add("title", blog.Title)
                                                .Add("content", blog.Content);

            _mongoCollection.InsertOne(doc);
            blog.Id = doc.GetValue("_id").ToString();

            return Task.FromResult(new CreateBlogResponse()
            {
                Blog = blog
            });
        }


        public override async Task<ReadBlogResponse> ReadBlog(ReadBlogRequest request, ServerCallContext context)
        {
            var blogId = request.BlogId;

            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", new ObjectId(blogId));
            var result = _mongoCollection.Find(filter).FirstOrDefault();

            if (result == null)
                throw new RpcException(new Status(StatusCode.NotFound, "The blog id " + blogId + " wasn't find"));

            Blog blog = new Blog()
            {
                AuthorId = result.GetValue("author_id").AsString,
                Title = result.GetValue("title").AsString,
                Content = result.GetValue("content").AsString
            };

            return new ReadBlogResponse() { Blog = blog };
        }



        public override async Task<UpdateBlogResponse> UpdateBlog(UpdateBlogRequest request, ServerCallContext context)
        {
            var blogId = request.Blog.Id;

            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", new ObjectId(blogId));
            var result = _mongoCollection.Find(filter).FirstOrDefault();

            if (result == null)
                throw new RpcException(new Status(StatusCode.NotFound, "The blog id " + blogId + " wasn't find"));

            var doc = new BsonDocument("author_id", request.Blog.AuthorId)
                                .Add("title", request.Blog.Title)
                                .Add("content", request.Blog.Content);

            _mongoCollection.ReplaceOne(filter, doc);

            var blog = new Blog()
            {
                AuthorId = doc.GetValue("author_id").AsString,
                Title = doc.GetValue("title").AsString,
                Content = doc.GetValue("content").AsString
            };

            blog.Id = blogId;

            return new UpdateBlogResponse() { Blog = blog };
        }



        public override async Task<DeleteBlogRepsponse> DeleteBlog(DeleteBlogRequest request, ServerCallContext context)
        {
            var blogId = request.BlogId;

            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", new ObjectId(blogId));

            var result = _mongoCollection.DeleteOne(filter);

            if (result.DeletedCount == 0)
                throw new RpcException(new Status(StatusCode.NotFound, "The blog with id " + blogId + " wasn't find"));

            return new DeleteBlogRepsponse() { BlogId = blogId };
        }




        public override async Task ListBlog(ListBlogRequest request, IServerStreamWriter<ListBlogResponse> responseStream, ServerCallContext context)
        {
            var filter = new FilterDefinitionBuilder<BsonDocument>().Empty;

            var result = _mongoCollection.Find(filter);

            foreach (var item in result.ToList())
            {
                await responseStream.WriteAsync(new ListBlogResponse()
                {
                    Blog = new Blog()
                    {
                        Id = item.GetValue("_id").ToString(),
                        AuthorId = item.GetValue("author_id").AsString,
                        Content = item.GetValue("content").AsString,
                        Title = item.GetValue("title").AsString
                    }
                });
            }
        }



    }
}
