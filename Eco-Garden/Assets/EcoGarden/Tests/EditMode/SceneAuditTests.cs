using EcoGarden.Editor;
using NUnit.Framework;
using System.Collections.Generic;

namespace EcoGarden.Tests.EditMode
{
    public sealed class SceneAuditTests
    {
        [Test]
        public void Level15Scene_HasRequiredReferences()
        {
            List<string> issues = EcoGardenSceneAudit.AuditLevel15SceneReferences();

            Assert.IsEmpty(issues, string.Join("\n", issues));
        }
    }
}
