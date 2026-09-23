using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheCharityBLL.DTOs.UserDTOs;
using TheCharityDAL.Entities;

namespace TheCharityBLL.Mapper.UserMapper
{
    [Mapper]
    public partial class UserResponse
    {
        public partial UserResponseDTO MapToUserResponseDto(User User);
        public partial User MapToUser(UserResponseDTO User);
        public partial List<UserResponseDTO> MapToUserDtoList(List<User> Users);
    }
}
