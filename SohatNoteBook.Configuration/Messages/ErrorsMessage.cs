using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SohatNoteBook.Configuration.Messages
{
    public static class ErrorsMessage
    {
        public static class Generic
        {
            public static string ObjectNotFound = "Object Not Found";
            public static string InvalidRequest = "InvalidRequest";
            public static string TypeBadRequest = "Bad Request";
            public static string InvalidPayload = "Invalid Payload";
            public static string SomethingWentWrong = "Something went wrong, please try again later";
            public static string UnableToProcess = "Unable to process request";
        }

        public static class ProfileMessage
        {
            public static string UserNotFound = "UserNotFound";
        }

        public static class UserMessage
        {
            public static string UserNotFound = "UserNotFound";
        }
    }
}
