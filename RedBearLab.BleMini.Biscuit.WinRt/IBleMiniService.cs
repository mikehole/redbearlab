using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace RedBearLab.BleMini.Biscuit.WinRt
{
    public interface IBleMiniService : INotifyPropertyChanged
    {
        bool Connected { get; set; }

        void Init(Action<uint, DataReader, Exception> dataRecievedCallback);

        void SendData(DataWriter writer, Action<GattCommunicationStatus, Exception> callback);
    }
}
