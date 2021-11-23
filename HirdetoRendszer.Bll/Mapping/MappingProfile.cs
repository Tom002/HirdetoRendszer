﻿using AutoMapper;
using HirdetoRendszer.Bll.Dto.Elofizetes;
using HirdetoRendszer.Bll.Dto.Hirdetes;
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

            CreateMap<Hirdetes, HirdetesDto>();

            // Kep

            CreateMap<KepToHirdetes, KepDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Kep.Url));

            CreateMap<KepToHirdetesHelyettesito, KepDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Kep.Url));

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
                        ? ((MennyisegiElofizetes)src).VasaroltIdotartam
                        : default))
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