using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Brands.Dtos
{
    public class CreatedBrandDto
    {
        public string Name { get; set; }

        public CreatedBrandDto(string name)
        {
            Name = name;
        }
    }
}
