namespace SLGIS.Core
{
    public class Location
    {
        public string Lat { get; set; }
        public string Lng { get; set; }

        public override string ToString()
        {
            return $"{Lat}, {Lng}";
        }
    }
}
