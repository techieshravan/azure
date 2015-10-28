using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorage.Model
{
    public class PersonEntity : TableEntity
    {
        public PersonEntity() { }

        public PersonEntity(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
