using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using System.Collections.Generic;
using TNTVehicleSystem.Models;

namespace TNTVehicleSystem.Managers
{
    public class VehicleManagerUi
    {
        public static void OpenUi(UnturnedPlayer Uplayer)
        {
            int id = 0;

            ITransportConnection Tplayer = Uplayer.Player.channel.GetOwnerTransportConnection();
            List<VehicleInfo> vehicleInfos = DataBaseManager.GetVehicleInfoAll();

            EffectManager.sendUIEffect(7520, 7520, Tplayer, true);
            Uplayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);

            foreach (VehicleInfo vehicleInfo in vehicleInfos)
            {
                if (vehicleInfo != null)
                {
                    if (vehicleInfo.OwnerId == Uplayer.CSteamID)
                    {
                        EffectManager.sendUIEffectVisibility(7520, Tplayer, true, $"Vehicle.{id}", true);
                        EffectManager.sendUIEffectText(7520, Tplayer, true, $"Vehicle.text.{id}", $"{vehicleInfo.RegistrationNumber}");
                        
                        var vfi = VehicleManager.getVehicle(vehicleInfo.NetInstanceID);

                        if (vfi)
                        {
                            if (vehicleInfo.NetInstanceID == 0)
                            {
                                EffectManager.sendUIEffectVisibility(7520, Tplayer, true, $"VehicleButtonFix.{id}", true);
                                EffectManager.sendUIEffectText(7520, Tplayer, true, $"Vehicle.SecondText.{id}", $"{vehicleInfo.RegistrationNumber}");
                            }
                            else
                            {
                                EffectManager.sendUIEffectVisibility(7520, Tplayer, true, $"VehicleButtonFind.{id}", true);
                            }
                        }
                        else
                        {
                            EffectManager.sendUIEffectVisibility(7520, Tplayer, true, $"VehicleButtonFix.{id}", true);
                            EffectManager.sendUIEffectText(7520, Tplayer, true, $"Vehicle.SecondText.{id}", $"{vehicleInfo.RegistrationNumber}");
                        }
                        id++;
                    }
                }
            }
        }
    }
}
