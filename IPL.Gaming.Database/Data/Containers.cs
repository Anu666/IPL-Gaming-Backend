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
            var strategy = new ContainerDetail()
            {
                Name = DataStore.User,
                PartitionKey = "id",
            };
            var masterdata = new ContainerDetail()
            {
                Name = DataStore.MasterData,
                PartitionKey = "type",
            };

            Containers.ContainerList.AddRange(
                new List<ContainerDetail>()
                {
                    strategy,
                    masterdata,
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

