using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ABCRETAIL;

using ABCRETAIL.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Define the Azure Storage connection string, table names, and container name
string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=10102168storage;AccountKey=ug8PIgczkbve6LHC0YRsBW970PONbPNEMyG/SdDSTCM1Mod7Mu+wK5dXZUo+OzVuOSznVUfhVAI1+AStpatpiA==;EndpointSuffix=core.windows.net";
string customerProfileTableName = "CustomerProfiles";
//string productTableName = "Products";
string containerName = "imageblob";  // Define the blob container name
string queueName = "Messages";
string fileShareName = "yourfileshare";


// Register StorageService with the connection string and both table names
builder.Services.AddSingleton(new StorageService(ConnectionString, customerProfileTableName));
// Register FileService with the connection string and file share name

builder.Services.AddSingleton(new FileService(ConnectionString, fileShareName));

// Register BlobStorageService with the connection string and container name
builder.Services.AddSingleton(new BlobStorageService(ConnectionString, containerName));

// Register QueueStorageService with the queue name
builder.Services.AddSingleton(new QueueStorageService(ConnectionString, queueName));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();