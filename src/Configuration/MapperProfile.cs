using AutoMapper;
using api_financiamento.src.Models;
using api_financiamento.src.Shared.DTOs;

namespace api_financiamento.src.Configuration
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            #region SIMULATOR
            CreateMap<UpdateSimulationDTO, Simulation>().ReverseMap();
            #endregion
        }
    }
}
