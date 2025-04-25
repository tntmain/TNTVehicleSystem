using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using TNTVehicleSystem.Models;
using Steamworks;
using SDG.Unturned;
using Rocket.Unturned.Player;
using UnityEngine;
using TNTPlus.Managers;
using TNTPlus.Models;
using TNTPlus.Main;

namespace TNTVehicleSystem.Managers
{
    public class DataBaseManager
    {
        private string DataBaseFilePath;
        private List<VehicleInfo> vehicleInfo;

        public static DataBaseManager dataBaseManager;
        public DataBaseManager()
        {
            dataBaseManager = this;
            DataBaseFilePath = Path.Combine(Environment.CurrentDirectory, "Plugins", "TNTVehicleSystem", "DataBase", "Vehicles.json");
            LoadDataBase();
        }

        public void LoadDataBase()
        {
            var directoryPath = Path.GetDirectoryName(DataBaseFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (File.Exists(DataBaseFilePath))
            {
                string json = File.ReadAllText(DataBaseFilePath);
                vehicleInfo = JsonConvert.DeserializeObject<List<VehicleInfo>>(json) ?? new List<VehicleInfo>();
            }
            else
            {
                vehicleInfo = new List<VehicleInfo>();
            }
        }

        public static void SaveDataBase()
        {
            string json = JsonConvert.SerializeObject(dataBaseManager.vehicleInfo, Formatting.Indented);
            File.WriteAllText(dataBaseManager.DataBaseFilePath, json);
        }

        public bool CheckRegisterVehicle(uint netInstanceID)
        {
            foreach(var vehicle in vehicleInfo)
            {
                if (vehicle.NetInstanceID == netInstanceID)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<VehicleInfo> GetVehicleInfoAll()
        {
            return dataBaseManager.vehicleInfo;
        }

        public static VehicleInfo GetVehicle(InteractableVehicle vehicle)
        {
            foreach (var item in dataBaseManager.vehicleInfo)
            {
                if (item.NetInstanceID == vehicle.instanceID)
                {
                    return item;
                }
            }
            return null;
        }

        public static VehicleInfo GetVehicle(string number)
        {
            foreach (var item in dataBaseManager.vehicleInfo)
            {
                if (item.RegistrationNumber == number)
                {
                    return item;
                }
            }
            return null;
        }

        public void TryRegisterVehicle(InteractableVehicle vehicle, CSteamID ownerId)
        {
            UnturnedPlayer Uplayer = UnturnedPlayer.FromCSteamID(ownerId);
            byte[] newMetadata = VehicleUtility.GenerateNewMetadata();

            VehicleAsset asset = vehicle.asset;
            if (asset == null)
            {
                return;
            }
            if (VehicleUtility.IsExcludedVehicles(asset))
            {
                return;
            }

            string guid = asset.GUID.ToString();
            string vehicleName = asset.vehicleName ?? "Неизвестная машина";

            if (VehicleUtility.CountNumbers(vehicle) == 0)
            {
                MessageManager.Send(Uplayer, Main.Plugin.Instance.Translate("install_license_plates_on_your_vehicle"), EMessageType.Notification);
            }

            VehicleUtility.UpdateNumbers(vehicle);

            EffectManager.sendUIEffect(7431, 7431, Uplayer.Player.channel.GetOwnerTransportConnection(), true);

            if (CheckRegisterVehicle(vehicle.instanceID))
            {
                return;
            }

            vehicleInfo.Add(new VehicleInfo
            {
                Guid = guid,
                NetInstanceID = vehicle.instanceID,
                VehicleName = vehicleName,
                OwnerId = ownerId,
                OwnerName = UnturnedPlayer.FromCSteamID(ownerId).DisplayName,
                Key = newMetadata,
                RegistrationNumber = VehicleUtility.GenerateNumber(),
            });

            CSteamID cSteamID = CSteamID.Nil;
            VehicleManager.ServerSetVehicleLock(vehicle, cSteamID, cSteamID, true);
            MessageManager.Send(Uplayer, Main.Plugin.Instance.Translate("car_registered_with_license_plate", GetVehicle(vehicle).RegistrationNumber), EMessageType.Succes);
            SaveDataBase();

            if (Main.Plugin.Instance.Configuration.Instance.AutoIssueKey)
            {
                VehicleUtility.GiveKey(Uplayer, newMetadata, false);
            }
        }

        public static List<VehicleInfo> GetPlayerVehicles(UnturnedPlayer Uplayer)
        {
            List<VehicleInfo> vehicleInfos = DataBaseManager.GetVehicleInfoAll();
            List<VehicleInfo> vehicleInfos2 = new List<VehicleInfo>();
            foreach (VehicleInfo vehicleInfo in vehicleInfos)
            {
                if (vehicleInfo != null)
                {
                    if (vehicleInfo.OwnerId == Uplayer.CSteamID)
                    {
                        vehicleInfos2.Add(vehicleInfo);
                    }
                }
            }
            return vehicleInfos2;
        }

        public static VehicleInfo InfioKey(Item item)
        {
            foreach (var vi in dataBaseManager.vehicleInfo)
            {
                if (BitConverter.ToString(vi.Key) == BitConverter.ToString(item.metadata))
                {
                    return vi;
                }
            }
            return null;
        }
    }
}
