using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json; // ← ДОБАВЬ

namespace NeptuneEvo.Character.Config.Models
{
    public class ChatData
    {
        // Старые поля (не трогаем для совместимости)
        public bool Timestamp { get; set; } = false;
        public bool ChatShadow { get; set; } = true;
        public byte Fontsize { get; set; } = 16;
        public byte ChatOpacity { get; set; } = 100;
        public byte Chatalpha { get; set; } = 1;
        public byte Pagesize { get; set; } = 10;
        public byte Widthsize { get; set; } = 100;
        public byte Transition { get; set; } = 0;
        public byte WalkStyle { get; set; } = 0;
        public byte FacialEmotion { get; set; } = 0;
        public bool Deaf { get; set; } = false;
        public bool TagsHead { get; set; } = true;

        public bool HudToggled { get; set; } = true;
        public bool HudStats { get; set; } = true;
        public bool HudSpeed { get; set; } = true;
        public bool HudOnline { get; set; } = true;
        public bool HudLocation { get; set; } = true;
        public bool HudKey { get; set; } = true;
        public bool HudMap { get; set; } = true;

        public byte VolumeInterface { get; set; } = 100;
        public byte VolumeQuest { get; set; } = 50;
        public byte VolumeAmbient { get; set; } = 50;
        public byte VolumePhoneRadio { get; set; } = 50;
        public byte VolumeVoice { get; set; } = 100;
        public byte VolumeRadio { get; set; } = 70;
        public byte DistancePlayer { get; set; } = 50;
        public byte DistanceVehicle { get; set; } = 50;
        public bool cPToggled { get; set; } = false;
        public byte cPWidth { get; set; } = 2;
        public byte cPGap { get; set; } = 2;
        public bool cPDot { get; set; } = true;
        public byte cPThickness { get; set; } = 0;
        public byte cPColorR { get; set; } = 255;
        public byte cPColorG { get; set; } = 255;
        public byte cPColorB { get; set; } = 255;
        public byte cPOpacity { get; set; } = 9;
        public bool cPCheck { get; set; } = true;
        public bool FirstMute { get; set; } = false;
        public bool APunishments { get; set; } = false;
        public bool CircleVehicle { get; set; } = true;
        public byte cEfValue { get; set; } = 0;
        public byte notifCount { get; set; } = 2;
        public float RadioVolume { get; set; } = 15;
        public bool hitPoint { get; set; } = false;
        [JsonProperty("speedoType")]
        public string SpeedoType { get; set; } = "Minimalistic_2";
        [JsonProperty("schat")]
        public bool Schat { get; set; } = true;

        [JsonProperty("hud")]
        public bool Hud { get; set; } = true;

        [JsonProperty("radar")]
        public bool Radar { get; set; } = true;

        [JsonProperty("ids")]
        public bool Ids { get; set; } = true;

        [JsonProperty("compass")]
        public bool Compass { get; set; } = true;

        [JsonProperty("displayChatMsg")]
        public bool DisplayChatMsg { get; set; } = true;

        [JsonProperty("schatPunish")]
        public bool SchatPunish { get; set; } = true;
        // ← НОВЫЕ ПОЛЯ С JsonProperty ДЛЯ МАЛЕНЬКИХ БУКВ
        [JsonProperty("chatSettings")]
        public string ChatSettings { get; set; } = "standard";

        [JsonProperty("chatTheme")]
        public string ChatTheme { get; set; } = "Clean";

        [JsonProperty("chatFont")]
        public string ChatFont { get; set; } = "ProximaNova";

        [JsonProperty("chatFontStyle")]
        public string ChatFontStyle { get; set; } = "Bold";

        [JsonProperty("chatBorderType")]
        public string ChatBorderType { get; set; } = "Stroke";

        [JsonProperty("chatTimestamp")]
        public string ChatTimestamp { get; set; } = "DateTime";

        [JsonProperty("chatFontSize")]
        public byte ChatFontSize { get; set; } = 14;

        [JsonProperty("chatFontShadow")]
        public byte ChatFontShadow { get; set; } = 0;

        [JsonProperty("chatFontShadowAlpha")]
        public byte ChatFontShadowAlpha { get; set; } = 0;

        [JsonProperty("chatLinesMargin")]
        public byte ChatLinesMargin { get; set; } = 0;

        [JsonProperty("chatLineHeight")]
        public byte ChatLineHeight { get; set; } = 0;

        [JsonProperty("chatWidth")]
        public ushort ChatWidth { get; set; } = 500;

        [JsonProperty("chatHeight")]
        public ushort ChatHeight { get; set; } = 500;

        // Быстрые команды
        [JsonProperty("chatCmdsSettings")]
        public string ChatCmdsSettings { get; set; } = "standard";

        [JsonProperty("chatCmdsFont")]
        public string ChatCmdsFont { get; set; } = "ProximaNova";

        [JsonProperty("chatCmdsFontStyle")]
        public string ChatCmdsFontStyle { get; set; } = "Regular";

        [JsonProperty("chatCmdsFontSize")]
        public byte ChatCmdsFontSize { get; set; } = 12;

        [JsonProperty("chatCmdsOrder")]
        public string ChatCmdsOrder { get; set; } = "/report,/b,/me,/do,/try,/s,/w,/m,/f,/fb,/c,/cb,/dep,/mark,/mark2,/gnews,/wnews,/adv,/vmute,/pay";
    }
}