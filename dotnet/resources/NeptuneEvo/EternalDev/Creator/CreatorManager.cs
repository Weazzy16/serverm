using Database;
using GTANetworkAPI;
using GTANetworkMethods;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.BattlePass.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Core;
using NeptuneEvo.EternalDev.Creator.Classes;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg.Sig;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//task
using System.Threading.Tasks;

namespace NeptuneEvo.EternalDev.Creator
{
    public class CreatorManager : Script
    {
        private static readonly nLog Log = new nLog(nameof(CreatorManager));

        //chat_gpt
        //private static bool isActionInProgress = false;

        public static void SendCreator(ExtPlayer player, bool isFirst)
        {
            try
            {
                var accountData = player.GetAccountData();
                if (accountData is null) return;

                var characterData = player.GetCharacterData();
                if (characterData is null) return;

                var slot = accountData.Chars.IndexOf(characterData.UUID);
                if (slot == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ошибка отправвки в меню кастомизации", 3000);
                    return;
                }

                player.SessionData.CreatorData.Inside = true;
                player.SessionData.CreatorData.Changed = true;
                player.SessionData.CreatorData.Tattoos = player.Customization.Tattoos;

                Trigger.Dimension(player, (uint)(player.Value + 1));
                Trigger.ClientEvent(player, "client.creator.open", false, characterData.FirstName, characterData.LastName);
            }
            catch (Exception ex) { Log.Write(nameof(SendCreator) + ": " + ex.ToString()); }
        }

        [RemoteEvent("server.creator.ready")]
        public void OnCreate(ExtPlayer player, int slot, string name, string surname, int genderId, int mother, int father, float similarity, 
            float skinSimilarity, int hair, int hairColor1, int hairColor2, int eyeColor, string jsonOverlay, string jsonFaceFeatureParams, string jsonClothes)
        {
            Trigger.SetTask(async () =>
            {
                try
                {
                    var accountData = player.GetAccountData();
                    if (accountData == null)
                        return;

                    var sessionData = player.GetSessionData();
                    if (sessionData == null)
                        return;

                    var isChanging = sessionData.CreatorData.Changed;

                    var custom = player.GetCustomization();

                    if (!isChanging)
                    {

                        if (accountData.Chars[slot] != -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Слот занят!", 3000);
                            return;
                        }

                        int result = await Character.Create.Repository.Create(player, name, surname);
                        if (result == -1) return;

                        accountData.Chars[slot] = result;
                        Main.Usernames[accountData.Login] = accountData.Chars;

                        if (custom == null)
                            custom = new PlayerCustomization();
                    }

                    var characterData = player.GetCharacterData();

                    bool gender = genderId == 0 ? true : false;
                    var genderChanged = !(isChanging && sessionData.CreatorData.IsCreate && characterData.Gender == gender);

                    characterData.Gender = gender;

                    custom.Parents.Father = father;
                    custom.Parents.Mother = mother;

                    custom.Parents.Similarity = similarity;
                    custom.Parents.SkinSimilarity = skinSimilarity;

                    var feature_data = JsonConvert.DeserializeObject<float[]>(jsonFaceFeatureParams);
                    custom.Features = feature_data;

                    var overlays = JsonConvert.DeserializeObject<List<OverlayData>>(jsonOverlay);
                    custom.Appearance = overlays.Select(x => new AppearanceItem(x.Index, x.Opacity, x.Color1)).ToArray();

                    custom.Hair.Hair = hair;
                    custom.Hair.Color = hairColor1;
                    custom.Hair.HighlightColor = hairColor2;

                    custom.EyeColor = eyeColor;

                    if (!genderChanged && isChanging)
                        custom.Tattoos = sessionData.CreatorData.Tattoos;

                    player.SetCustomization(custom);

                    NAPI.Task.Run(() =>
                    {
                        Trigger.ClientEvent(player, "client.charStore.Gender", gender);

                        player.SetDefaultSkin();

                        if (genderChanged)
                        {
                            if (isChanging)
                                Chars.Repository.RemoveAllClothes(player);

                            var clothes_data = JsonConvert.DeserializeObject<Dictionary<string, ClothesData>>(jsonClothes);
                            var topsData = Chars.ClothesComponents.ClothesComponentData[gender][Chars.ClothesComponent.Tops];
                            foreach (var item in clothes_data)
                            {
                                if (item.Value.Drawable == -1) continue;

                                int slotId = item.Key == "tops" ? Chars.ClothesComponents.IsTopDown(topsData[item.Value.Drawable]) ? 6 : 5 : item.Key == "legs" ? 9 : item.Key == "shoes" ? 13 : -1;
                                if (slotId == -1) continue;

                                Chars.Repository.ChangeAccessoriesItem(player, slotId, $"{item.Value.Drawable}_{item.Value.Texture}_{characterData.Gender}");
                            }
                        }

                        Trigger.ClientEvent(player, "client.creator.close");

                        if (!isChanging)
                        {
                            sessionData.LoggedIn = true;
                            //Trigger.ClientEvent(player, "startWelcomeCutscene", genderId, $"{name} {surname}");

                            //NAPI.Task.Run(() =>
                            //{
                            //    Trigger.ClientEvent(player, "startWelcomeCutscene", genderId, $"{name} {surname}", 26000);
                            //    Main.ClientEvent_Spawn(player, "last");
                            //});


                            Main.HelloText(player);

                            Trigger.SendToAdmins(1, LangFunc.GetText(LangType.Ru, DataName.NewChar, player.Name, player.Value));

                            Trigger.ClientEvent(player, "client:OnOpenHelpMenu");
                            characterData.SpawnPos = Customization.GetSpawnPos();



                            
                        }
                        else
                            characterData.SpawnPos = new Vector3(-1039.687, -2741.4824, 20.169268);

                        Main.ClientEvent_Spawn(player, "last");
                    });


                    sessionData.CreatorData = new CreatorData();


                    if (!isChanging)
                    {
                        Customization.InsertCustomization(player);
                    }
                    else
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке

                        await Customization.SaveCharacter(db, player, characterData.UUID, UpdateCreate: true);
                    }
                }
                catch (Exception ex) { Log.Write("OnCreate: " + ex.ToString()); }
            });
        }
    }
}
