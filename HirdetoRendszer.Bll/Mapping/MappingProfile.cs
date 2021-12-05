using AutoMapper;
using HirdetoRendszer.Bll.Dto.Common;
using HirdetoRendszer.Bll.Dto.Elofizetes;
using HirdetoRendszer.Bll.Dto.Fedelzeti;
using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Bll.Dto.Jarmu;
using HirdetoRendszer.Bll.Dto.Kep;
using HirdetoRendszer.Bll.Dto.Vonal;
using HirdetoRendszer.Common.Enum;
using HirdetoRendszer.Dal.Model;
using System.Linq;

namespace HirdetoRendszer.Bll.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Vonal

            CreateMap<Vonal, VonalDto>();
            CreateMap<HirdetesToVonal, VonalDto>()
                .ForMember(dest => dest.JarmuTipus, opt => opt.MapFrom(src => src.Vonal.JarmuTipus))
                .ForMember(dest => dest.Nev, opt => opt.MapFrom(src => src.Vonal.Nev))
                .ForMember(dest => dest.VonalId, opt => opt.MapFrom(src => src.VonalId));

            // Hirdetes

            CreateMap<Hirdetes, HirdetesDto>()
                .ForMember(dest => dest.Ervenyesseg, opt => opt.MapFrom(
                    src => (src.ErvenyessegKezdet.HasValue && src.ErvenyessegVeg.HasValue) ? new IdotartamDto() {
                        Kezdet = src.ErvenyessegKezdet.Value,
                        Veg = src.ErvenyessegVeg.Value,
                    } : null
                ));

            // Hirdetés helyettesítő

            CreateMap<HirdetesHelyettesito, HirdetesHelyettesitoDto>()
                .ForMember(dest => dest.Ervenyesseg, opt => opt.MapFrom(
                    src => (src.ErvenyessegKezdet.HasValue && src.ErvenyessegVeg.HasValue) ? new IdotartamDto() {
                        Kezdet = src.ErvenyessegKezdet.Value,
                        Veg = src.ErvenyessegVeg.Value,
                    }: null
                ));

            // Kep

            CreateMap<KepToHirdetes, KepDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Kep.Url));

            CreateMap<KepToHirdetesHelyettesito, KepDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Kep.Url));

            // Jarmu

            CreateMap<HirdetesHelyettesitoToJarmu, JarmuDto>()
                .ForMember(dest => dest.Azonosito, opt => opt.MapFrom(src => src.Jarmu.Azonosito))
                .ForMember(dest => dest.JarmuTipus, opt => opt.MapFrom(src => src.Jarmu.JarmuTipus));

            // Elofizetes

            CreateMap<Elofizetes, ElofizetesDto>()
                .ForMember(
                    dest => dest.ElhasznaltIdotartam,
                    opt => opt.MapFrom(src => src.ElofizetesTipus == ElofizetesTipus.Mennyisegi
                        ? ((MennyisegiElofizetes)src).ElhasznaltIdotartam
                        : ((HaviElofizetes)src).HaviElofizetesReszletek.Sum(h => h.ElhasznaltPercek)))
                .ForMember(
                    dest => dest.VasaroltIdotartam,
                    opt => opt.MapFrom(src => src.ElofizetesTipus == ElofizetesTipus.Mennyisegi
                        ? (int?)((MennyisegiElofizetes)src).VasaroltIdotartam
                        : null))
                .ForMember(
                    dest => dest.HaviLimit,
                    opt => opt.MapFrom(src => src.ElofizetesTipus == ElofizetesTipus.Havi
                        ? (int?)((HaviElofizetes)src).HaviLimit
                        : null))
                .ForMember(
                    dest => dest.Aktiv,
                    opt => opt.MapFrom(src => src.ElofizetesTipus == ElofizetesTipus.Havi
                        ? (bool?)((HaviElofizetes)src).Aktiv
                        : null));
        }
    }
}
