using System.Collections.Generic;
using Rocket.API;
namespace TNTVehicleSystem.Config
{
    public class Configurations : IRocketPluginConfiguration, IDefaultable
    {
        public bool AutoLoadWorkShopMod { get; set; }
        public string NumberType { get; set; }
        public uint RepairCost { get; set; }
        public bool AutoRegisterOnFirstEntry { get; set; }
        public bool AutoIssueKey { get; set; }
        public bool LockWithKeyOnly { get; set; }
        public uint EmptyKeyId { get; set; }
        public List<ushort> KeyIds { get; set; }
        public float KeyActionDistance { get; set; }
        public List<string> ExcludedVehicles { get; set; }
        public bool Carjacking { get; set; }
        public bool CarjackingOwnerOnly { get; set; }
        public List<string> RoadSurfaceTypes { get; set; }
        public bool OffRoadTireDamage { get; set; }
        public float OffRoadTireBreakSpeed { get; set; }
        public float OffRoadCheckInterval { get; set; }

        public void LoadDefaults()
        {
            AutoLoadWorkShopMod = true;
            NumberType = "{charEN:1}-{number:3}{charRU:2}";
            RepairCost = 0;
            AutoRegisterOnFirstEntry = true;
            AutoIssueKey = true;
            LockWithKeyOnly = true;
            EmptyKeyId = 11099;
            KeyIds = new List<ushort>()
            {
                11100,
                11101,
                11102,
                11103,
                11104,
            };
            KeyActionDistance = 15;
            ExcludedVehicles = new List<string>()
            {
                "e0f8b4b2-3249-483a-9cd5-c1402e2170fb",
            };
            Carjacking = true;
            CarjackingOwnerOnly = false;
            RoadSurfaceTypes = new List<string>() { "Segment_", "Road_Line_", "Road_Tee_" };
            OffRoadTireDamage = true;
            OffRoadTireBreakSpeed = 20;
            OffRoadCheckInterval = 10;
        }
    }
}
