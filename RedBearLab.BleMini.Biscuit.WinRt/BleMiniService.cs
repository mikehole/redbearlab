using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace RedBearLab.BleMini.Biscuit.WinRt
{
    public class BleMiniService : IBleMiniService
    {
        GattDeviceService service;

        GattCharacteristic tx;
        GattCharacteristic rx;

        Action<uint, DataReader, Exception> _dataRecievedCallback;

        public bool Connected { get; set; }

        public async void Init(Action<uint, DataReader, Exception> dataRecievedCallback)
        {
            _dataRecievedCallback = dataRecievedCallback;

            var devices = await DeviceInformation.FindAllAsync(
                GattDeviceService.GetDeviceSelectorFromUuid(Guid.Parse("713D0000-503E-4C75-BA94-3148F18D941E")),
                new string[] { "System.Devices.ContainerId" });

            if (devices != null)
            {
                service = await GattDeviceService.FromIdAsync(devices[0].Id);

                if (service != null)
                {
                    rx = service.GetCharacteristics(Guid.Parse("713D0002-503E-4C75-BA94-3148F18D941E"))[0];
                    tx = service.GetCharacteristics(Guid.Parse("713D0003-503E-4C75-BA94-3148F18D941E"))[0];

                    rx.ValueChanged += rx_ValueChanged;
                    var res = await rx.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                }
            }
        }

        public void rx_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            try
            {
                _dataRecievedCallback(args.CharacteristicValue.Length, DataReader.FromBuffer(args.CharacteristicValue), null);
            }
            catch (Exception error)
            {
                _dataRecievedCallback(0, null, error);
            }
        }

        public async void SendData(DataWriter writer, Action<GattCommunicationStatus, Exception> callback)
        {
            try
            {
                var status = await tx.WriteValueAsync(writer.DetachBuffer(), GattWriteOption.WriteWithoutResponse);
                callback(status, null);
            }
            catch (Exception error)
            {
                callback(GattCommunicationStatus.Unreachable, error);
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
