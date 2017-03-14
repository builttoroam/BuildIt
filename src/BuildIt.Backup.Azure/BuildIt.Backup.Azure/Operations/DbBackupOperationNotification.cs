using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildIt.Backup.Azure.Operations
{
    public class DbBackupOperationNotification
    {
        public DbBackupOperationNotification(
            BackupOperationType operationType,
            string dbServer,
            string dbName,
            Guid operationId,
            string errorMessage)
        {
            OperationType = operationType;
            DbServer = dbServer;
            DbName = dbName;
            OperationId = operationId;
            ErrorMessage = errorMessage;
        }

        [JsonConverter(typeof(StringEnumConverter))] // Serializing as string for message readability
        public BackupOperationType OperationType { get; }
        public string DbServer { get; }
        public string DbName { get; }
        public Guid OperationId { get; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ErrorMessage { get; }
    }
}
