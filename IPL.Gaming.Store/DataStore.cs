using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IPL.Gaming.Store
{
    public partial class DataStore
    {
        public const string User = "user";
        public const string Match = "match";
        public const string MasterData = "masterdata";
        public const string Question = "question";
        public const string UserAnswer = "useranswer";
        public const string Transaction = "transaction";

        public DataStore()
        {

        }

        public static List<object> GetRecords(string containerName)
        {
            switch (containerName)
            {
                case MasterData:
                    return MasterDataRecords;
                default:
                    return new List<object>();
            }
        }
    }
}
