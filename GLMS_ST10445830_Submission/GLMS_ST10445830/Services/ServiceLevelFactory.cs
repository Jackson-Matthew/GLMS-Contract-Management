namespace GLMS_ST10445830.Services
{
    //flyweight pattern

    //acts as a  factory and a cache that reuses instances instead of duplicating
    public class ServiceLevelFactory
    {

        //caches the flyweight objects using a dictionary
        private static readonly Dictionary<string, ServiceLevelFlyweight> _levels = new();

        /*retrives the object form the cahce if it exists, otherwise
         * creates a new one and adds it to the cache
         */
        public static ServiceLevelFlyweight GetLevel(string levelName)
        {
            if (!_levels.ContainsKey(levelName))
            {
                _levels[levelName] = new ServiceLevelFlyweight(levelName);
            }
            return _levels[levelName];
        }
    }
}
