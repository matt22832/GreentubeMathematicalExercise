using System;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;

namespace simulator
{
    class simulator
    {
        public static double payout { get; set; }
        public static double specialPayout { get; set; }
        public static string payline { get; set; }
        public static string[] reels { get; set; }
        public static Boolean[] reelsAreSpecial { get; set; }
        public static char specialSymbol { get; set; }
        public static int specialSymbolValue { get; set; }
        public static Dictionary<char, int[]> winPlans { get; set; }

        static void initialise()
        {
            payout = 0;
            specialPayout = 0;
            payline = "";
            reelsAreSpecial = new Boolean[] { false, false, false, false, false };

            reels = new string[] { "NTJPNTJQCKNTPJAQKNJNTPJTAKQA", "ANPQJKATJNQJKPTNATNJTPQNJKTN", "QPKNAPQTKPJTAPNKTJQAPKNACJAK", "ANPQJKATJNQJKPTNATNJTPQNJKTN", "NTJPNTJQCKNTPJAQKNJNTPJTAKQA" };
            specialSymbol = 'C';
            specialSymbolValue = 100;

            winPlans = new Dictionary<char, int[]>();
            winPlans.Add('N', new int[] { 20, 40, 80 });
            winPlans.Add('T', new int[] { 20, 40, 80 });
            winPlans.Add('J', new int[] { 20, 40, 80 });
            winPlans.Add('Q', new int[] { 40, 80, 160 });
            winPlans.Add('K', new int[] { 50, 100, 200 });
            winPlans.Add('A', new int[] { 75, 150, 300 });
            winPlans.Add('P', new int[] { 100, 200, 500 });
        }
        static void Main()
        {
            initialise();
            //check every value the reels can be in and update payout and specialPayout
            iterateNextReelAndUpdatePayout(0);

            double totalPayout = payout + specialPayout;
            double totalNumberOfSpins = reels[0].Length * reels[1].Length * reels[2].Length * reels[3].Length * reels[4].Length;
            double returnToPlayer = totalPayout / totalNumberOfSpins;
            Console.WriteLine(returnToPlayer);
        }
        static void iterateNextReelAndUpdatePayout(int currentReel)
        // Increase the position of the current reel by one and use the new value to update the payline and check for special characters.
        // If this is the final reel then use those values to update the payout. Otherwise call this method again to get the next reel.
        {
            for (int count = 0; count < reels[currentReel].Length; count++)
            {
                payline += reels[currentReel][count];
                reelsAreSpecial[currentReel] = isSpecialCharacterOnScreen(reels[currentReel], count);
                if (currentReel < reels.Length - 1)
                {
                    iterateNextReelAndUpdatePayout(currentReel + 1);
                }
                else
                {
                    updatePayout();
                }
                payline = payline.Remove(payline.Length - 1, 1);
                reelsAreSpecial[currentReel] = false;
            }
        }
        static void updatePayout()
        {
            payout += getPayout();
            payout += getSpecialPayout();
        }
        static bool isSpecialCharacterOnScreen(string reel, int count)
        // Check if the special character is shown at the current position on the reel or either side of it.
        // If the current position is the first or last then loop around to the otherside of the string.
        {
            if (count == 0)
            {
                if ((reel[0] == specialSymbol) || (reel[1] == specialSymbol) || (reel[reel.Length - 1] == specialSymbol))
                {
                    return true;
                }
                else
                    return false;
            }
            if (count == reel.Length)
            {
                if ((reel[0] == specialSymbol) || (reel[reel.Length - 2] == specialSymbol) || (reel[reel.Length] == specialSymbol))
                {
                    return true;
                }
                else
                    return false;
            }
            else if (0 < count && count < (reel.Length - 1))
            {
                if ((reel[count - 1] == specialSymbol) || (reel[count] == specialSymbol) || (reel[count + 1] == specialSymbol))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        static int getPayout()
        // For each sysmbol in the win-plan check if there is a consecutive string of 3, 4 or 5 of those symbols and return the correct payout.
        {
            foreach (var winPlan in winPlans)
            {
                var symbol = winPlan.Key;
                if (payline.Contains(new string(symbol, 5)))
                {
                    return winPlan.Value[2];
                }
                else if (payline.Contains(new string(symbol, 4)))
                {
                    return winPlan.Value[1];
                }
                else if (payline.Contains(new string(symbol, 3)))
                {
                    return winPlan.Value[0];
                }
            }
            return 0;
        }
        static int getSpecialPayout()
        // If 3 or more of the reels display a special character then return the special character payout
        {
            if (reelsAreSpecial.Count(x => x) >= 3)
            {
                return specialSymbolValue;
            }
            else
                return 0;
        }
    }
}