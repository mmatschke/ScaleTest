using System.Diagnostics;

namespace M5.KernScaleTest.Model
{
    public class StopWatchLogger
    {
        #region Constructor
        public StopWatchLogger()
        {

        }
        #endregion

        #region Fields
        private Stopwatch _stopWatch = new Stopwatch();
        private long _getRunningTimes = 0;
        #endregion

        #region Properties
        public Stopwatch StopWatch { get => _stopWatch; set => _stopWatch = value; }

        public long GetRunningTime
        {
            get
            {
                if (_stopWatch != null)
                {
                    _getRunningTimes = _stopWatch.ElapsedMilliseconds;
                }
                return _getRunningTimes;
            }
        }
        #endregion

        #region PublicMethods
        public void Start()
        {
            _stopWatch.Start();
        }

        public void Stop()
        {
            if (_stopWatch.IsRunning)
                _stopWatch.Stop();
        }
        #endregion
    }
}
