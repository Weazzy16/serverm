using Database;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using LinqToDB;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Players;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Localization;
using System.Net.Mail;
using System.Net;

namespace NeptuneEvo.Accounts.Recovery
{
    class Repository
    {
        private static readonly nLog Log = new nLog("Accounts.Repository.Events");

        private static string _mailFrom = "piecerp@piecerp.ru";
        private static string _mailTitle1 = "Восстановление пароля";
        private static string _mailTitle2 = "Временный пароль пароль";
               private static string _Server = "smtp.timeweb.ru";
        private static string _Password = "p5fjwzqs77";
        public static int _port = 2525;

        public static void SendEmail(ExtPlayer player, string login)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var auntificationData = sessionData.AuntificationData;

                login = login.ToLower();

                if (!auntificationData.IsCreateAccount)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RecoveryCantFind), 4500);
                    Trigger.ClientEvent(player, "restorepassstep", 2);
                    return;
                }
                else if (!auntificationData.Login.ToLower().Equals(login) && !auntificationData.Email.ToLower().Equals(login))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RecoveryCantFind), 4500);
                    Trigger.ClientEvent(player, "restorepassstep", 2);
                    return;
                }
                else if (!auntificationData.Email.Contains("@"))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RecoveryEmailCant), 4500);
                    Trigger.ClientEvent(player, "restorepassstep", 2);
                    return;
                }
                sessionData.RecoveryCode = Generate.RandomOneTimePassword();
                if (sessionData.RecoveryCode == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RecoveryError), 5000);
                    Trigger.ClientEvent(player, "restorepassstep", 2);
                    return;
                }
                string _mailBody1 = $"<div style='padding-top: 30px;position: absolute; left: 0;top: 0;width:auto;height:auto;background:rgba(22,21,35); color: #fff; font-family: arial;overflow: hidden;padding-bottom: 40px'><div style='max-width: 1000px; height:auto; background: #12121e; padding-bottom: 10px;display: block;margin-left: auto;margin-right: auto'><header style='font-family: Arial; color: #fff; font-size: 40px; text-align: center;margin-top: -3vw; text-align: center;width: 100%;position: absolute'>Majestic Role Play</header><header style = 'position: relative; margin-left: 20px ;margin-top: 40px;font-size: 20px;color: #fff; max-width: 400px; padding-top: 10px;' ></header><header style = 'position: relative; margin-left: 20px;margin-top: 17px;font-size: 17px; max-width: 900px' > для завершения восстановления пароля на сервере <a style='color: #a4a4ef' HREF = 'https://vk.com/astrarpgta5'>Astra Project</a> вам необходимо ввести следующий код в меню авторизации</header><header style = 'position: relative;margin-left: 20px; margin-top: 20px;font-weight: 400;font-size: 30px;' > {sessionData.RecoveryCode}</header><header style = 'position: relative; margin-left: 20px;margin-top: 21px;font-size: 17px; max-width: 900px;color: #ffffff90;' > Вход был запрошен с этого IP: {sessionData.Address}. Если вы не отправляли этот запрос, необходимо как можно скорее поменять свой пароль.</header></div>";
                MailMessage msg;
                msg = new MailMessage(_mailFrom, auntificationData.Email, _mailTitle1, $"{_mailBody1}");
                msg.IsBodyHtml = true;


                SmtpClient smtpClient = new SmtpClient(_Server, _port)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_mailFrom, _Password),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                smtpClient.Send(msg);              
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Сообщение с кодом для восстановления пароля успешно выслано на {Generate.ObfuscateEmail(auntificationData.Email)}.", 5000);
                Trigger.ClientEvent(player, "restorepassstep", 1);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
        public static async void RecoveryPassword(ExtPlayer player, string code)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var auntificationData = sessionData.AuntificationData;
                
                if (!auntificationData.IsCreateAccount)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RecoveryCantFind), 4500);
                    Trigger.ClientEvent(player, "restorepassstep", 2);
                    return;
                }
                if (code == sessionData.RecoveryCode)
                {
                    Log.Debug($"{sessionData.RealSocialClub} удачно восстановил пароль!", nLog.Type.Info);
                    sessionData.RecoveryCode = null;
                   
                    string newPassword = Generate.RandomString(9);
                    if (newPassword == null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.RecoveryError), 5000);
                        Trigger.ClientEvent(player, "restorepassstep", 2);
                        return;
                    }

                    string _mailBody2 = $"<div style='padding-top: 30px;position: absolute; left: 0;top: 0;width:auto;height:auto;background:rgba(22,21,35); color: #fff; font-family: arial;overflow: hidden;padding-bottom: 40px'><div style='max-width: 1000px; height:auto; background: #12121e; padding-bottom: 10px;display: block;margin-left: auto;margin-right: auto'><header style='font-family: Arial; color: #fff; font-size: 40px; text-align: center;margin-top: -3vw; text-align: center;width: 100%;position: absolute'>Majestic Role Play, Ваш новый пароль: {newPassword} </header><header style = 'position: relative; margin-left: 20px ;margin-top: 40px;font-size: 20px;color: #fff; max-width: 400px; padding-top: 10px;' > Вы успешно восстановили свой пароль, приятной игры</header>";
                    MailMessage msg;
                    msg = new MailMessage(_mailFrom, auntificationData.Email, _mailTitle1, $"{_mailBody2}");
                    msg.IsBodyHtml = true;


                    SmtpClient smtpClient = new SmtpClient(_Server, _port)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(_mailFrom, _Password),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };


                    smtpClient.Send(msg);            

                    auntificationData.Password = Accounts.Repository.GetSha256(newPassword.ToString());

                    await using var db = new ServerBD("MainDB");//В отдельном потоке
                    await db.Accounts
                        .Where(v => v.Login == auntificationData.Login)
                        .Set(v => v.Password, auntificationData.Password)
                        .UpdateAsync();

                    Autorization.Repository.AutorizationAccount(player, auntificationData.Login, auntificationData.Password).Wait();

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно восстановили доступ к аккаунту. Новый пароль выслан на емайл!", 5000);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Помимо этого Вы можете изменить его прямо сейчас из игры введя /password и новый желаемый пароль через пробел. Пример [/password 123] без скобок.", 10000);
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CodeDoesntMatter), 4500);
                    Trigger.ClientEvent(player, "restorepassstep", 1);
                }
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}