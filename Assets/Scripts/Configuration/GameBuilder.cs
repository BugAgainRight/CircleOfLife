using Milutools.SceneRouter;
using UnityEngine;

namespace CircleOfLife.Configuration
{
    public class GameBuilder : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Setup()
        {
            SceneRouter.Setup(new SceneRouterConfig()
            {
                SceneNodes = new []
                {
                    SceneRouter.Node(SceneIdentifier.WeatherTest, "test/weather-system", "WeatherSystem")
                }
            });
        }
    }
}
