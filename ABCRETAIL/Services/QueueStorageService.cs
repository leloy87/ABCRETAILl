using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;

namespace ABCRETAIL.Services
{
    public class QueueStorageService
    {
        private readonly QueueServiceClient _queueServiceClient;
        private readonly string _queueName;

        public QueueStorageService(string connectionString, string queueName)
        {
            _queueServiceClient = new QueueServiceClient(connectionString);
            _queueName = queueName.ToLower(); // Ensure queue name is in lowercase
        }

        public async Task SendMessageAsync(string message)
        {
            var queueClient = _queueServiceClient.GetQueueClient(_queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);
        }

        public async Task<string> ReceiveMessageAsync()
        {
            var queueClient = _queueServiceClient.GetQueueClient(_queueName);
            QueueMessage[] messages = await queueClient.ReceiveMessagesAsync(maxMessages: 1);

            if (messages.Length > 0)
            {
                string messageText = messages[0].MessageText;
                await queueClient.DeleteMessageAsync(messages[0].MessageId, messages[0].PopReceipt);
                return messageText;
            }

            return null;
        }

        public async Task DeleteQueueAsync()
        {
            var queueClient = _queueServiceClient.GetQueueClient(_queueName);
            await queueClient.DeleteIfExistsAsync();
        }
    }
}
