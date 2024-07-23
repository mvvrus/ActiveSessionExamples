using MVVrus.AspNetCore.ActiveSession;
using System.Text.RegularExpressions;

namespace SampleApplication
{
    public static class RunnerKeyExtensions
    {
        public const String RunnerKeyTemplate = @"(\d+)-(\d+)";

        public static Boolean TryParse(this String Source, out RunnerKey RunnerKey)
        {
            Int32 num, gen;
            if(Source == null) throw new ArgumentNullException(nameof(Source));
            RunnerKey=default;
            Match key_parts = Regex.Match(Source, RunnerKeyTemplate);
            if (key_parts.Success 
                && key_parts.Groups.Count==3 
                && Int32.TryParse(key_parts.Groups[1].Value, out gen) 
                && Int32.TryParse(key_parts.Groups[2].Value,out num)) 
            { 
                RunnerKey=(num, gen);
                return true;
            }
            else return false;
        }

        public static RunnerKey Parse(this String Source)
        {
            RunnerKey result;
            if(TryParse(Source, out result)) return result;
            else throw new FormatException("Bad RunnerKey format:"+Source);
        }
    }
}
