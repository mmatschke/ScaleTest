using M5.KernScaleTest.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace M5.KernScaleTest.ViewModel
{
    public class WeightResultViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        #region Constructor
        public WeightResultViewModel()
        {
            try
            {
                _serialPortConnection = new SerialPortConnectNew();
                _serialPortConnection.AllDataReceived += _serialPortConnection_AllDataReceived;
            }
            catch (Exception exp)
            {
                MessageBox.Show($"Fehler: {exp.Message}");
            }
        }
        #endregion

        #region Fields
        private System.Collections.ObjectModel.ObservableCollection<Model.WeightResult> _weightResults = new System.Collections.ObjectModel.ObservableCollection<Model.WeightResult>();
        private ICommand _clickCommand;
         private bool _canExecute = true;
        private int _amountWeights = 1;
        private string _comInterface = "COM5";
        private long _runningTime = 0;
        private int _countRepeats = 1;
        private float _waitTime = 0.5f;
        private int _currentNumber = 0;
        private SerialPortConnectNew _serialPortConnection;
        StopWatchLogger _stopWatchLogger;
        #endregion

        #region Properties
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() => GetResultCommand(), _canExecute));
            }
        }

        public ObservableCollection<WeightResult> WeightResults
        {
            get { return _weightResults; }
            set { _weightResults = value; RaisePropertyChanged("WeightResults"); }
        }

        public int AmountWeights { get => _amountWeights; set => _amountWeights = value; }
        public string ComInterface { get => _comInterface; set { _comInterface = value; _serialPortConnection.ComPort = _comInterface; } }
        public long RunningTime { get => _runningTime; set => _runningTime = value; }
        public int CountRepeats { get => _countRepeats; set => _countRepeats = value; }
        public float WaitTime { get => _waitTime; set => _waitTime = value; }
        public int CurrentNumber { get => _currentNumber; set => _currentNumber = value; }
        #endregion

        #region Public Methods
        public void GetResultCommand()
        {
            try
            {
                _currentNumber++;
                if (string.IsNullOrEmpty(_comInterface))
                {
                    MessageBox.Show("Bitte prüfen Sie, ob die COM Schnittstelle angegeben ist.");
                    return;
                }
                Random random = new Random();
                _amountWeights = random.Next(1, 10);
                if (_amountWeights < 1)
                    _amountWeights = 1;
                if(_serialPortConnection == null)
                    _serialPortConnection = new SerialPortConnectNew(_comInterface, _amountWeights);
                _stopWatchLogger = new StopWatchLogger();
                _stopWatchLogger.Start();
                _serialPortConnection.GetScaleValue("W");                
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            SaveResults();
        }
        #endregion

        #region Private Methods
        private void SaveResults()
        {
            StreamWriter writer;
            string path = "c:\\KernScaleTest";
            string fileName = "KernScaleTest.csv";
            string fullFileName = Path.Combine(path, fileName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!File.Exists(fullFileName))
            {
                var strHeadLine = new StringBuilder();
                strHeadLine.Append("Lfd. Nr." + ";");
                strHeadLine.Append("Datum" + ";");
                strHeadLine.Append("Laufzeit (ms)" + ";");
                strHeadLine.Append("Anzahl Wiederholungen" + ";");
                strHeadLine.Append("Ergebnis" + ";");
                writer = File.CreateText(fullFileName);
                writer.WriteLine(strHeadLine);
                writer.Close();
            }
            if(_serialPortConnection != null)
            {
                for (int i = 0; i < _serialPortConnection.WeightResults.Count; i++)
                {
                    SaveOneLine(i, _serialPortConnection.WeightResults[i], fullFileName);
                }
            }
            SaveOneLine(_serialPortConnection.AmountWeights, _weightResults.Last().Result, fullFileName);
        }

        private void SaveOneLine(int amount, string result, string fileName)
        {
            StreamWriter writer;
            var strTextLine = new StringBuilder();
            strTextLine.Append(_currentNumber + ";");
            strTextLine.Append(DateTime.Now + ";");
            strTextLine.Append(_runningTime + ";");
            strTextLine.Append(amount + ";");
            strTextLine.Append(result + ";");
            writer = File.AppendText(fileName);
            writer.WriteLine(strTextLine);
            writer.Close();
        }
        #endregion

        #region INotifyPropertyChanged
        /// <summary>
        /// The PropertyChanged event is used by consuming code
        /// (like WPF's binding infrastructure) to detect when
        /// a value has changed.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise the PropertyChanged event for the 
        /// specified property.
        /// </summary>
        /// <param name="propertyName">
        /// A string representing the name of 
        /// the property that changed.</param>
        /// <remarks>
        /// Only raise the event if the value of the property 
        /// has changed from its previous value</remarks>
        protected void RaisePropertyChanged(string propertyName)
        {
            // Validate the property name in debug builds
            VerifyProperty(propertyName);

            if (null != PropertyChanged)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Verifies whether the current class provides a property with a given
        /// name. This method is only invoked in debug builds, and results in
        /// a runtime exception if the <see cref="OnPropertyChanged"/> method
        /// is being invoked with an invalid property name. This may happen if
        /// a property's name was changed but not the parameter of the property's
        /// invocation of <see cref="OnPropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        private void VerifyProperty(string propertyName)
        {
            try
            {
                Type type = this.GetType();

                // Look for a *public* property with the specified name
                System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
                if (pi == null)
                {
                    // There is no matching property - notify the developer
                    string msg = "OnPropertyChanged was invoked with invalid " +
                                    "property name {0}. {0} is not a public " +
                                    "property of {1}.";
                    msg = String.Format(msg, propertyName, type.FullName);
                    System.Diagnostics.Debug.Fail(msg);
                }
            }
            catch (Exception exp)
            {
                // no action
            }
        }
        #endregion

        #region EventHandling
        private void _serialPortConnection_AllDataReceived(object sender, EventArgs e)
        {
            _stopWatchLogger.Stop();
            _runningTime = _stopWatchLogger.GetRunningTime;
            WeightResults.Add(new WeightResult
            {
                Date = DateTime.Now,
                AmountWeights = _amountWeights,
                RunningTime = _runningTime,
                Result = _serialPortConnection.WeightResult,
                CurrentNumber = _currentNumber.ToString("0000")
            });
            RaisePropertyChanged("WeightResults");
            RaisePropertyChanged("CurrentNumber");
        }
        #endregion
    }
}
