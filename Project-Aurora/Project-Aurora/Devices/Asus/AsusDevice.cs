using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Settings;

namespace Aurora.Devices.Asus
{
    /// <summary>
    /// Acts as the interfacing class to Aurora
    /// </summary>
    public class AsusDevice : Device
    {
        private const string DeviceName = "Asus";
        private AuraState _state = AuraState.Off;
        private SocketServer socket;
        private AuraState State
        {
            get => _state;
            set
            {
                Log($"Status { _state} -> {value}");
                _state = value;
            }
        }

        private VariableRegistry _defaultRegistry;
        private AsusHandler _asusHandler;
        private bool _runAsync;

        /// <inheritdoc />
        public string GetDeviceName() => DeviceName;

        /// <inheritdoc />
        public string GetDeviceDetails()
        {
            var result = $"{DeviceName}: ";
            switch (State)
            {
                case AuraState.Off:
                    result += "Not initialized";
                    break;
                case AuraState.NotFound:
                    result += "Aura SDK not installed";
                    break;
                case AuraState.Stopping:
                    result += "Stopping";
                    break;
                case AuraState.Starting:
                    result += "Connecting";
                    break;
                case AuraState.On:
                    result += "Connected. Devices: " + _asusHandler?.GetDeviceStatus();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        /// <inheritdoc />
        public string GetDeviceUpdatePerformance() => "";

        /// <inheritdoc />
        public VariableRegistry GetRegisteredVariables()
        {
            if (_defaultRegistry != null) return _defaultRegistry;

            _defaultRegistry = new VariableRegistry();
            _defaultRegistry.Register($"{DeviceName}_async", false, "Sync only Keyboard");
            return _defaultRegistry;
        }

        /// <inheritdoc />
        public bool Initialize()
        {
            if (State == AuraState.Starting || State == AuraState.On || State == AuraState.Stopping)
                return true;

            socket = new SocketServer();
            socket.Start();
            State = AuraState.On;
            return true;



            //if (_asusHandler == null)
            //    _asusHandler = new AsusHandler();

            //State = AuraState.Starting;

            //_runAsync = Global.Configuration.VarRegistry.GetVariable<bool>($"{DeviceName}_async");
            //bool initialized = _asusHandler.GetControl(_runAsync);
            //if (initialized)
            //{
            //    State = AuraState.On;
            //}
            //return initialized;
            //return _asusHandler.GetControl(_runAsync, success => State = success ? AuraState.On : State = AuraState.Off);
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            StopAsus();
        }

        /// <inheritdoc />
        public void Reset()
        {
            StopAsus();
            Initialize();
        }

        private void StopAsus()
        {
            if (State == AuraState.Off || State == AuraState.Stopping || State == AuraState.Starting)
                return;

            State = AuraState.Stopping;
            _asusHandler.ReleaseControl();
            State = AuraState.Off;
        }

        /// <inheritdoc />
        public bool Reconnect()
        {
            return false;
        }

        /// <inheritdoc />
        public bool IsInitialized() => IsConnected();
        /// <inheritdoc />
        public bool IsConnected() => State == AuraState.On || State == AuraState.Starting || State == AuraState.Stopping;

        /// <inheritdoc />
        public bool IsKeyboardConnected() => IsConnected();

        /// <inheritdoc />
        public bool IsPeripheralConnected() => IsConnected();

        /// <inheritdoc />
        public bool UpdateDevice(DeviceColorComposition colorComposition, DoWorkEventArgs e, bool forced = false)
        {
            if (socket != null)
            {
                socket.Send(colorComposition.keyColors);
            }
            //Task.Run(() =>
            //{
            //_asusHandler.UpdateDevices(colorComposition.keyColors);
            //});
            return true;
        }

        public static void Log(string text)
        {
            Global.logger.Info($"[ASUS] {text}");
        }

        public bool UpdateDevice(Dictionary<DeviceKeys, Color> keyColors, DoWorkEventArgs e, bool forced = false)
        {
            Task.Run(() =>
            {
                _asusHandler.UpdateDevices(keyColors);
            });
            return true;
        }

        private enum AuraState
        {
            Off,
            Starting,
            On,
            Stopping,
            NotFound
        }
    }
}
