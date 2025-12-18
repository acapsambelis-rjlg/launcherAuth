using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS.Data.DataSerializer
{
    public sealed class SNSTimer
    {
        Stopwatch stopwatch = new Stopwatch();
        private static readonly float tickFrequency = 1000f / Stopwatch.Frequency;
        public static int GetTickCount()
        {
            return (int)(Stopwatch.GetTimestamp() * tickFrequency) & int.MaxValue;
            //return System.Environment.TickCount & int.MaxValue;
            //return (int)( System.DateTime.Now.Ticks / 10000);
        }
        private long keeptime;
        public long StartTicks
        {
            get
            {
                return keeptime;
            }
            set
            {
                keeptime = value;
            }
        }
        private long amount;
        private bool continuous;
        private bool stopped;
        public long TimeLimit
        {
            get
            {
                return amount;
            }
            set
            {
                amount = value;
            }
        }
        public bool Continuous
        {
            get
            {
                return continuous;
            }
            set
            {
                continuous = value;
            }
        }
        public void Stop()
        {
            stopped = true;
        }

        public void Start()
        {
            SetTime();
        }
        public long SetTime()
        {
            stopped = false;
            keeptime = GetTickCount();
            return keeptime;
        }
        public bool Stopped
        {
            get
            {
                return stopped;
            }
            set
            {
                stopped = value;
            }
        }
        public int ElapsedTime
        {
            get
            {
                return (int)(GetTickCount() - keeptime);
            }
        }
        public bool Elapsed()
        {
            if (!stopped)
            {
                int ndif = (int)((GetTickCount() - keeptime));
                if (ndif >= amount)
                {

                    if (Continuous)
                    {
                        SetTime();
                    }
                    else
                    {
                        stopped = true;
                    }
                    return true;
                }
            }
            return false;
        }
        public SNSTimer()
        {
            stopped = true;
            continuous = false;
            amount = 0;
        }
        public SNSTimer(bool StartNow, long TimeIncrement, bool Forever)
        {
            stopped = true;
            continuous = Forever;
            amount = TimeIncrement;
            if (StartNow)
            {
                SetTime();
            }
        }
    }

}
