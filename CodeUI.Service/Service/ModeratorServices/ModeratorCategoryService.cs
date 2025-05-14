using AutoMapper;
using CodeUI.Data.Entity;
using CodeUI.Data.UnitOfWork;
using CodeUI.Service.DTO.Response;
using CodeUI.Service.DTO.Response.ElementResponses;
using CodeUI.Service.Exceptions;
using CodeUI.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeUI.Service.Helpers.Enum;
using static CodeUI.Service.Helpers.ErrorEnum;

namespace CodeUI.Service.Service.ModeratorServices
{
    public interface IModeratorCategoryService
    {
        
    }
    public class ModeratorCategoryService : IModeratorCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ModeratorCategoryService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork= unitOfWork;
            _mapper= mapper;
        }
    }
}
