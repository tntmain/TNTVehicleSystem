using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using TNTPlus.Managers;
using TNTPlus.Models;
using TNTVehicleSystem.Managers;

namespace TNTVehicleSystem.Commands
{
    public class CommandVs : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public string Name
        {
            get
            {
                return "vs";
            }
        }

        public string Help
        {
            get
            {
                return "";
            }
        }

        public string Syntax
        {
            get
            {
                return "/vs";
            }
        }

        public List<string> Aliases
        {
            get
            {
                return new List<string>
                {
                };
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>
                {
                    "tnt.vs",
                };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;
            if (command.Length < 1)
            {
                MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vs_missing_command"), EMessageType.Error);
                return;
            }
            if (command[0] == "givekey")
            {
                if (!IRocketPlayerExtension.HasPermission(caller, "tnt.vs.givekey"))
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("permission_no_access"), EMessageType.Error);
                    return;
                }
                if (command.Length < 2)
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("givekey_missing_vehicle"), EMessageType.Error);
                    return;
                }
                var vehicle = DataBaseManager.GetVehicle(command[1]);
                if (vehicle != null)
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("givekey_key_gived"), EMessageType.Notification);
                    VehicleUtility.GiveKey(unturnedPlayer, vehicle.Key, false);
                }
                else
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("givekey_vehicle_not_found"), EMessageType.Error);
                }
                return;
            }
            else if (command[0] == "tp")
            {
                if (!IRocketPlayerExtension.HasPermission(caller, "tnt.vs.tp"))
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("permission_no_access"), EMessageType.Error);
                    return;
                }
                if (command.Length < 2)
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("tp_missing_vehicle"), EMessageType.Error);
                    return;
                }
                var vehicle = DataBaseManager.GetVehicle(command[1]);
                if (vehicle != null)
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("player_moved_to_vehicle"), EMessageType.Notification);
                    InteractableVehicle interactableVehicle = VehicleManager.getVehicle(vehicle.NetInstanceID);
                    unturnedPlayer.Player.transform.position = interactableVehicle.transform.position;
                }
                else
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_not_found"), EMessageType.Error);
                }
                return;
            }
            else if(command[0] == "tpv")
            {
                if (!IRocketPlayerExtension.HasPermission(caller, "tnt.vs.tpv"))
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("permission_no_access"), EMessageType.Error);
                    return;
                }
                if (command.Length < 2)
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("tpv_missing_vehicle"), EMessageType.Error);
                    return;
                }
                var vehicle = DataBaseManager.GetVehicle(command[1]);
                if (vehicle != null)
                {
                    InteractableVehicle interactableVehicle = VehicleManager.getVehicle(vehicle.NetInstanceID);
                    interactableVehicle.transform.position = unturnedPlayer.Position;
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_moved_to_player"), EMessageType.Notification);
                }
                else
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_not_found"), EMessageType.Error);
                }
            }
            else if(command[0] == "setowner")
            {
                if (!IRocketPlayerExtension.HasPermission(caller, "tnt.vs.setowner"))
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("permission_no_access"), EMessageType.Error);
                    return;
                }
                if (command.Length < 3)
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("setowner_missing_data"), EMessageType.Error);
                    return;
                }
                var vehicle = DataBaseManager.GetVehicle(command[1]);
                if (vehicle != null)
                {
                    UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(command[2]);
                    if (targetPlayer != null)
                    {
                        vehicle.OwnerId = targetPlayer.CSteamID;
                        DataBaseManager.SaveDataBase();
                        MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_owner_set"), EMessageType.Succes);
                    }
                    else
                    {
                        MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("player_not_found"), EMessageType.Error);
                    }
                }
                else
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_not_found"), EMessageType.Error);
                }
            }
            else if(command[0] == "setnumber")
            {
                if (!IRocketPlayerExtension.HasPermission(caller, "tnt.vs.setnumber"))
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("permission_no_access"), EMessageType.Error);
                    return;
                }
                if (command.Length < 3)
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("setnumber_missing_data"), EMessageType.Error);
                    return;
                }
                var vehicle = DataBaseManager.GetVehicle(command[1]);
                if (vehicle != null)
                {
                    vehicle.RegistrationNumber = command[2];
                    DataBaseManager.SaveDataBase();
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_number_set"), EMessageType.Succes);
                }
                else
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_not_found"), EMessageType.Error);
                }
            }
            else if(command[0] == "unreg")
            {
                if (!IRocketPlayerExtension.HasPermission(caller, "tnt.vs.unreg"))
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("permission_no_access"), EMessageType.Error);
                    return;
                }
                if (command.Length < 2)
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("unreg_missing_vehicle"), EMessageType.Error);
                    return;
                }
                var vehicle = DataBaseManager.GetVehicle(command[1]);
                if (vehicle != null)
                {
                    foreach (var item in DataBaseManager.GetVehicleInfoAll())
                    {
                        if (item == vehicle)
                        {
                            DataBaseManager.GetVehicleInfoAll().Remove(vehicle);
                            DataBaseManager.SaveDataBase();
                            MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_unregistered"), EMessageType.Succes);
                        }
                    }
                }
                else
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_not_found"), EMessageType.Error);
                }
            }
            else if(command[0] == "getground")
            {
                if (!IRocketPlayerExtension.HasPermission(caller, "tnt.vs.getground"))
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("permission_no_access"), EMessageType.Error);
                    return;
                }
                MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("getground_success", VehicleUtility.GetName(unturnedPlayer)), EMessageType.Succes);
            }
        }

    }
}
