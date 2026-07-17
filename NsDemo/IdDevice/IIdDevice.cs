using NUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsDemo
{
    internal interface IIdDevice
    {
        bool Init(Action<string, bool> ActLog);
        bool Login();
        void Logout();

        void Enroll(UserGroup group, string[] RightName);
        void Stop_Enroll();
    }
}
