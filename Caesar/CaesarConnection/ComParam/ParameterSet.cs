using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarConnection.ComParam
{
    public class ParameterSet
    {
        Dictionary<string, decimal> Values = new Dictionary<string, decimal>();
        List<ParameterMapping> Maps = new List<ParameterMapping>();
        public ParameterSet(Dictionary<string, decimal> defaultValues, List<ParameterMapping> maps) 
        {
            foreach (var row in defaultValues)
            {
                SetParameter(row.Key, row.Value);
            }
            Maps.Clear();
            Maps.AddRange(maps);
        }
        public bool HasParameter(string key)
        {
            var map = Maps.FirstOrDefault(x => x.Source == key);
            if (map != null)
            {
                key = map.Destination;
            }
            return Values.ContainsKey(key);
        }
        public decimal GetParameterOrDefault(string key, decimal defaultIfNull = 0) 
        {
            var map = Maps.FirstOrDefault(x => x.Source == key);
            decimal factor = 1M;
            if (map != null)
            {
                key = map.Destination;
                factor = map.ConversionFactor;
            }
            
            if (Values.TryGetValue(key, out decimal result)) 
            {
                return (result * factor);
            }
            return defaultIfNull;
        }
        public decimal GetParameter(string key)
        {
            var map = Maps.FirstOrDefault(x => x.Source == key);
            decimal factor = 1M;
            if (map != null)
            {
                key = map.Destination;
                factor = map.ConversionFactor;
            }
            return (Values[key] * factor);
        }
        public void SetParameter(string key, decimal value) 
        {
            var map = Maps.FirstOrDefault(x => x.Source == key);
            if (map != null) 
            {
                key = map.Destination;
                value = (value / map.ConversionFactor);
            }
            Values[key] = value;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var row in Values) 
            {
                sb.AppendLine($"{row.Key}={row.Value}");
            }
            foreach (var row in Maps) 
            {
                string value = "NULL";
                if (this.HasParameter(row.Source)) 
                {
                    value = GetParameter(row.Source).ToString();
                }
                sb.AppendLine($"{row.Source}->{row.Destination}={value}");
            }
            return sb.ToString();
        }
    }
}