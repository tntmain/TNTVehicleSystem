using System;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Reflection;
using TNTVehicleSystem.Config;
using TNTVehicleSystem.Managers;
using UnityEngine;
using HarmonyLib;
using Rocket.Unturned;
using System.Collections.Generic;
using System.Collections;
using TNTPlus.Managers;
using SDG.NetTransport;
using TNTVehicleSystem.Models;
using TNTPlus.Models;

namespace TNTVehicleSystem.Main
{
    public class Plugin : RocketPlugin<Configurations>
    {
        public static Plugin Instance;
        private Harmony harmony;
        public static DataBaseManager dataBaseManager;
        private Dictionary<UnturnedPlayer, ItemJar> playerKeys = new Dictionary<UnturnedPlayer, ItemJar>();
        protected override void Load()
        {
            Instance = this;
            dataBaseManager = new DataBaseManager();

            harmony = new Harmony("TNTVehicleSystem");
            harmony.PatchAll();

            Rocket.Core.Logging.Logger.Log($"TNTVehicleSystem load success | version {Assembly.GetExecutingAssembly().GetName().Version}]");
            Rocket.Core.Logging.Logger.Log($"vk: https://vk.com/tntplugins");
            VehicleUtility.LogExcludedVehicles();
            if (Configuration.Instance.AutoLoadWorkShopMod)
            {
                WorkshopDownloadConfig.getOrLoad().File_IDs.Add(3238333133);
            }

            EffectManager.onEffectButtonClicked += EffectButtonClickedHandler;
            VehicleManager.onEnterVehicleRequested += VehicleManager_onEnterVehicleRequested;
            VehicleManager.onDamageVehicleRequested += DamageVehicleRequestHandler;
            VehicleManager.OnToggleVehicleLockRequested += VehicleManager_OnToggleVehicleLockRequested;
            VehicleManager.onVehicleCarjacked += VehicleCarjackedSignature;
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;

            if (Configuration.Instance.OffRoadTireDamage)
            {
                StartCoroutine(TierDestroy());
            }
        }

        protected override void Unload()
        {
            harmony.UnpatchAll();

            EffectManager.onEffectButtonClicked -= EffectButtonClickedHandler;
            VehicleManager.onEnterVehicleRequested -= VehicleManager_onEnterVehicleRequested;
            VehicleManager.onDamageVehicleRequested -= DamageVehicleRequestHandler;
            VehicleManager.OnToggleVehicleLockRequested -= VehicleManager_OnToggleVehicleLockRequested;
            VehicleManager.onVehicleCarjacked -= VehicleCarjackedSignature;
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;

            StopAllCoroutines();

            Instance = null;
            dataBaseManager = null;
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                TranslationList translationList = new TranslationList
                {
                    { "cannot_use_jack", "Вы не можете использовать домкрат." },
                    { "cannot_use_jack_only_on_jack_owner", "Вы можете использовать домкрат только на свой транспорт." },
                    { "car_registered_with_license_plate", "Автомобиль был зарегистрирован на вас, ему присвоен государственный номер {0}." },
                    { "install_license_plates_on_your_vehicle", "Установите на ваше транспортное средство номерные знаки." },
                    { "wheel_broken", "Колесо сломано." },
                    { "vehicle_moved_to_player", "Транспорт перемещен к вам." },
                    { "player_moved_to_vehicle", "Вы телепортированы к транспортному средству." },
                    { "not_in_vehicle", "Вы не находитесь в транспортном средстве." },
                    { "vehicle_not_found", "Транспорт не найден." },
                    { "vehicle_owner_set", "Владелец транспортного средства изменен." },
                    { "vehicle_number_set", "Регистрационный номер транспортного средства изменен." },
                    { "vehicle_unregistered", "Транспортное средство удалено из базы данных." },
                    { "vehicle_information_retrieved", "Информация о транспортном средстве получена." },
                    { "mv_command_missing", "Используйте /mv reg, /mv unreg или /mv transfer." },
                    { "vs_missing_command", "Команда не указана. Используйте: /vs 'tp', 'tpv','givekey','setowner', 'setnumber', 'unreg', 'getground'." },
                    { "tp_missing_vehicle", "Не указано транспортное средство для телепортации. Используйте: /vs tp <Номер_транспорта>" },
                    { "givekey_missing_vehicle", "Не указано транспортное средство. Используйте: /vs givekey <Номер_транспорта>" },
                    { "givekey_key_gived", "Вы получили ключ от транспортного средства." },
                    { "givekey_vehicle_not_found", "Транспортное средство не найдено." },
                    { "tpv_missing_vehicle", "Не указано транспортное средство для телепортации игрока. Используйте: /vs tpv <Номер_транспорта>" },
                    { "setowner_missing_data", "Не указано транспортное средство или игрок для установки владельца. Используйте: /vs setowner <Номер_транспорта> <имя_игрока>" },
                    { "setnumber_missing_data", "Не указано транспортное средство или новый номерной знак. Используйте: /vs setnumber <Номер_транспорта> <новый_номер>" },
                    { "unreg_missing_vehicle", "Не указано транспортное средство для удаления из базы данных. Используйте: /vs unreg <Номер_транспорта>" },
                    { "getground_success", "Вы стоите на поверхности: {0}." },
                    { "permission_no_access", "У вас нет доступа для выполнения этой команды." },
                    { "vehicle_already_registered", "Этот автомобиль уже зарегистрирован." },
                    { "vehicle_successfully_registered", "Транспорт успешно зарегистрирован." },
                    { "vehicle_successfully_unregistered", "Транспорт больше не зарегистрирован." },
                    { "not_vehicle_owner", "Вы не владелец этого транспортного средства." },
                    { "vehicle_not_registered", "Транспортное средство не зарегистрировано." },
                    { "transfer_missing_data", "Не указано транспортное средство или игрок для передачи." },
                    { "vehicle_owner_successfully_changed", "Владелец транспортного средства успешно изменен." },
                    { "insufficient_funds_for_vehicle_repair", "Недостаточно средств для ремонта вашего транспортного средства." },
                    { "error_vehicle_destroyed", "Транспорт, который вы пытаетесь найти, уничтожен!" },
                    { "vehicle_location_marker_on_map", "Местоположение вашего транспортного средства отмечено на карте." },
                    { "vehicle_repaired_and_spawned_near_you", "Транспорт был отремонирован и заспавнен поблизости от вас." },
                    { "duplicate_key_generated", "Вы успешно сделали дубликат ключа от транспортного средства." },
                    { "key_action_distance_insufficient", "Дистанция до транспортного средства слишком велика для действия ключа." },
                    { "key_invalid_no_vehicle", "Ключ недействителен: нет привязанного транспорта." }
                };
                return translationList;
            }
        }

        public void DamageVehicleRequestHandler(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if ((vehicle.health - pendingTotalDamage) <= 0)
            {
                var veh = DataBaseManager.GetVehicle(vehicle);
                if (veh != null)
                {
                    if (veh.NetInstanceID == vehicle.instanceID)
                    {
                        veh.NetInstanceID = 0;
                        DataBaseManager.SaveDataBase();
                    }
                }
            }
        }

        private void VehicleManager_onEnterVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            UnturnedPlayer Uplayer = UnturnedPlayer.FromPlayer(player);

            if (Configuration.Instance.AutoRegisterOnFirstEntry)
            {
                dataBaseManager.TryRegisterVehicle(vehicle, Uplayer.CSteamID);
            }
        }

        private IEnumerator TierDestroy()
        {
            while (true)
            {
                yield return new WaitForSeconds(Configuration.Instance.OffRoadCheckInterval);

                foreach (var player in Provider.clients)
                {
                    UnturnedPlayer Uplayer = UnturnedPlayer.FromSteamPlayer(player);

                    if (Uplayer.IsInVehicle && VehicleUtility.IsRoad(Uplayer.Position.x,Uplayer.Position.z) == false)
                    {
                        InteractableVehicle vehicle = Uplayer.CurrentVehicle;
                        float speedInKmPerHour = vehicle.ReplicatedSpeed * 3.6f;

                        int randomTireIndex = UnityEngine.Random.Range(0, vehicle.tires.Length);

                        if (randomTireIndex >= 0 && randomTireIndex < vehicle.tires.Length && vehicle.tires[randomTireIndex].isAlive && speedInKmPerHour > Configuration.Instance.OffRoadTireBreakSpeed)
                        {
                            VehicleManager.damageTire(vehicle, (byte)randomTireIndex, CSteamID.Nil, EDamageOrigin.Unknown);
                            MessageManager.Send(Uplayer, Translate("wheel_broken"), EMessageType.Error);
                        }
                    }
                }
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            player.Player.equipment.onEquipRequested += OnEquipRequested;
        }

        private void OnEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(equipment.player);

            Models.VehicleInfo ite2 = DataBaseManager.InfioKey(jar.item);
            if (ite2 != null)
            {
                if (BitConverter.ToString(jar.item.metadata) == BitConverter.ToString(ite2.Key))
                {
                    if (playerKeys.ContainsKey(player))
                    {
                        playerKeys[player] = jar;
                    }
                    else
                    {
                        playerKeys.Add(player, jar);
                    }
                    DelayedVehicleAnnouncement(player, ite2);
                }
            }
        }

        private void DelayedVehicleAnnouncement(UnturnedPlayer player, VehicleInfo vehicleInfo)
        {
            ITransportConnection Tplayer = player.Player.channel.GetOwnerTransportConnection();
            EffectManager.sendUIEffect(7521, 7521, Tplayer, true);
            EffectManager.sendUIEffectText(7521, Tplayer, true, $"VehicleSystem.key.text.number", $"{vehicleInfo.RegistrationNumber}");
            if (vehicleInfo.NetInstanceID != 0)
            {
                EffectManager.sendUIEffectText(7521, Tplayer, true, $"VehicleSystem.key.text.name", $"{vehicleInfo.VehicleName}");
                if (VehicleManager.getVehicle(vehicleInfo.NetInstanceID).isLocked)
                {
                    EffectManager.sendUIEffectVisibility(7521, Tplayer, true, $"VehicleSystem.key.red", true);
                    return;
                }
                EffectManager.sendUIEffectVisibility(7521, Tplayer, true, $"VehicleSystem.key.green", true);
                return;
            }
            EffectManager.sendUIEffectText(7521, Tplayer, true, $"VehicleSystem.key.text.name", $"{vehicleInfo.VehicleName} (Уничтожен)");
            EffectManager.sendUIEffectVisibility(7521, Tplayer, true, $"VehicleSystem.key.red", true);
            return;
        }

        public void EffectButtonClickedHandler(Player player, string buttonName)
        {
            UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromPlayer(player);
            ITransportConnection Tplayer = player.channel.GetOwnerTransportConnection();
            if (buttonName == "VehicleButton.Close")
            {
                EffectManager.askEffectClearByID(7520, Tplayer);
                unturnedPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            }
            if (buttonName.Contains("VehicleButtonFix."))
            {
                List<VehicleInfo> vehicleInfos = DataBaseManager.GetPlayerVehicles(unturnedPlayer);
                for (int i = 0; i <= vehicleInfos.Count; i++)
                {
                    if (buttonName == $"VehicleButtonFix.{i}")
                    {
                        unturnedPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                        EffectManager.askEffectClearByID(7520, Tplayer);
                        if (unturnedPlayer.Experience >= Configuration.Instance.RepairCost)
                        {
                            VehicleUtility.SpawnVehicle(unturnedPlayer, vehicleInfos[i]);
                            MessageManager.Send(unturnedPlayer, Translate("vehicle_repaired_and_spawned_near_you"), EMessageType.Succes);
                            return;
                        }
                        MessageManager.Send(unturnedPlayer, Translate("insufficient_funds_for_vehicle_repair"), EMessageType.Error);
                        return;
                    }
                }
            }
            if (buttonName.Contains("VehicleButtonFind."))
            {
                List<VehicleInfo> vehicleInfos = DataBaseManager.GetPlayerVehicles(unturnedPlayer);
                for (int i = 0; i <= vehicleInfos.Count; i++)
                {
                    if (buttonName == $"VehicleButtonFind.{i}")
                    {
                        unturnedPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                        EffectManager.askEffectClearByID(7520, Tplayer);

                        InteractableVehicle interactableVehicle = VehicleManager.getVehicle(vehicleInfos[i].NetInstanceID);
                        if (interactableVehicle != null)
                        {
                            unturnedPlayer.Player.quests.ReceiveSetMarkerRequest(true, interactableVehicle.transform.position);
                            MessageManager.Send(unturnedPlayer, Translate("vehicle_location_marker_on_map"), EMessageType.Notification);
                            return;
                        }
                        MessageManager.Send(unturnedPlayer, Translate("error_vehicle_destroyed"), EMessageType.Error);
                        return;
                    }
                }
            }
        }

        private void VehicleManager_OnToggleVehicleLockRequested(InteractableVehicle vehicle, ref bool shouldAllow)
        {
            if (Configuration.Instance.LockWithKeyOnly)
            {
                if (vehicle.isLocked)
                {
                    shouldAllow = true;
                    return;
                }
                shouldAllow = false;
                CSteamID cSteamID = CSteamID.Nil;
                VehicleManager.ServerSetVehicleLock(vehicle, cSteamID, cSteamID, true);
                return;
            }
        }

        public void VehicleCarjackedSignature(InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow, ref Vector3 force, ref Vector3 torque)
        {
            VehicleInfo vehicleInfo = DataBaseManager.GetVehicle(vehicle);
            UnturnedPlayer Uplayer = UnturnedPlayer.FromPlayer(instigatingPlayer);

            if (!Configuration.Instance.Carjacking)
            {
                MessageManager.Send(Uplayer, Translate("cannot_use_jack"), EMessageType.Error);
                allow = false;
                return;
            }

            if (Configuration.Instance.CarjackingOwnerOnly)
            {
                if (vehicleInfo == null || vehicleInfo.OwnerId != Uplayer.CSteamID)
                {
                    MessageManager.Send(Uplayer, Translate("cannot_use_jack_only_on_jack_owner"), EMessageType.Warning);
                    allow = false;
                    return;
                }
            }

            allow = true;
        }

        public static void HandleSwing(UseableMelee instance)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(instance.player);
            ITransportConnection Tplayer = player.Player.channel.GetOwnerTransportConnection();
            if (Plugin.Instance.Configuration.Instance.EmptyKeyId == player.Player.equipment.itemID)
            {
                InteractableVehicle interactableVehicle = VehicleUtility.GetAimAt(player.Player, RayMasks.VEHICLE, 10f).vehicle;
                if (interactableVehicle != null)
                {
                    var vehicleInfo = DataBaseManager.GetVehicle(interactableVehicle);
                    if (vehicleInfo != null)
                    {
                        if (vehicleInfo.OwnerId != player.CSteamID)
                        {
                            MessageManager.Send(player, Instance.Translate("not_vehicle_owner"), EMessageType.Error);
                            return;
                        }
                        MessageManager.Send(player, Instance.Translate("duplicate_key_generated"), EMessageType.Succes);
                        VehicleUtility.GiveKey(player, vehicleInfo.Key, true);
                        player.Player.equipment.dequip();
                        return;
                    }
                    MessageManager.Send(player, Instance.Translate("vehicle_not_registered"), EMessageType.Warning);
                }
            }
            if (Instance.playerKeys.ContainsKey(player) && Main.Plugin.Instance.Configuration.Instance.KeyIds.Contains(player.Player.equipment.itemID))
            {

                ItemJar key = Instance.playerKeys[player];
                Models.VehicleInfo ite2 = DataBaseManager.InfioKey(key.item);
                if (ite2 == null)
                {
                    MessageManager.Send(player, Instance.Translate("key_invalid_no_vehicle"), EMessageType.Error);
                    return;
                }
                
                if (ite2.NetInstanceID != 0)
                {
                    if (Main.Plugin.Instance.Configuration.Instance.KeyActionDistance < Vector3.Distance(VehicleManager.getVehicle(ite2.NetInstanceID).transform.position, player.Position))
                    {
                        MessageManager.Send(player, Instance.Translate("key_action_distance_insufficient"), EMessageType.Notification);
                        return;
                    }
                    EffectManager.sendUIEffectVisibility(7521, Tplayer, true, $"VehicleSystem.key.green", false);
                    EffectManager.sendUIEffectVisibility(7521, Tplayer, true, $"VehicleSystem.key.red", false);
                    if (VehicleManager.getVehicle(ite2.NetInstanceID).isLocked)
                    {
                        VehicleManager.ServerSetVehicleLock(VehicleManager.getVehicle(ite2.NetInstanceID), CSteamID.Nil, CSteamID.Nil, false);
                        EffectManager.sendUIEffectVisibility(7521, Tplayer, true, $"VehicleSystem.key.green", true);
                        return;
                    }
                    VehicleManager.ServerSetVehicleLock(VehicleManager.getVehicle(ite2.NetInstanceID), CSteamID.Nil, CSteamID.Nil, true);
                    EffectManager.sendUIEffectVisibility(7521, Tplayer, true, $"VehicleSystem.key.red", true);
                }
            }
        }
    }
}
