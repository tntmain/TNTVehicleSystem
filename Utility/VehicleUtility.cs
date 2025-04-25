using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Linq;
using System.Text;
using TNTVehicleSystem.Main;
using TNTVehicleSystem.Models;
using UnityEngine;

namespace TNTVehicleSystem.Managers
{
    public class VehicleUtility
    {
        public static RaycastInfo GetAimAt(Player player, int pRayMasks, float pDistance = 10f)
        {
            return DamageTool.raycast(new Ray(player.look.aim.position, player.look.aim.forward), pDistance, pRayMasks);
        }

        public static bool IsRoad(float x, float z)
        {
            if (Physics.Raycast(new Vector3(x, 1024, z), Vector3.down, out var hit, 2048, RayMasks.GROUND | RayMasks.GROUND2 | RayMasks.TRANSPARENT_FX | RayMasks.CHART | RayMasks.STRUCTURE | RayMasks.STRUCTURE_INTERACT))
            {
                if (IsRoadSurfaceTypes(hit))
                {
                    return true;
                }
                
            }
            return false;
        }

        public static string GetName(UnturnedPlayer unturnedPlayer)
        {
            if (Physics.Raycast(new Vector3(unturnedPlayer.Position.x, 1024, unturnedPlayer.Position.z), Vector3.down, out var hit, 2048, RayMasks.GROUND | RayMasks.GROUND2 | RayMasks.TRANSPARENT_FX | RayMasks.CHART | RayMasks.STRUCTURE | RayMasks.STRUCTURE_INTERACT))
            {
                return hit.collider.name;
            }
            return "null";
        }

        public static void GiveKey(UnturnedPlayer Uplayer, byte[] newMeradata, bool isRemoveEmpty)
        {
            System.Random random = new System.Random();
            ushort randomItemId = Plugin.Instance.Configuration.Instance.KeyIds[random.Next(Plugin.Instance.Configuration.Instance.KeyIds.Count)];
            Uplayer.GiveItem(randomItemId, 1);
            foreach (var inventory in Uplayer.Inventory.items)
            {
                if (inventory == null)
                {
                    continue;
                }
                ProcessInventory(Uplayer, inventory, newMeradata);
            }
            if (isRemoveEmpty)
            {
                ProcessInventoryRemove(Uplayer, Plugin.Instance.Configuration.Instance.EmptyKeyId);
            }
        }

        private static void ProcessInventoryRemove(UnturnedPlayer Uplayer, uint itemRemove)
        {
            bool Active = true;
            foreach (var inventory in Uplayer.Inventory.items)
            {
                for (byte x = 0; x < inventory.width; x++)
                {
                    for (byte y = 0; y < inventory.height; y++)
                    {
                        if (Active)
                        {
                            byte index = inventory.getIndex(x, y);
                            if (index == byte.MaxValue)
                            {
                                return;
                            }

                            var item = inventory.getItem(index);
                            if (item.item.id == itemRemove)
                            {
                                Active= false;
                                inventory.removeItem(index);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static void ProcessInventory(UnturnedPlayer player, Items inventory, byte[] newMetadata)
        {
            for (byte x = 0; x < inventory.width; x++)
            {
                for (byte y = 0; y < inventory.height; y++)
                {
                    TryProcessInventorySlot(player, inventory, x, y, newMetadata);
                }
            }
        }

        private static void TryProcessInventorySlot(UnturnedPlayer player, Items inventory, byte x, byte y, byte[] newMetadata)
        {
            try
            {
                if (inventory == null)
                {
                    return;
                }
                byte index = inventory.getIndex(x, y);
                if (index == byte.MaxValue)
                {
                    return;
                }

                var item = inventory.getItem(index);

                if (item == null || item.item == null)
                {
                    return;
                }
                if (Plugin.Instance?.Configuration?.Instance == null)
                {
                    return;
                }
                if (Plugin.Instance.Configuration.Instance.KeyIds == null)
                {
                    return;
                }
                if (Plugin.Instance.Configuration.Instance.KeyIds.Contains(item.item.id) && DataBaseManager.InfioKey(item.item) == null)
                {
                    item.item.metadata = newMetadata;
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"TryProcessInventorySlotError: {ex}");
            }
        }

        public static void GivePlate(UnturnedPlayer Uplayer)
        {
            System.Random random = new System.Random();
            ushort randomItemId = Plugin.Instance.Configuration.Instance.KeyIds[random.Next(Plugin.Instance.Configuration.Instance.KeyIds.Count)];
            Uplayer.GiveItem(randomItemId, 1);
        }

        public static void SpawnVehicle(UnturnedPlayer Uplayer, VehicleInfo vehicleInfo)
        {
            InteractableVehicle interactableVehicle = VehicleManager.spawnVehicleV2(GetVehicleAssetByGUID(vehicleInfo.Guid), new Vector3(Uplayer.Position.x, Uplayer.Position.y+10, Uplayer.Position.z), Quaternion.identity);
            vehicleInfo.NetInstanceID = interactableVehicle.instanceID;
            DataBaseManager.SaveDataBase();
        }

        public static VehicleAsset GetVehicleAssetByGUID(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }
            if (!Guid.TryParse(guid, out Guid parsedGuid))
            {
                return null;
            }
            Asset asset = Assets.find(parsedGuid);
            if (asset == null)
            {
                return null;
            }
            if (asset is VehicleAsset vehicleAsset)
            {
                //Debug.Log($"Найден транспорт: {vehicleAsset.vehicleName}, Legacy ID: {vehicleAsset.id}, GUID: {vehicleAsset.GUID}");
                return vehicleAsset;
            }
            //Debug.LogWarning($"Ассет с GUID {guid} найден, но это не VehicleAsset, а {asset.GetType().Name}");
            return null;
        }

        /*public static bool IsExcludedVehicles(string guid)
        {
            foreach (var item in Plugin.Instance.Configuration.Instance.ExcludedVehicles)
            {
                if (guid == item)
                {
                    return true;
                }
            }
            return false;
        }*/
        public static bool IsExcludedVehicles(VehicleAsset asset)
        {
            string guid = asset.GUID.ToString();
            if (Main.Plugin.Instance.Configuration.Instance.ExcludedVehicles.Contains(guid))
            {
                return true;
            }

            string id = asset.id.ToString();
            if (Main.Plugin.Instance.Configuration.Instance.ExcludedVehicles.Contains(id))
            {
                return true;
            }

            return false;
        }
        public static void LogExcludedVehicles()
        {
            if (Main.Plugin.Instance.Configuration.Instance.ExcludedVehicles == null ||
                Main.Plugin.Instance.Configuration.Instance.ExcludedVehicles.Count == 0)
            {
                Rocket.Core.Logging.Logger.Log("No excluded vehicles in configuration.");
                return;
            }

            Rocket.Core.Logging.Logger.Log("Excluded vehicles list:");

            foreach (var identifier in Main.Plugin.Instance.Configuration.Instance.ExcludedVehicles)
            {
                if (ushort.TryParse(identifier, out ushort id))
                {
                    VehicleAsset asset = Assets.find(EAssetType.VEHICLE, id) as VehicleAsset;
                    if (asset != null)
                    {
                        Rocket.Core.Logging.Logger.Log($"- ID {id} ({asset.vehicleName})");
                        continue;
                    }
                }

                if (Guid.TryParse(identifier, out Guid guid))
                {
                    VehicleAsset asset = Assets.find(guid) as VehicleAsset;
                    if (asset != null)
                    {
                        Rocket.Core.Logging.Logger.Log($"- GUID \"{guid}\" ({asset.vehicleName})");
                        continue;
                    }
                }

                Rocket.Core.Logging.Logger.Log($"- Unknown identifier: {identifier}");
            }
        }

        private static bool IsRoadSurfaceTypes(RaycastHit hit)
        {
            foreach (var item in Plugin.Instance.Configuration.Instance.RoadSurfaceTypes)
            {
                if (hit.collider.name.StartsWith($"{item}"))
                {
                    return true;
                }
            }
            return false;
        }

        public static string GenerateNumber()
        {
            var TypeNumbers = Plugin.Instance.Configuration.Instance.NumberType;
            System.Random random = new System.Random();
            StringBuilder result = new StringBuilder();

            int currentIndex = 0;
            while (currentIndex < TypeNumbers.Length)
            {
                int openBraceIndex = TypeNumbers.IndexOf('{', currentIndex);
                if (openBraceIndex == -1)
                {
                    result.Append(TypeNumbers.Substring(currentIndex));
                    break;
                }

                result.Append(TypeNumbers.Substring(currentIndex, openBraceIndex - currentIndex));

                int closeBraceIndex = TypeNumbers.IndexOf('}', openBraceIndex);
                if (closeBraceIndex == -1)
                {
                    result.Append(TypeNumbers.Substring(openBraceIndex));
                    break;
                }

                string[] tokens = TypeNumbers.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1).Split(':');
                if (tokens.Length != 2)
                {
                    result.Append(TypeNumbers.Substring(openBraceIndex, closeBraceIndex - openBraceIndex + 1));
                    currentIndex = closeBraceIndex + 1;
                    continue;
                }

                string type = tokens[0];
                int count;
                if (!int.TryParse(tokens[1], out count))
                {
                    result.Append(TypeNumbers.Substring(openBraceIndex, closeBraceIndex - openBraceIndex + 1));
                    currentIndex = closeBraceIndex + 1;
                    continue;
                }

                switch (type)
                {
                    case "charEN":
                        for (int i = 0; i < count; i++)
                        {
                            char rndCharEN = (char)('A' + random.Next(0, 26));
                            result.Append(rndCharEN);
                        }
                        break;
                    case "number":
                        for (int i = 0; i < count; i++)
                        {
                            int randomDigit = random.Next(0, 10);
                            result.Append(randomDigit);
                        }
                        break;
                    case "charRU":
                        for (int i = 0; i < count; i++)
                        {
                            char rndCharRu = (char)('А' + random.Next(0, 32));
                            result.Append(rndCharRu);
                        }
                        break;
                    default:
                        result.Append(TypeNumbers.Substring(openBraceIndex, closeBraceIndex - openBraceIndex + 1));
                        break;
                }

                currentIndex = closeBraceIndex + 1;
            }

            return result.ToString();
        }

        public static byte[] GenerateNewMetadata()
        {
            int length = 10; 
            byte[] metadata = new byte[length];

            System.Random random = new System.Random();
            random.NextBytes(metadata);

            return metadata;
        }

        public static void UpdateNumbers(InteractableVehicle vehicle, string vehicleNumber)
        {
            VehicleBarricadeRegion vehicleRegion = BarricadeManager.findRegionFromVehicle(vehicle);

            if (vehicleRegion == null)
                return;

            foreach (var drop in vehicleRegion.drops.ToList())
            {
                if (drop.interactable is InteractableSign storage)
                {
                    //if (storage.text != vehicleNumber)
                    {
                        BarricadeManager.ServerSetSignText(storage, vehicleNumber);
                    }
                }
            }
        }

        public static void UpdateNumbers(InteractableVehicle vehicle)
        {
            VehicleBarricadeRegion vehicleRegion = BarricadeManager.findRegionFromVehicle(vehicle);

            if (vehicleRegion == null)
                return;

            foreach (var drop in vehicleRegion.drops.ToList())
            {
                if (drop.interactable is InteractableSign storage)
                {
                    string number = DataBaseManager.GetVehicle(vehicle).RegistrationNumber;
                    if (storage.text != number)
                    {
                        BarricadeManager.ServerSetSignText(storage, number);
                    }
                }
            }
        }

        public static int CountNumbers(InteractableVehicle vehicle)
        {
            VehicleBarricadeRegion vehicleRegion = BarricadeManager.findRegionFromVehicle(vehicle);
            int count = 0;
            if (vehicleRegion == null)
                return 0;

            foreach (var drop in vehicleRegion.drops.ToList())
            {
                if (drop.interactable is InteractableSign storage)
                {
                    count++;
                }
            }
            return count;
        }

        public static void VehicleBarricadeDestoy(InteractableVehicle vehicle)
        {
            VehicleBarricadeRegion vehicleRegion = BarricadeManager.findRegionFromVehicle(vehicle);

            if (vehicleRegion == null)
                return;

            foreach (var drop in vehicleRegion.drops.ToList())
            {
                if (drop.interactable is InteractableStorage storage)
                    storage.items.clear();

                BarricadeManager.tryGetPlant(vehicle.transform, out var x, out var y, out var plant, out _);
                BarricadeManager.destroyBarricade(drop, x, y, plant);
            }
        }
    }
}
