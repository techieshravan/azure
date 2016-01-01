using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorage
{
    public class EmployeeEntity : TableEntity
    {
        public EmployeeEntity(string lastName,string firstName)
        {
            PartitionKey = lastName;
            RowKey = firstName;
        }

        public EmployeeEntity()
        {
            
        }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
