using AutoMapper;
using RESTfulAPI_Technical_Task_.Model;

namespace RESTfulAPI_Technical_Task_.Composable
{
    public class MappingModels : Profile
    {
        public MappingModels()
        {
            // Маппинг задач
            CreateMap<TaskModel, TaskDTO>().ReverseMap();       // TaskModel <-> TaskDTO
            CreateMap<TaskModel, UpdateTaskDTO>().ReverseMap(); // TaskModel <-> UpdateTaskDTO

            // Маппинг токенов
            CreateMap<TokenDTO, TokenDTO>(); // Токены

            // Маппинг логина (если понадобится)
            CreateMap<LoginRequest, LoginRequest>();
        }
    }
}
