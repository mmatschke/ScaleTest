using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace M5.KernScaleTest.Model
{
    public delegate void ScaleEventHandlerNew(object source, ScaleEventArgs e);

    public class ScaleEventArgsNew : EventArgs
    {
        private string eventInfo;
        public ScaleEventArgsNew(string Text)
        {
            eventInfo = Text;
        }
        public string GetInfo()
        {
            return eventInfo;
        }
    }

    public class SerialPortConnectNew
    {
        private static bool _continue;
        private static SerialPort _port;
        private string _comPort = "COM5";
        private int _weightCount = 10;
        private static string _weightResult = string.Empty;
        private List<string> _weightResults = new List<string>();
        private int _amountWeights = 0;

        public event ScaleEventHandler AllDataReceived;

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
        public int AmountWeights { get => _amountWeights; set => _amountWeights = value; }

        public SerialPortConnectNew()
        {
            _port = new SerialPort();
            InitializePort();
        }

        public SerialPortConnectNew(string comPort, int weightCount)
            :base()
        {
            if (string.IsNullOrEmpty(comPort))
                this.ComPort = comPort;
            _weightCount = weightCount;
        }

        public void GetScaleValue(string scaleCommand)
        {
            Thread readThread = new Thread(Read);
            _continue = true;
            _amountWeights = 0;
            if (string.IsNullOrEmpty(scaleCommand))
                return;
            _weightResults = new List<string>();
            if (!_port.IsOpen)
                _port.Open();
            readThread.Start();
            while (_continue)
            {
                if(_weightResult != string.Empty)
                {
                    //ToDo: check scale value
                    if (_weightResult.StartsWith("ST"))
                    {
                        GetWeightResults(_weightResult);
                        _weightResults.Add(_weightResult);
                        _continue = false;
                    }
                    else
                    {
                        _weightResult = string.Empty;
                    }
                }
                else
                {
                    _amountWeights++;
                    _port.Write(scaleCommand);
                }
            }
            readThread.Join();
            _port.Close();
            AllDataReceived(this, new ScaleEventArgs("All data processed"));
        }

        private void InitializePort()
        {
            _port.PortName = _comPort;
            _port.BaudRate = 9600;
            _port.Parity = Parity.None;
            _port.DataBits = 8;
            _port.StopBits = StopBits.One;
            _port.ReadTimeout = 500;
            _port.WriteTimeout = 500;
        }


        private void GetWeightResults(string weightResult)
        {
            if (!string.IsNullOrEmpty(weightResult))
            {
                if (weightResult.StartsWith("ST"))
                {
                    _weightResult = weightResult.Substring(5).Trim().Replace(".", ",");
                    if (_weightResult.StartsWith("-"))
                        _weightResult = $"{_weightResult.Substring(0, 1)}{_weightResult.Substring(1).Trim()}";
                    else
                        _weightResult = _weightResult.Substring(1).Trim();
                }                    
                else
                    _weightResult = weightResult;
            }
            else
                _weightResult = "Result is null or empty | 0,0 g";
        }

        public static void Read()
        {
            while (_continue)
            {
                try
                {
                    if(string.IsNullOrEmpty(_weightResult))
                    {
                        string message = _port.ReadLine();
                        if (!string.IsNullOrEmpty(message))
                        {
                            _weightResult = message;
                        }
                    }
                }
                catch (TimeoutException) { }
            }
        }
    }    
}
