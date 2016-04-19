using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace csharpFramework.Services
{
    public class ServiceState
    {
        // constant definition
        private long CST_WAIT_HINT = 100000;

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public long dwServiceType;
            public enServiceStatus dwCurrentState;
            public long dwControlsAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;
        };

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        /** internal current state definition */
        private enServiceStatus currentState;

        /** internal setter on the service status
         * @param [in] handle on the service 
         */
        private bool setServiceStatus(IntPtr handle, enServiceStatus state)
        {
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = state;
            serviceStatus.dwWaitHint = CST_WAIT_HINT;
            bool result = SetServiceStatus(handle, ref serviceStatus);
            if (true == result)
            {
                currentState = state;
            }
            return result;
        }

        /** setter on the service status as service is start pending
         * @param [in] handle on the service 
         */
        public bool setServicePending(IntPtr handle)
        {
            return setServiceStatus(handle, enServiceStatus.SERVICE_START_PENDING);
        }

        /** setter on the service status as service is running
         * @param [in] handle on the service 
         */
        public bool setServiceRunning(IntPtr handle)
        {
            return setServiceStatus(handle, enServiceStatus.SERVICE_RUNNING);
        }

        /** setter on the service status as service is stop pendind
         * @param [in] handle on the service 
         */
        public bool setServiceStopPending(IntPtr handle)
        {
            return setServiceStatus(handle, enServiceStatus.SERVICE_STOP_PENDING);
        }

        /** setter on the service status as service is stopped
         * @param [in] handle on the service 
         */
        public bool setServiceStopped(IntPtr handle)
        {
            return setServiceStatus(handle, enServiceStatus.SERVICE_STOPPED);
        }
    }

}
