namespace SLGIS.Core
{
    public class Location
    {
        public string Lat { get; set; } = "0";
        public string Lng { get; set; } = "0";

        public override string ToString()
        {
            return $"{Lat}, {Lng}";
        }
    }
}
