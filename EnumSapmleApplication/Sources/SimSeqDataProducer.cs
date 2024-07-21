using SapmleApplication.Models;
using System.Text;

namespace SapmleApplication.Sources
{
    public class SimSeqDataProducer
    {
         Random _random = new Random();

        public SimSeqData Sample(int index) 
        {
            Int32 rnd_data = _random.Next(10000,100000);
            Int32 name_len = _random.Next(4, 10);
            StringBuilder name = new StringBuilder(name_len);
            for(int i = 0; i<name_len; i++) {
                Char c = (Char)(Convert.ToInt16('a')+_random.Next(0, 26));
                if(i==0) c=Char.ToUpper(c);
                name.Append(c);
            }
            return new SimSeqData { Number=index, Data=rnd_data, Name=name.ToString()};
        }
    }
}
