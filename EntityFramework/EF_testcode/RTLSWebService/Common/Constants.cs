namespace RTLSWebService.Common
{
    public class Constants
    {
        public enum Faults { Panic = 6000, HighTemp, LowTemp, AllIsNotWell, InActive, Out, LowBattery };

        public enum Status { Active = 1, InActive, Out, Panic, FIn, FOut, In };

        public enum StatusSaved { No = 0, Yes };

        public enum Tag { UnAssignment = 0, Assignment, No_Transaction }

        public enum DataRC
        {
            Success = 200, NoChangeInMasterData = 202, NoOnlineDataAvailable = 203, StatusPostedSuccessfully = 204, LoginSuccess = 206, InvalidCredentials = 208,
            WrongPassword = 210, InternalException = 212, RequestBodyIsEmpty = 214
        }
        public enum zoneSafety { unsafeZone = 0, safeZone = 1 }


    }
}