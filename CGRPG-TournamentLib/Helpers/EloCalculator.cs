using System;

namespace CGRPG_TournamentLib.Helpers
{
    public class EloCalculator
    {
        private static double Probability(double rating1,
            double rating2)
        {
            return 1.0f * 1.0f / (1 + 1.0f *
                Math.Pow(10, 1.0f *
                    (rating1 - rating2) / 400));
        }
        
        public static (double, double) Rating(double ra, double rb,
            int k, bool d)
        { 
      
            // To calculate the Winning
            // Probability of Player B
            double pb = Probability(ra, rb);
      
            // To calculate the Winning
            // Probability of Player A
            double pa = Probability(rb, ra);
      
            // Case when Player A wins
            // Updating the Elo Ratings
            if (d) 
            {
                ra += k * (1 - pa);
                rb += k * (0 - pb);
            }
            // Case when Player B wins
            // Updating the Elo Ratings
            else 
            {
                ra += k * (0 - pa);
                rb += k * (1 - pb);
            }
            
            return (Math.Round(ra * 1000000.0) / 1000000.0, Math.Round(rb * 1000000.0) / 1000000.0);
        }
    }
}