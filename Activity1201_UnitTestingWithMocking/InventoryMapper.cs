using AutoMapper;
using InventoryModels;
using InventoryModels.Dtos;

namespace Activity1201_UnitTestingWithMocking
{
    public class InventoryMapper : Profile
    {
        public InventoryMapper()
        {
            CreateMaps();
        }

        private void CreateMaps()
        {
            CreateMap<Item, ItemDto>().ReverseMap();
            CreateMap<Item, CreateOrUpdateItemDto>()
                .ReverseMap()
                .ForMember(x => x.Category, opt => opt.Ignore());
            CreateMap<Category, CategoryDto>()
                .ForMember(x => x.Category, opt => opt.MapFrom(y => y.Name))
                .ReverseMap()
                .ForMember(y => y.Name, opt => opt.MapFrom(x => x.Category));
            CreateMap<CategoryColor, CategoryColorDto>()
                .ForMember(x => x.Color, opt => opt.MapFrom(y => y.ColorValue))
                .ReverseMap()
                .ForMember(y => y.ColorValue, opt => opt.MapFrom(x => x.Color));

        }
    }
}
