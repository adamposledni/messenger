using AutoMapper;
using Messenger.Web.Entities;
using Messenger.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messenger.Web.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserRes>();
            CreateMap<User, UserNameRes>();
            CreateMap<Chat, ChatRes>();
            CreateMap<Message, MessageRes>();
        }
    }
}
