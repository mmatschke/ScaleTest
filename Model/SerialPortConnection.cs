using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Threading;

namespace M5.KernScaleTest.Model
{
    public class SerialPortConnection
    {
        #region Constructor
        public SerialPortConnection()
        {

        }

        public SerialPortConnection(string comPort, int weightCount)
        {
            if (string.IsNullOrEmpty(comPort))
                this.ComPort = comPort;
            _weightCount = weightCount;
        }
        #endregion

        #region Fields
        // Create the serial port with basic settings
        private SerialPort _port = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One);
        private string _comPort = "COM5";
        private int _weightCount = 10;
        private string _weightResult = string.Empty;
        private List<string> _weightResults = new List<string>();
        #endregion

        #region Properties
        public string ComPort
        {
            get => _comPort;
            set
            {
                _comPort = value;
                _port = new SerialPort(value, 9600, Parity.Even, 8, StopBits.One);
            }
        }

        public int WeightCount { get => _weightCount; set => _weightCount = value; }
        public string WeightResult { get => _weightResult; set => _weightResult = value; }
        public List<string> WeightResults { get => _weightResults; set => _weightResults = value; }
        #endregion

        #region Public Methods
        public void TarScale()
        {
            if (!_port.IsOpen)
                _port.Open();
            _port.Write("T");
        }

        public void GetScaleValue(string scaleCommand)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(scaleCommand))
                return;            
            _weightResults = new List<string>();
            for (int i = 0; i < _weightCount; i++)
            {
                if (!_port.IsOpen)
                    _port.Open();
                _port.Write(scaleCommand);
                Thread.Sleep(100);
                PortDataReceived();
                _weightResults.Add(_weightResult);
                _port.Close();
            }                                               
        }
        #endregion

        #region Private Methods
        private void PortDataReceived()
        {
            string result;
            result = _port.ReadExisting();
            GetWeightResults(result);
        }

        private void GetWeightResults(string weightResult)
        {
            if(!string.IsNullOrEmpty(weightResult))
            {
                if (weightResult.StartsWith("ST"))
                    _weightResult = weightResult.Substring(6).Trim().Replace(".", ",");
                else
                    _weightResult = weightResult;
            }
            else
                _weightResult = "Result is null or empty | 0,0 g";
        }
        #endregion
    }
}
