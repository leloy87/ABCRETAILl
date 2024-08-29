using Azure.Data.Tables;
using Azure;
using System.ComponentModel.DataAnnotations;
using ABCRETAIL;

namespace ABCRETAIL.Models
{
    public class CustomerProfile : ITableEntity
    {
        // PartitionKey is typically used to organize entities by a logical division
        public string PartitionKey { get; set; }

        // RowKey is the unique identifier for each entity within a partition
        public string RowKey { get; set; }

        // Timestamp and ETag are used by Azure Table Storage for concurrency and tracking
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Custom properties with validation attributes
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "First Name cannot be longer than 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, ErrorMessage = "Last Name cannot be longer than 100 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }
    }
}