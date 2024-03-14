using System.Reflection;
using ChatAppWebApi.Hubs;
using ChatAppWebApi.RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

builder.Services.AddSignalR(options =>
{
    
});

// builder.Services.AddSingleton<IConnection>(provider =>
// {
//     var factory = new ConnectionFactory()
//     {
//         HostName = "localhost", // Thay đổi thành địa chỉ RabbitMQ của bạn
//         UserName = "baongoc02",     // Thay đổi nếu cần thiết
//         Password = "123456"      // Thay đổi nếu cần thiết
//     };
//     return factory.CreateConnection();
// });

builder.Services.AddSingleton<RabbitMQManager>(provider =>
{
    // Thay đổi thông tin kết nối RabbitMQ của bạn
    string hostName = "localhost";
    string userName = "baongoc02";
    string password = "123456";
    
    var factory = new ConnectionFactory
    {
        HostName = hostName,
        UserName = userName,
        Password = password
    };

    var connection = factory.CreateConnection();
    var channel = connection.CreateModel();
    return new RabbitMQManager(channel);
});

builder.Services.AddSingleton<IModel>(provider =>
{
    var connection = provider.GetRequiredService<IConnection>();
    return connection.CreateModel();
});

builder.Services.AddSingleton<RabbitMQWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<ChatHub>("/api/chatHub");

app.MapControllers();

app.Run();
