using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M5.KernScaleTest.Model
{
    public class WeightResult
    {
        #region Constructors
        public WeightResult()
        { }
        #endregion

        #region Fields
        private int _amountWeights;
        private DateTime _date;
        private string _result;
        private List<decimal> _individualResults;
        private StringBuilder _individualResultsSting;
        private long _runningTime = 0;
        private string _currentNumber = "0";
        #endregion

        #region Proerties
        public int AmountWeights { get => _amountWeights; set => _amountWeights = value; }
        public DateTime Date { get => _date; set => _date = value; }
        public string Result { get => _result; set => _result = value; }
        public List<decimal> IndividualResults { get => _individualResults; set => _individualResults = value; }
        public StringBuilder IndividualResultsSting { get => _individualResultsSting; set => _individualResultsSting = value; }
        public long RunningTime { get => _runningTime; set => _runningTime = value; }
        public string CurrentNumber { get => _currentNumber; set => _currentNumber = value; }
        #endregion
    }
}
