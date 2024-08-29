using ABCRETAIL.Models;
using Azure.Data.Tables;
using Azure;

namespace ABCRETAIL.Services
{
    
        public class StorageService
        {
            private readonly TableServiceClient _tableServiceClient;
            private readonly string _customerProfileTableName;

            public StorageService(string storageConnectionString, string customerProfileTableName)
            {
                try
                {
                    _tableServiceClient = new TableServiceClient(storageConnectionString);
                    _customerProfileTableName = customerProfileTableName;

                    // Initialize the customer profile table
                    var customerProfileTableClient = _tableServiceClient.GetTableClient(_customerProfileTableName);
                    customerProfileTableClient.CreateIfNotExists();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing StorageService: {ex.Message}");
                    throw;
                }
            }

            // CustomerProfile Methods
            public async Task AddCustomerProfileAsync(CustomerProfile profile)
            {
                try
                {
                    var tableClient = _tableServiceClient.GetTableClient(_customerProfileTableName);
                    profile.PartitionKey = "Customer";
                    profile.RowKey = Guid.NewGuid().ToString();
                    await tableClient.AddEntityAsync(profile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding customer profile: {ex.Message}");
                    throw;
                }
            }

            public async Task<CustomerProfile> GetCustomerProfileAsync(string partitionKey, string rowKey)
            {
                try
                {
                    var tableClient = _tableServiceClient.GetTableClient(_customerProfileTableName);
                    return await tableClient.GetEntityAsync<CustomerProfile>(partitionKey, rowKey);
                }
                catch (RequestFailedException ex)
                {
                    Console.WriteLine($"RequestFailedException: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving customer profile: {ex.Message}");
                    throw;
                }
            }

            public async Task UpdateCustomerProfileAsync(CustomerProfile profile)
            {
                try
                {
                    var tableClient = _tableServiceClient.GetTableClient(_customerProfileTableName);

                    if (profile.ETag == null || profile.ETag.Equals(ETag.All))
                    {
                        throw new ArgumentException("ETag cannot be empty or All for update operations.");
                    }

                    await tableClient.UpdateEntityAsync(profile, profile.ETag, TableUpdateMode.Replace);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating customer profile: {ex.Message}");
                    throw;
                }
            }

            public async Task DeleteCustomerProfileAsync(string partitionKey, string rowKey, string eTag)
            {
                try
                {
                    var tableClient = _tableServiceClient.GetTableClient(_customerProfileTableName);

                    if (string.IsNullOrWhiteSpace(eTag))
                    {
                        throw new ArgumentException("ETag cannot be null or empty.");
                    }

                    var etag = new ETag(eTag);
                    await tableClient.DeleteEntityAsync(partitionKey, rowKey, etag);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting customer profile: {ex.Message}");
                    throw;
                }
            }

            public async Task<IEnumerable<CustomerProfile>> GetAllCustomerProfilesAsync()
            {
                var tableClient = _tableServiceClient.GetTableClient(_customerProfileTableName);
                var profiles = new List<CustomerProfile>();

                try
                {
                    await foreach (var profile in tableClient.QueryAsync<CustomerProfile>())
                    {
                        profiles.Add(profile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving all customer profiles: {ex.Message}");
                    throw;
                }

                return profiles;
            }
        }
    }