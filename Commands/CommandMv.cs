using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using TNTPlus.Managers;
using TNTPlus.Models;
using TNTVehicleSystem.Managers;

namespace TNTVehicleSystem.Commands
{
    public class CommandMv : IRocketCommand
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
                return "mv";
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
                return "/mv";
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
                    "tnt.mv",
                };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;
            if (command.Length < 1)
            {
                MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("mv_command_missing"), EMessageType.Warning);
                VehicleManagerUi.OpenUi(unturnedPlayer);
                return;
            }
            if (command[0] == "reg")
            {
                if (!IRocketPlayerExtension.HasPermission(caller, "tnt.mv.reg"))
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("permission_no_access"), EMessageType.Error);
                    return;
                }
                if (unturnedPlayer.IsInVehicle)
                {
                    var vehicle = unturnedPlayer.CurrentVehicle;
                    if (DataBaseManager.GetVehicle(vehicle) != null)
                    {
                        MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_already_registered"), EMessageType.Warning);
                        return;
                    }
                    Main.Plugin.dataBaseManager.TryRegisterVehicle(vehicle, unturnedPlayer.CSteamID);
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_successfully_registered"), EMessageType.Succes);
                }
                else
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("not_in_vehicle"), EMessageType.Error);
                }
                return;
            }
            if (command[0] == "unreg")
            {
                if (!IRocketPlayerExtension.HasPermission(caller, "tnt.mv.unreg"))
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("permission_no_access"), EMessageType.Error);
                    return;
                }
                if (unturnedPlayer.IsInVehicle)
                {
                    var vehicle = unturnedPlayer.CurrentVehicle;
                    var datVeh = DataBaseManager.GetVehicle(vehicle);
                    if (datVeh != null)
                    {
                        if (datVeh.OwnerId == unturnedPlayer.CSteamID)
                        {
                            foreach (var item in DataBaseManager.GetVehicleInfoAll())
                            {
                                if (item == datVeh)
                                {
                                    DataBaseManager.GetVehicleInfoAll().Remove(datVeh);
                                    DataBaseManager.SaveDataBase();
                                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_successfully_unregistered"), EMessageType.Succes);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("not_vehicle_owner"), EMessageType.Error);
                            return;
                        }
                    }
                    else
                    {
                        MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_not_registered"), EMessageType.Error);
                        return;
                    }
                }
                else
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("not_in_vehicle"), EMessageType.Error);
                }
                return;
            }
            if (command[0] == "transfer")
            {
                if (!IRocketPlayerExtension.HasPermission(caller, "tnt.mv.transfer"))
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("permission_no_access"), EMessageType.Error);
                    return;
                }
                if (command.Length < 2)
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("transfer_missing_data"), EMessageType.Error);
                    return;
                }
                if (unturnedPlayer.IsInVehicle)
                {
                    var vehicle = unturnedPlayer.CurrentVehicle;
                    var datVeh = DataBaseManager.GetVehicle(vehicle);
                    if (vehicle != null)
                    {
                        if (datVeh.OwnerId != unturnedPlayer.CSteamID)
                        {
                            MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("not_vehicle_owner"), EMessageType.Error);
                            return;
                        }
                        UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(command[1]);
                        if (targetPlayer != null)
                        {
                            datVeh.OwnerId = targetPlayer.CSteamID;
                            DataBaseManager.SaveDataBase();
                            MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("vehicle_owner_successfully_changed"), EMessageType.Succes);
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
                else
                {
                    MessageManager.Send(unturnedPlayer, Main.Plugin.Instance.Translate("not_in_vehicle"), EMessageType.Error);
                }
                return;
            }
        }

    }
}
