using SDG.Unturned;
using Steamworks;
using Newtonsoft.Json;

namespace TNTVehicleSystem.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VehicleInfo
    {
        [JsonProperty]
        public string Guid { get; set; }
        [JsonProperty]
        public uint NetInstanceID { get; set; }
        [JsonProperty]
        public string VehicleName { get; set; }
        [JsonProperty]
        public string RegistrationNumber { get; set; }
        [JsonProperty]
        public byte[] Key { get; set; }
        [JsonProperty]
        public string OwnerName { get; set; }
        [JsonProperty]
        public CSteamID OwnerId { get; set; }
    }
}
