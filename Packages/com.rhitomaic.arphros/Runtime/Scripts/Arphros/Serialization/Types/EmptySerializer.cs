using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArphrosFramework
{
    public class EmptySerializer : ObjectSerializer
    {
        public override void Deserialize(string data)
        {
            base.Deserialize(data);
            tag = "Empty";
        }

        public override string Serialize()
        {
            return null;
        }
    }
}
