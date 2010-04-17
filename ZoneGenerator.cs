using System;
using System.Linq;
using System.Collections.Generic;

namespace SomePathingThing
{
    public class ZoneGenerator
    {
        public ZoneRecommender ZoneRecommender { get; set; }

        public ZoneGenerator(ZoneRecommender zoneRecommender)
        {
            this.ZoneRecommender = zoneRecommender;
        }

        public IEnumerable<Rectangle> GenerateZones(float[][] map)
        {
            var zones = new List<Rectangle>();
            while (true)
            {
                var recommendations = ZoneRecommender.RecommendZones(map);
                zones.Add(recommendations.OrderBy(r => r.GetArea()).First());
            }
            return zones;
        }
    }
}
