﻿using System.ComponentModel.DataAnnotations;

namespace Kontecg.Organizations.Dto
{
    public class RolesToOrganizationUnitInput
    {
        public int[] RoleIds { get; set; }

        [Range(1, long.MaxValue)] public long OrganizationUnitId { get; set; }
    }
}
