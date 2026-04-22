namespace GLMS_ST10445830.Services
{
    // stores a service level name and used a an object,instead of making one many times.
    public class ServiceLevelFlyweight
    {
        public string LevelName { get; set; }

        public ServiceLevelFlyweight(string levelName)
        {
            LevelName = levelName;
        }
    }
}
