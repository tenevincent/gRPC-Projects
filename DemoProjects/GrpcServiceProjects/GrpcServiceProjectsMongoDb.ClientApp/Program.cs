using Grpc.Core;
using Grpc.Net.Client;
using GrpcServiceProjectsMongoDbServer;
using System;
using System.Threading.Tasks;

namespace GrpcServiceProjectsMongoDb.ClientApp
{
    class Program
    {
        private static GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
       private static BlogService.BlogServiceClient client = new BlogService.BlogServiceClient(channel);



        static async Task Main(string[] args)
        {
           var newBlog = CreateBlog(client);
             
           await ReadBlogAsync(client);

            UpdateBlog(client, newBlog);
            DeleteBlog(client, newBlog);

            await ListBlog(client);

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }


        private static Blog CreateBlog(BlogService.BlogServiceClient client)
        {
            var response = client.CreateBlog(new CreateBlogRequest()
            {
                Blog = new Blog()
                {
                    AuthorId = "Clement",
                    Title = "New blog!",
                    Content = "Hello world, this is a new blog"
                }
            });

            Console.WriteLine("The blog " + response.Blog.Id + " was created !");

            return response.Blog;
        }

        private static async Task ReadBlogAsync(BlogService.BlogServiceClient client)
        {
            try
            {
                var response = await client.ReadBlogAsync(new ReadBlogRequest()
                {
                    BlogId = "5ef0621520cd07227c217e46"
                });

                Console.WriteLine(response.Blog.ToString());
            }
            catch (RpcException e)
            {
                Console.WriteLine(e.Status.Detail);
            }
        }

        private static void UpdateBlog(BlogService.BlogServiceClient client, Blog blog)
        {
            try
            {
                blog.AuthorId = "Updated author";
                blog.Title = "Updated title";
                blog.Content = "Updated content";

                var response = client.UpdateBlog(new UpdateBlogRequest()
                {
                    Blog = blog
                });

                Console.WriteLine(response.Blog.ToString());
            }
            catch (RpcException e)
            {
                Console.WriteLine(e.Status.Detail);
            }
        }
        private static void DeleteBlog(BlogService.BlogServiceClient client, Blog blog)
        {
            try
            {
                var response = client.DeleteBlog(new DeleteBlogRequest() { BlogId = blog.Id });

                Console.WriteLine("The blog with id " + response.BlogId + " was deleted");
            }
            catch (RpcException e)
            {
                Console.WriteLine(e.Status.Detail);
            }
        }
        private static async Task ListBlog(BlogService.BlogServiceClient client)
        {
            var response = client.ListBlog(new ListBlogRequest() { });

            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Blog.ToString());
            }
        }
    
    


    }
}
 
