using System.Collections;
using System.Collections.Generic;
using Core.CustomIdentification;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode
{
    public class UniqueIdTest
    {
        [Test]
        public void CustomLoggerTestSimplePasses()
        {
            UniqueId uniqueId = new UniqueId
            {
                Id = 0,
                Kind = UniqueKind.Player,
            };

            Assert.AreEqual(new UniqueId
            {
                Id = 0,
                Kind = UniqueKind.Player,
            },
            uniqueId);

        }


    }
}