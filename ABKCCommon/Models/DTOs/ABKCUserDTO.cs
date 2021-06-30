using System.Collections.Generic;

namespace ABKCCommon.Models.DTOs
{
    public class ABKCUserDTO
    {

        public int Id { get; set; }
        public string OktaId { get; set; }
        public string LoginName { get; set; }
        public ICollection<RoleDTO> Roles { get; set; }
        public string IsSuspended { get; set; }
    }


    public class RoleDTO
    {
        public string Name { get; set; }
        public int RoleTypeId { get; set; }
    }
}