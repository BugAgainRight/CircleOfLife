using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public static class UIDCreater
    {
        public static string CreatID()
        {
            string id = Guid.NewGuid().ToString();
            return id;
        }
    }
}
