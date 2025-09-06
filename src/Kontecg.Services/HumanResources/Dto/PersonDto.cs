using Kontecg.Application.Services.Dto;
using Kontecg.Runtime.Validation;
using System;

namespace Kontecg.HumanResources.Dto
{
    public class PersonDto : EntityDto<long>, IShouldNormalize
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Lastname { get; set; }

        public string FullName { get; set; }

        public string IdentityCard { get; set; }

        public string Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public int Age { get; set; }

        public Address Address { get; set; }

        public Etnia Etnia { get; set; }

        public ClothingSize ClothingSize { get; set; }

        public string ScholarshipLevel { get; set; }

        public string Scholarship { get; set; }

        public byte[] Photo { get; set; }

        public string PhotoId { get; set; }

        public string PhotoFileType { get; set; }

        /// <inheritdoc />
        public void Normalize()
        {
            Name = Name?.ToUpperInvariant();
            Surname = Surname?.ToUpperInvariant();
            Lastname = Lastname?.ToUpperInvariant();
        }
    }
}
