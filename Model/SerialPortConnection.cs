using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Threading;

namespace M5.KernScaleTest.Model
{
    public delegate void ScaleEventHandler(object source, ScaleEventArgs e);

    public class ScaleEventArgs : EventArgs
    {
        private string eventInfo;
        public ScaleEventArgs(string Text)
        {
            eventInfo = Text;
        }
        public string GetInfo()
        {
            return eventInfo;
        }
    }

    public class SerialPortConnection
    {
        #region Constructor
        public SerialPortConnection()
        {
            InitializePort();
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
        private SerialPort _port = new SerialPort();
        private string _comPort = "COM5";
        private int _weightCount = 10;
        private string _weightResult = string.Empty;
        private List<string> _weightResults = new List<string>();
        private bool _dataReceived = false;
        private int _workingCounter = 0;
        #endregion

        #region Properties
        public string ComPort
        {
            get => _comPort;
            set
            {
                _comPort = value;
                _port.PortName = _comPort;
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
            //for (int i = 0; i < _weightCount; i++)
            //{
                //_workingCounter = i;
                if (!_port.IsOpen)
                    _port.Open();
                _port.Write(scaleCommand);
                _dataReceived = false;
                do
                {
                    // no action waiting of scale result
                } while (!_dataReceived);
            //Thread.Sleep(100);
            //PortDataReceived();
            //_weightResults.Add(_weightResult);
            //_port.Close();
            //}  
            AllDataReceived(this, new ScaleEventArgs("All data processed"));
        }
        #endregion

        #region Private Methods
        private void InitializePort()
        {
            _port.PortName = _comPort;
            _port.BaudRate = 9600;
            _port.Parity = Parity.None;
            _port.DataBits = 8;
            _port.StopBits = StopBits.One;
            _port.ReadTimeout = 2000;
            _port.WriteTimeout = 2000;
            _port.DataReceived += _port_DataReceived;
            _port.PinChanged += _port_PinChanged;
            _port.ErrorReceived += _port_ErrorReceived;
            _port.Disposed += _port_Disposed;
        }

        private void _port_Disposed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void _port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void _port_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

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

        #region EventHandle
        public event ScaleEventHandler AllDataReceived;

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int intBuffer;
            intBuffer = _port.BytesToRead;
            byte[] byteBuffer = new byte[intBuffer];
            _port.Read(byteBuffer, 0, intBuffer);
            PortDataReceived();
            _weightResults.Add(_weightResult);
            _dataReceived = true;
        }
        #endregion
    }
}
