using IPL.Gaming.Database.Models;
using IPL.Gaming.Store;

namespace IPL.Gaming.Database.Data
{
    public class Containers
    {
        public static readonly List<ContainerDetail> ContainerList = new List<ContainerDetail>();

        static Containers()
        {
            PopulateContainerDetailsData();
        }

        private static void PopulateContainerDetailsData()
        {
            var user = new ContainerDetail()
            {
                Name = DataStore.User,
                PartitionKey = "id",
            };
            var masterdata = new ContainerDetail()
            {
                Name = DataStore.MasterData,
                PartitionKey = "type",
            };
            var match = new ContainerDetail()
            {
                Name = DataStore.Match,
                PartitionKey = "type",
            };
            var question = new ContainerDetail()
            {
                Name = DataStore.Question,
                PartitionKey = "matchId",
            };
            var userAnswer = new ContainerDetail()
            {
                Name = DataStore.UserAnswer,
                PartitionKey = "matchId",
            };
            var transaction = new ContainerDetail()
            {
                Name = DataStore.Transaction,
                PartitionKey = "userId",
            };
            var matchStatus = new ContainerDetail()
            {
                Name = DataStore.MatchStatus,
                PartitionKey = "matchId",
            };

            Containers.ContainerList.AddRange(
                new List<ContainerDetail>()
                {
                    user,
                    masterdata,
                    match,
                    question,
                    userAnswer,
                    transaction,
                    matchStatus
                }
            );
        }

        public static bool ValidateContainer(string containerName)
        {
            var containerDetail = Containers.ContainerList.FirstOrDefault(x => x.Name.ToUpper() == containerName.ToUpper());
            return containerDetail == null;
        }
    }
}

