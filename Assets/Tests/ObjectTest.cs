using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace ArphrosFramework.Tests {
    public class ObjectTest {
        private const string TestSceneName = "SampleScene";

        [UnitySetUp]
        public IEnumerator SetUp() {
            SceneManager.LoadScene(TestSceneName);
            yield return null;

            while (SceneManager.GetActiveScene().name != TestSceneName)
                yield return null;
        }

        [UnityTest]
        public IEnumerator LevelManager_InstanceExists() {
            yield return new WaitUntil(() => References.AreReferencesInitialized);

            // --- Core game managers ---
            Assert.IsNotNull(References.Manager, "References.Manager should be assigned.");
            Assert.IsNotNull(References.Player, "References.Player should be assigned.");
            Assert.IsNotNull(References.MainCamera, "References.MainCamera should be assigned.");
            Assert.IsNotNull(References.Camera, "References.Camera (CameraMovement) should be assigned.");
            Assert.IsNotNull(References.CamPort, "References.CamPort should be assigned.");
            Assert.IsNotNull(References.DirectionalLight, "References.DirectionalLight should be assigned.");
            Assert.IsNotNull(References.Porter, "References.Porter should be assigned.");
            Assert.IsNotNull(References.ButtonMixerGroup, "References.ButtonMixerGroup should be assigned.");

            // --- Optional camera variants ---
            Assert.IsNotNull(References.WeirdCamera, "References.WeirdCamera should be assigned.");
            Assert.IsNotNull(References.OldCamera, "References.OldCamera should be assigned.");

            // --- Sanity check: AreReferencesInitialized flag ---
            Assert.IsTrue(References.AreReferencesInitialized, "AreReferencesInitialized should be true after Awake().");

        }

        [UnityTearDown]
        public IEnumerator TearDown() {
            if (SceneManager.GetActiveScene().name == TestSceneName) {
                var unloadOp = SceneManager.UnloadSceneAsync(TestSceneName);
                if (unloadOp != null) {
                    while (!unloadOp.isDone)
                        yield return null;
                }
            }

            References.Clear();
            yield return null;
        }
    }
}