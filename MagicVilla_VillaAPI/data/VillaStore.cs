using MagicVilla_VillaAPI.model.Dto;

namespace MagicVilla_VillaAPI.data
{
    public static class VillaStore
    {
        public static List<VillaDTO> vilaList= new List<VillaDTO> {
                new VillaDTO {Id=1,Name="getch",Occupancy=3,Sqft=100},
                new VillaDTO {Id=2,Name="lulie",Occupancy=4,Sqft=300}
            };
    }
}
