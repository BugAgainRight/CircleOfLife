using CircleOfLife.General;
using Milease.Configuration;
using Milutools.Milutools.UI;
using Milutools.SceneRouter;
using UnityEngine;

namespace CircleOfLife.Configuration
{
    public class GameBuilder : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Setup()
        {
            MileaseConfiguration.Configuration.DefaultColorTransformationType = ColorTransformationType.RGB;
            SetupSceneRouter();
            SetupUIManager();
        }

        private static void SetupSceneRouter()
        {
            SceneRouter.Setup(new SceneRouterConfig()
            {
                SceneNodes = new []
                {
                    SceneRouter.Node(SceneIdentifier.WeatherTest, "test/weather-system", "WeatherSystem")
                }
            });
        }

        private static void SetupUIManager()
        {
            UIManager.Setup(new []
            {
                UI.FromResources(UIIdentifier.MessageBox, "UI/MessageBox")
            });
        }
    }
}
