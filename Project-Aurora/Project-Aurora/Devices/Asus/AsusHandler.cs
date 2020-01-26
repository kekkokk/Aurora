using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraServiceLib;
using Microsoft.Win32;
using System.Drawing;
using System.Diagnostics;

namespace Aurora.Devices.Asus
{
    public class GenericAsusDevice
    {
        public string name;
        public int id;
        protected readonly IAuraRgbLightCollection lights;
        protected readonly IAuraSyncDevice device;
        public bool busy = false;
        private readonly Dictionary<string, IAuraRgbLight> idToKey = new Dictionary<string, IAuraRgbLight>();
        private Color lastLogoColor;
        public GenericAsusDevice(IAuraSyncDevice thisDevice)
        {
            name = thisDevice.Name;
            id = thisDevice.GetHashCode();
            device = thisDevice;
            lights = thisDevice.Lights;
            foreach (IAuraRgbLight light in lights)
            {
                idToKey[light.Name] = light;
            }
        }

        public void applyColors(Dictionary<DeviceKeys, Color> keyColors)
        {

            if (busy)
                return;

            if (device.Name == "Armoury")
            {
                applyClaymoreColors(keyColors);
            }
            else
            {
                if (keyColors.TryGetValue(DeviceKeys.Peripheral_Logo, out var color))
                {
                    if (color != lastLogoColor)
                    {
                        applyGenericColors(keyColors);
                        lastLogoColor = color;
                    }
                }
            }
        }

        private void applyGenericColors(Dictionary<DeviceKeys, Color> keyColors)
        {
            if (keyColors.TryGetValue(DeviceKeys.Peripheral_Logo, out var color))
            {
                busy = true;
                foreach (IAuraRgbLight light in lights)
                {
                    lock (light)
                    {
                        light.Color = (uint)((color.A << 24) | (color.B << 16) | (color.G << 8) | (color.R << 0));
                    }
                }
            }
            busy = false;
            device.Apply();

        }

        private void applyClaymoreColors(Dictionary<DeviceKeys, Color> keyColors)
        {
            //var stop = new Stopwatch();

            busy = true;
            foreach (var keyColor in keyColors)
            {
                var deviceKey = keyColor.Key;
                var color = keyColor.Value;

                var keyId = DeviceKeyToAuraKeyboardKeyIdNumpadRight(deviceKey);
                if (keyId == null)
                    continue;

                lock (idToKey[keyId])
                {
                    idToKey[keyId].Color = (uint)((color.A << 24) | (color.B << 16) | (color.G << 8) | (color.R << 0));
                }
            }

            //Log($"Took {totalTimeConvert} to convert and {totalTimeApplying} to apply, should be {stop2.ElapsedMilliseconds} instead is {totalTimeApplying + totalTimeConvert}");
            busy = false;
            device.Apply();

        }

        private static string DeviceKeyToAuraKeyboardKeyIdNumpadRight(DeviceKeys key)
        {
            switch (key)
            {
                case DeviceKeys.ESC:
                    return "Armoury_1";
                case DeviceKeys.F1:
                    return "Armoury_3";
                case DeviceKeys.F2:
                    return "Armoury_4";
                case DeviceKeys.F3:
                    return "Armoury_5";
                case DeviceKeys.F4:
                    return "Armoury_6";
                case DeviceKeys.F5:
                    return "Armoury_8";
                case DeviceKeys.F6:
                    return "Armoury_9";
                case DeviceKeys.F7:
                    return "Armoury_10";
                case DeviceKeys.F8:
                    return "Armoury_11";
                case DeviceKeys.F9:
                    return "Armoury_13";
                case DeviceKeys.F10:
                    return "Armoury_14";
                case DeviceKeys.F11:
                    return "Armoury_15";
                case DeviceKeys.F12:
                    return "Armoury_16";
                case DeviceKeys.PRINT_SCREEN:
                    return "Armoury_17";
                case DeviceKeys.SCROLL_LOCK:
                    return "Armoury_18";
                case DeviceKeys.PAUSE_BREAK:
                    return "Armoury_19";
                //case DeviceKeys.OEM5:
                //    return 6;
                case DeviceKeys.TILDE:
                    return "Armoury_24";
                case DeviceKeys.ONE:
                    return "Armoury_25";
                case DeviceKeys.TWO:
                    return "Armoury_26";
                case DeviceKeys.THREE:
                    return "Armoury_27";
                case DeviceKeys.FOUR:
                    return "Armoury_28";
                case DeviceKeys.FIVE:
                    return "Armoury_29";
                case DeviceKeys.SIX:
                    return "Armoury_30";
                case DeviceKeys.SEVEN:
                    return "Armoury_31";
                case DeviceKeys.EIGHT:
                    return "Armoury_32";
                case DeviceKeys.NINE:
                    return "Armoury_33";
                case DeviceKeys.ZERO:
                    return "Armoury_34";
                case DeviceKeys.MINUS:
                    return "Armoury_35";
                case DeviceKeys.EQUALS:
                    return "Armoury_36";
                //case DeviceKeys.OEM6:
                //    return 7;
                case DeviceKeys.BACKSPACE:
                    return "Armoury_37";
                case DeviceKeys.INSERT:
                    return "Armoury_40";
                case DeviceKeys.HOME:
                    return "Armoury_41";
                case DeviceKeys.PAGE_UP:
                    return "Armoury_42";
                case DeviceKeys.NUM_LOCK:
                    return "Armoury_43";
                case DeviceKeys.NUM_SLASH:
                    return "Armoury_44";
                case DeviceKeys.NUM_ASTERISK:
                    return "Armoury_45";
                case DeviceKeys.NUM_MINUS:
                    return "Armoury_46";
                case DeviceKeys.TAB:
                    return "Armoury_47";
                case DeviceKeys.Q:
                    return "Armoury_48";
                case DeviceKeys.W:
                    return "Armoury_49";
                case DeviceKeys.E:
                    return "Armoury_50";
                case DeviceKeys.R:
                    return "Armoury_51";
                case DeviceKeys.T:
                    return "Armoury_52";
                case DeviceKeys.Y:
                    return "Armoury_53";
                case DeviceKeys.U:
                    return "Armoury_54";
                case DeviceKeys.I:
                    return "Armoury_55";
                case DeviceKeys.O:
                    return "Armoury_56";
                case DeviceKeys.P:
                    return "Armoury_57";
                //case DeviceKeys.OEM1:
                //    return 2;
                case DeviceKeys.OPEN_BRACKET:
                    return "Armoury_58";
                //case DeviceKeys.OEMPlus:
                //    return 13;
                case DeviceKeys.CLOSE_BRACKET:
                    return "Armoury_59";
                //case DeviceKeys.BACKSLASH:
                //    return 43;
                case DeviceKeys.DELETE:
                    return "Armoury_63";
                case DeviceKeys.END:
                    return "Armoury_64";
                case DeviceKeys.PAGE_DOWN:
                    return "Armoury_65";
                case DeviceKeys.NUM_SEVEN:
                    return "Armoury_66";
                case DeviceKeys.NUM_EIGHT:
                    return "Armoury_67";
                case DeviceKeys.NUM_NINE:
                    return "Armoury_68";
                case DeviceKeys.NUM_PLUS:
                    return "Armoury_69";
                case DeviceKeys.CAPS_LOCK:
                    return "Armoury_70";
                case DeviceKeys.A:
                    return "Armoury_71";
                case DeviceKeys.S:
                    return "Armoury_72";
                case DeviceKeys.D:
                    return "Armoury_73";
                case DeviceKeys.F:
                    return "Armoury_74";
                case DeviceKeys.G:
                    return "Armoury_75";
                case DeviceKeys.H:
                    return "Armoury_76";
                case DeviceKeys.J:
                    return "Armoury_77";
                case DeviceKeys.K:
                    return "Armoury_78";
                case DeviceKeys.L:
                    return "Armoury_79";
                //case DeviceKeys.OEMTilde:
                //    return 41;
                case DeviceKeys.SEMICOLON:
                    return "Armoury_80";
                case DeviceKeys.APOSTROPHE:
                    return "Armoury_81";
                case DeviceKeys.HASHTAG:
                    return "Armoury_82";
                case DeviceKeys.ENTER:
                    return "Armoury_83";
                case DeviceKeys.NUM_FOUR:
                    return "Armoury_89";
                case DeviceKeys.NUM_FIVE:
                    return "Armoury_90";
                case DeviceKeys.NUM_SIX:
                    return "Armoury_91";
                case DeviceKeys.LEFT_SHIFT:
                    return "Armoury_93";
                case DeviceKeys.BACKSLASH_UK:
                    return "Armoury_94";
                case DeviceKeys.Z:
                    return "Armoury_95";
                case DeviceKeys.X:
                    return "Armoury_96";
                case DeviceKeys.C:
                    return "Armoury_97";
                case DeviceKeys.V:
                    return "Armoury_98";
                case DeviceKeys.B:
                    return "Armoury_99";
                case DeviceKeys.N:
                    return "Armoury_100";
                case DeviceKeys.M:
                    return "Armoury_101";
                case DeviceKeys.COMMA:
                    return "Armoury_102";
                case DeviceKeys.PERIOD:
                    return "Armoury_103";
                case DeviceKeys.FORWARD_SLASH:
                    return "Armoury_104";
                //case DeviceKeys.OEM8:
                //    return 9; 105
                case DeviceKeys.RIGHT_SHIFT:
                    return "Armoury_106";
                case DeviceKeys.ARROW_UP:
                    return "Armoury_110";
                case DeviceKeys.NUM_ONE:
                    return "Armoury_112";
                case DeviceKeys.NUM_TWO:
                    return "Armoury_113";
                case DeviceKeys.NUM_THREE:
                    return "Armoury_114";
                case DeviceKeys.NUM_ENTER:
                    return "Armoury_115";
                case DeviceKeys.LEFT_CONTROL:
                    return "Armoury_116";
                case DeviceKeys.LEFT_WINDOWS:
                    return "Armoury_117";
                case DeviceKeys.LEFT_ALT:
                    return "Armoury_118";
                case DeviceKeys.SPACE:
                    return "Armoury_120";
                case DeviceKeys.RIGHT_ALT:
                    return "Armoury_125";
                case DeviceKeys.APPLICATION_SELECT:
                    return "Armoury_127";
                case DeviceKeys.RIGHT_CONTROL:
                    return "Armoury_129";
                case DeviceKeys.ARROW_LEFT:
                    return "Armoury_132";
                case DeviceKeys.ARROW_DOWN:
                    return "Armoury_133";
                case DeviceKeys.ARROW_RIGHT:
                    return "Armoury_134";
                case DeviceKeys.NUM_ZERO:
                    return "Armoury_135";
                case DeviceKeys.NUM_PERIOD:
                    return "Armoury_137";
                case DeviceKeys.FN_Key:
                    return "Armoury_126";
                case DeviceKeys.LOGO:
                    return "Armoury_124";
                default:
                    return null;
            }
        }

        public static void Log(string text)
        {
            Global.logger.Info($"[ASUS] {text}");
        }
    }

    public class AsusHandler
    {
        private const string AuraSdkRegistryEntry = @"{05921124-5057-483E-A037-E9497B523590}\InprocServer32";
        private IAuraSdk sdk;
        private readonly List<GenericAsusDevice> devices = new List<GenericAsusDevice>();
        private bool initialized = false;

        private bool isAuraServiceInstalled()
        {
            Global.logger.Error("[ASUS] Initializing Asus Handler, searching for AuraService in registry");
            var registryClassKey = Registry.ClassesRoot.OpenSubKey("CLSID");
            if (registryClassKey != null)
            {
                var auraSdkRegistry = registryClassKey.OpenSubKey(AuraSdkRegistryEntry);
                if (auraSdkRegistry != null)
                {
                    Global.logger.Error("[ASUS] Aura SDK found!");
                    return true;
                }
            }
            return false;
        }

        public bool GetControl(bool async)
        {
            if (!isAuraServiceInstalled())
            {
                //onComplete?.Invoke(false);
                return false;
            }

            if (initialized)
            {
                return true;
            }

            sdk = new AuraSdk();
            sdk.SwitchMode();

            IAuraSyncDeviceCollection auraDevices = sdk.Enumerate(0);
            foreach (IAuraSyncDevice auraDevice in auraDevices)
            {
                try
                {
                    if ((async && auraDevice.Name == "Armoury") || !async)
                    {
                        devices.Add(new GenericAsusDevice(auraDevice));
                    }
                }
                catch (Exception e)
                {
                    Log(e.StackTrace);
                }
            }

            Log($"Enumerated devices {devices.Count}");
            initialized = true;
            //onComplete?.Invoke(true);
            return true;
        }

        public void UpdateDevices(object keyColorsObject)
        {
            if (devices == null || devices.Count == 0 || !initialized)
                return;

            foreach (GenericAsusDevice device in devices)
            {
                device.applyColors((Dictionary<DeviceKeys, Color>)keyColorsObject);
            }
        }

        public string GetDeviceStatus()
        {
            var status = new StringBuilder();
            foreach (var device in devices)
            {
                status.Append($"{device.name} active " +
                    $"" +
                    $"");
            }
            return status.ToString();
        }

        public void ReleaseControl()
        {
            // Do this async because it may take a while to uninitialize
            devices.Clear();
            initialized = false;
            ((IAuraSdk2)sdk).ReleaseControl(0);
        }


        private enum AsusDeviceType : uint
        {
            All = 0x00000000,
            Motherboard = 0x00010000,
            MotherboardLedStrip = 0x00011000,
            AllInOnePc = 0x00012000,
            Vga = 0x00020000,
            Display = 0x00030000,
            Headset = 0x00040000,
            Microphone = 0x00050000,
            ExternalHdd = 0x00060000,
            ExternalBdDrive = 0x00061000,
            Dram = 0x00070000,
            Keyboard = 0x00080000,
            NotebookKeyboard = 0x00081000,
            NotebookKeyboard4ZoneType = 0x00081001,
            Mouse = 0x00090000,
            Chassis = 0x000B0000,
            Projector = 0x000C0000,
        }

        public static void Log(string text)
        {
            Global.logger.Info($"[ASUS] {text}");
        }

    }
}
